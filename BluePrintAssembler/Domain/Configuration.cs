using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BluePrintAssembler.Annotations;
using BluePrintAssembler.Steam;
using BluePrintAssembler.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLua;

namespace BluePrintAssembler.Domain
{
    class Configuration : INotifyPropertyChanged
    {
        private Configuration()
        {
        }

        private RawData _rawData;
        private static Configuration _instance;
        private string _loadStatus;
        public static Configuration Instance => _instance ?? (_instance = new Configuration());

        public Task<string[]> DiscoverFolders()
        {
            return Task.Run(() =>
            {
                LoadStatus = "Discovering game/mods location…";
                var sd = new SteamPathDiscoverer();
                return new[] {sd.GetBasePath()}.Concat(sd.GetModsPaths()).ToArray();
            });
        }

        public async Task<ModInfo[]> GetMods(string[] modFolders)
        {
            LoadStatus = "Determining enabled mods…";
            var allItems = await Task.WhenAll(modFolders.Select(folder => Task.Run(() =>
            {
                if (File.Exists(Path.Combine(folder, "mod-list.json")))
                    return ((JArray) ((JObject) JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(folder, "mod-list.json"))))["mods"])
                        .Select(x => new ModInfo {Name = x["name"].Value<string>(), Enabled = x["enabled"].Value<bool>()});
                else
                    return new ModInfo[0];
            })).ToArray());
            return new[] {new ModInfo {Name = "core", Enabled = true}}.Concat(allItems.SelectMany(x => x)).ToArray();
        }

        public async Task<JObject> GetModSettings(string[] modFolders)
        {
            LoadStatus = "Reading mod settings…";
            var allItems = await Task.WhenAll(modFolders.Select(folder => Task.Run(() =>
            {
                if (File.Exists(Path.Combine(folder, "mod-settings.json")))
                    return (JObject) JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(folder, "mod-settings.json")));
                else
                    return new JObject();
            })).ToArray());
            return allItems.Aggregate(new JObject(), (acc, obj) =>
            {
                acc.Merge(obj);
                return acc;
            });
        }

        public async Task Load()
        {
            var folders = await DiscoverFolders();
            var mods = await GetMods(folders);
            var modSettings = await GetModSettings(folders);
            LoadStatus = "Locating enabled mods…";
            Task.WaitAll(mods.Where(x => x.Enabled).Select(x => x.MapToFilesystem(folders)).ToArray());
            LoadStatus = $"Guessing load order…";
            mods = mods.Where(x => x.Enabled).TopologicalSort((a, b) => a.Name != b.Name && ((b.Name == "core") || (b.Name == "base" && a.Name != "core") || (a.Dependencies?.Any(x => x.TrimStart(' ', '?').StartsWith(b.Name)) ?? false))).ToArray();

            var verStamp = mods.Aggregate(new StringBuilder("BluePrintAssembler.").Append(Assembly.GetEntryAssembly().GetName().Version), (builder, mod) => builder.Append('|').Append(mod.Name).Append('.').Append(mod.Version?.ToString() ?? "*"));
            verStamp.Append("|").Append(modSettings.ToString(Formatting.None));
            var verStampHash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(verStamp.ToString()))
                .Aggregate(new StringBuilder(), (builder, b) => builder.Append(b.ToString("x2"))).ToString();
            var cacheFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BluePrintAssembler", verStampHash + ".cache");
            if (File.Exists(cacheFile))
            {
                LoadStatus = "Loading data from cache…";
                _rawData = JsonConvert.DeserializeObject<RawData>(File.ReadAllText(cacheFile));
            }
            else
            {
                LoadStatus = "Loading mods…";
                var rawData = await LoadMods(modSettings, mods);
                _rawData = rawData.ToObject<RawData>();
                if (!Directory.Exists(Path.GetDirectoryName(cacheFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(cacheFile));
                File.WriteAllText(cacheFile+"_raw", JsonConvert.SerializeObject(rawData, Formatting.Indented));
                File.WriteAllText(cacheFile, JsonConvert.SerializeObject(_rawData, Formatting.Indented));
            }
            LoadStatus = "All done!";
        }

        private static readonly Regex LuaPropertyBlacklist = new Regex(@"((_picture)|(achievement)|(bounding_box)|(offsets)|(remnants)|(sound?)|(sprites?)|(graphics?)|(animations?)$)|(^(picture)|(hr_version)|(circuit_wire_connection_point)|(tutorial)|(technology)|(fluid_boxes)|(collision_box)|(noise-expression)|(decorative)|(smoke)|(projectile)|(font)|(unit)|(unit-spawner)|(beam)|(tree)|(tile)|(fire)|(corpse)|(explosion)$)", RegexOptions.Compiled);
        private Task<JObject> LoadMods(JObject modSettings, ModInfo[] mods)
        {
            return Task.Run(() =>
            {
                using (var lua = new Lua())
                {
                    InitLuaParser(lua, mods.First(), mods, modSettings);

                    //Unpack mods
                    foreach (var mod in mods)
                    {
                        mod.LoadPath = mod.BasePath;
                        if (File.Exists(mod.LoadPath))
                        {
                            LoadStatus = $"Unpacking mod {mod.Name}…";
                            mod.TempPath = mod.LoadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BluePrintAssembler", $"UNPACKED_{mod.Name}_{mod.Version?.ToString() ?? "SRC"}");
                            if (!Directory.Exists(mod.LoadPath))
                            {
                                Directory.CreateDirectory(mod.LoadPath);
                            }{
                                using (var zfs = File.OpenRead(mod.BasePath))
                                using (var ar = new ZipArchive(zfs))
                                {
                                    foreach (var entry in ar.Entries)
                                    {
                                        if (!entry.Name.EndsWith(".lua") && entry.Name != "info.json") continue;
                                        var fullPath = Path.Combine(mod.LoadPath, entry.FullName);
                                        if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                                            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                                        if (entry.Length > 0)
                                        {
                                            using (var fs = entry.Open())
                                            using (var ofs = File.Create(fullPath))
                                                fs.CopyTo(ofs);
                                        }
                                    }
                                }
                            }
                        }

                        mod.LoadPath = Path.GetDirectoryName(Directory.EnumerateFiles(mod.LoadPath, "info.json", SearchOption.AllDirectories).First());
                    }

                    void ProcessFileInAllMods(string filename, string message)
                    {
                        foreach (var mod in mods)
                        {
                            if (File.Exists(Path.Combine(mod.LoadPath, filename)))
                            {
                                LoadStatus = string.Format(message, mod.Name);
                                SetupLuaParser(lua, mods.First(), mod);
                                lua.DoFile(Path.Combine(mod.LoadPath, filename));
                            }
                        }
                    }
                    ProcessFileInAllMods("settings.lua","Executing settings.lua from {0}…");
                    ProcessFileInAllMods("settings-updates.lua","Executing settings-updates.lua from {0}…");
                    ProcessFileInAllMods("settings-final-fixes.lua","Executing settings-final-fixes.lua from {0}…");
                    ProcessFileInAllMods("data.lua","Executing data.lua from {0}…");
                    ProcessFileInAllMods("data-updates.lua","Executing data-updates.lua from {0}…");
                    ProcessFileInAllMods("data-final-fixes.lua","Executing data-final-fixes.lua from {0}…");

                    LoadStatus = "Extracting raw data…";
                    JObject Convert(LuaTable table)
                    {
                        var rv=new JObject();
                        foreach (var item in table.Keys)
                        {
                            if (item is string str && LuaPropertyBlacklist.IsMatch(str)) continue;
                            var value = table[item];
                            JToken jvalue = null;
                            if (value is LuaTable tbl)
                                jvalue = Convert(tbl);
                            else
                                jvalue = new JValue(value);
                            /*if (item is string && ((string) item == "results" ||(string) item == "ingredients") && value is string)
                            {
                                jvalue = new JObject(new JProperty("1", new JObject(new JProperty("name", value))));
                            }*/
                            rv.Add(item.ToString(), jvalue);
                        }
                        return rv;
                    }

                    var rootObject = Convert(lua.GetTable("data.raw"));
                    LoadStatus = "Removing temporary files…";
                    foreach (var mod in mods)
                    {
                        if (mod.TempPath!=null && mod.TempPath != mod.BasePath)
                            Directory.Delete(mod.TempPath, true);
                    }

                    return rootObject;
                }
            });
        }

        private void SetupLuaParser(Lua lua,ModInfo core,ModInfo mod)
        {
            //Initialize package.path
            lua.GetFunction("___BPA___pkgpath__set___").Call(mod.LoadPath);
            lua.GetFunction("___BPA___pkgpath__add___").Call(Path.Combine(core.BasePath, "lualib"));
            //Some mods (e.g. Angel's Refining) are doing imports with relative paths from within prototypes/ folder
            lua.GetFunction("___BPA___pkgpath__add___").Call(Path.Combine(mod.LoadPath, "prototypes"));
            //"Unload" all previously loaded packages
            lua.DoString("for name, version in pairs(package.loaded) do package.loaded[name]=false end");
        }

        private void InitLuaParser(Lua lua,ModInfo core, IEnumerable<ModInfo> enabledMods,JObject modSettings)
        {
            using (var reader = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("BluePrintAssembler.Lua.Init.lua")))
                lua.DoString(reader.ReadToEnd());
            lua.GetFunction("___BPA___pkgpath__add___").Call(Path.Combine(core.BasePath, "lualib"));
            using (var reader = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("BluePrintAssembler.Lua.FactorioDefs.lua")))
                lua.DoString(reader.ReadToEnd());

            foreach (var mod in enabledMods)
                lua.DoString($"mods[\'{mod.Name}\'] = '{mod.Version?.ToString() ?? ""}'");

            void MakeSettings(StringBuilder builder, string prefix, JProperty obj)
            {
                var val = ((JObject) obj.Value).Property("value");
                builder.Append(prefix);
                builder.Append("[\'");
                builder.Append(obj.Name);
                builder.Append("\']=");
                if (val != null)
                {
                    builder.Append("{value=");
                    if (val.Value.Type == JTokenType.String)
                    {
                        builder.Append('\'');
                        builder.Append(val.Value.Value<string>().Replace("\\", "\\").Replace("'", "\\'"));
                        builder.Append('\'');
                    }
                    else
                    {
                        builder.Append(val.Value.ToString(Formatting.None));
                    }
                    builder.AppendLine("}");
                }
                else
                {
                    builder.AppendLine("{}");
                    prefix = $"{prefix}[\'{obj.Name}\']";
                    foreach (var xprop in ((JObject) obj.Value).Properties())
                        MakeSettings(builder, prefix, xprop);
                }
            }

            var settingsBuilder = new StringBuilder();
            foreach (var setting in modSettings.Properties())
                MakeSettings(settingsBuilder, "settings", setting);
            lua.DoString(settingsBuilder.ToString());

            lua.DoFile(Path.Combine(core.BasePath, "lualib", "dataloader.lua"));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string LoadStatus
        {
            get => _loadStatus;
            private set
            {
                if (value == _loadStatus) return;
                _loadStatus = value;
                OnPropertyChanged();
            }
        }

        public void Test()
        {
            var satisfied=new HashSet<BaseProducibleObject>();
            var satisfiedRaw=new HashSet<BaseProducibleObject>();
            var unsatisfied=new HashSet<BaseProducibleObject>{_rawData.Items["iron-plate"]};
            while (unsatisfied.Any())
            {
                var result = unsatisfied.First();
                var possibleRecipies=_rawData.Recipes.Where(x => x.Value.HasResult(result)).ToArray();
                if (!possibleRecipies.Any())
                {
                    unsatisfied.Remove(result);
                    satisfiedRaw.Add(result);
                }
                else
                {
                    foreach (var r in possibleRecipies.First().Value.CurrentMode.Results)
                    {
                        var res=_rawData.Get(r.Value.Type, r.Value.Name);
                        satisfied.Add(res);
                        unsatisfied.Remove(res);
                    }
                    foreach (var r in possibleRecipies.First().Value.CurrentMode.Sources)
                    {
                        var res=_rawData.Get(r.Value.Type, r.Value.Name);
                        if (!satisfied.Contains(res) && !satisfiedRaw.Contains(res))
                        {
                            unsatisfied.Add(res);
                        }
                    }
                }
            }
        }
    }
}