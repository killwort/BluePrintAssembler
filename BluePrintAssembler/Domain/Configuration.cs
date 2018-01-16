using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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
    internal class Configuration : INotifyPropertyChanged
    {
        private static Configuration _instance;

        private static readonly Regex LuaPropertyBlacklist = new Regex(@"((_picture)|(achievement)|(bounding_box)|(offsets)|(remnants)|(sound?)|(sprites?)|(graphics?)|(animations?)$)|(^(picture)|(hr_version)|(circuit_wire_connection_point)|(tutorial)|(technology)|(fluid_boxes)|(collision_box)|(noise-expression)|(decorative)|(smoke)|(projectile)|(font)|(unit)|(unit-spawner)|(beam)|(tree)|(tile)|(fire)|(corpse)|(explosion)$)", RegexOptions.Compiled);
        private string _loadStatus;

        private Configuration()
        {
        }

        public RawData RawData { get; private set; }
        public static Configuration Instance => _instance ?? (_instance = new Configuration());

        public Bitmap StdIconSlot { get; private set; }

        public ModInfo[] Mods { get; set; }

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


        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler Loaded;

        private Task<string[]> DiscoverFolders()
        {
            return Task.Run(() =>
            {
                LoadStatus = Resources.Configuration.DiscoveringLocations;
                var sd = new SteamPathDiscoverer();
                return new[] {sd.GetBasePath()}.Concat(sd.GetModsPaths()).ToArray();
            });
        }

        private async Task<ModInfo[]> GetMods(string[] modFolders)
        {
            LoadStatus = Resources.Configuration.DeterminngEnabledMods;
            var allItems = await Task.WhenAll(modFolders.Select(folder => Task.Run(() =>
            {
                if (File.Exists(Path.Combine(folder, "mod-list.json")))
                    return ((JArray) ((JObject) JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(folder, "mod-list.json"))))["mods"])
                        .Select(x => new ModInfo {Name = x["name"].Value<string>(), Enabled = x["enabled"].Value<bool>()});
                return new ModInfo[0];
            })).ToArray());
            return new[] {new ModInfo {Name = "core", Enabled = true}}.Concat(allItems.SelectMany(x => x)).ToArray();
        }

        private async Task<JObject> GetModSettings(string[] modFolders)
        {
            LoadStatus = Resources.Configuration.ReadingModSettings;
            var allItems = await Task.WhenAll(modFolders.Select(folder => Task.Run(() =>
            {
                if (File.Exists(Path.Combine(folder, "mod-settings.json")))
                    return (JObject) JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(folder, "mod-settings.json")));
                return new JObject();
            })).ToArray());
            return allItems.Aggregate(new JObject(), (acc, obj) =>
            {
                acc.Merge(obj);
                return acc;
            });
        }

        public async Task<Bitmap> GetIcon(IWithIcon iconSource)
        {
            if (iconSource.Icon == null && iconSource.Icons == null)
                iconSource = iconSource.FallbackIcon;
            if (iconSource == null) return new Bitmap(1, 1);
            var cacheKey = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {icon = iconSource.Icon, icon_size = iconSource.IconSize, icons = iconSource.Icons})))
                .Aggregate(new StringBuilder(), (builder, b) => builder.Append(b.ToString("x2"))).ToString();
            var cacheFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BluePrintAssembler", "IconCache", cacheKey + ".png");
            if (File.Exists(cacheFile))
                return await Task.Run(() => new Bitmap(cacheFile));
            var newIcon = await MakeIcon(iconSource);
            if (!Directory.Exists(Path.GetDirectoryName(cacheFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(cacheFile));
            newIcon.Save(cacheFile, ImageFormat.Png);
            return newIcon;
        }

        private MultiDisposeObject<Stream> GetStreamFromMod(string path)
        {
            var parts = path.Split(new[] {'\\', '/'}, 2);
            if (parts[0].StartsWith("__") && parts[0].EndsWith("__"))
            {
                var mod = Mods.FirstOrDefault(x => x.Name == parts[0].Trim('_'));
                if (mod == null) return null;
                if (File.Exists(mod.BasePath) && mod.BasePath.EndsWith(".zip"))
                {
                    var file = File.OpenRead(mod.BasePath);
                    var za = new ZipArchive(file);
                    var ent = za.GetEntry($"{mod.Name}_{mod.Version.ToString(3)}/{parts[1]}");
                    return new MultiDisposeObject<Stream>(ent?.Open(), za, file);
                }

                var physicalPath = Path.Combine(mod.BasePath, parts[1]);
                if (File.Exists(physicalPath))
                    return new MultiDisposeObject<Stream>(File.OpenRead(physicalPath));
                return null;
            }

            return null;
        }

        private Task<Bitmap> MakeIcon(IWithIcon iconSource)
        {
            return Task.Run(async () =>
            {
                var src = !string.IsNullOrEmpty(iconSource.Icon)
                    ? new Dictionary<string, IconPart> {{"1", new IconPart {Icon = iconSource.Icon}}}
                    : iconSource.Icons;
                if (src == null) return new Bitmap(1, 1);
                var sz = (int) Math.Min(64, Math.Max(32, iconSource.IconSize));
                var bmp = new Bitmap(sz, sz);

                using (var dc = Graphics.FromImage(bmp))
                {
                    foreach (var layer in src.OrderBy(x => int.Parse(x.Key)))
                        using (var iconStream = GetStreamFromMod(layer.Value.Icon))
                        {
                            if (iconStream == null) continue;
                            using (var layerBmp = Image.FromStream(iconStream.Object))
                            {
                                var dest = new RectangleF(0, 0, sz, sz);
                                if (layer.Value.Scale != 1)
                                {
                                    dest.Height *= layer.Value.Scale;
                                    dest.Width *= layer.Value.Scale;
                                    dest.X += (sz - dest.Width) / 2f;
                                    dest.Y += (sz - dest.Height) / 2f;
                                }

                                if (layer.Value.Shift != null) dest.Offset(layer.Value.Shift.X, layer.Value.Shift.Y);

                                var attr = new ImageAttributes();
                                if (layer.Value.Tint != null)
                                {
                                    var cm = new ColorMatrix(new[]
                                    {
                                        new[] {layer.Value.Tint.R, 0, 0, 0, 0},
                                        new[] {0, layer.Value.Tint.G, 0, 0, 0},
                                        new[] {0, 0, layer.Value.Tint.B, 0, 0},
                                        new[] {0, 0, 0, 1f - layer.Value.Tint.A, 0},
                                        new[] {0, 0, 0, 0, 1f}
                                    });
                                    attr.SetColorMatrix(cm);
                                }

                                dc.DrawImage(layerBmp, new[] {dest.Location, new PointF(dest.Right, dest.Top), new PointF(dest.Left, dest.Bottom)}, new RectangleF(0, 0, layerBmp.Width, layerBmp.Height), GraphicsUnit.Pixel, attr);
                            }
                        }

                    //layer.Value.Scale
                }

                return bmp;
            });
        }

        public async Task Load()
        {
            var folders = await DiscoverFolders();
            var mods = await GetMods(folders);
            var modSettings = await GetModSettings(folders);
            LoadStatus = Resources.Configuration.LocatingEnabledMods;
            Task.WaitAll(mods.Where(x => x.Enabled).Select(x => x.MapToFilesystem(folders)).ToArray());
            LoadStatus = Resources.Configuration.GuessingLoadOrder;
            mods = mods.Where(x => x.Enabled).TopologicalSort((a, b) => a.Name != b.Name && (b.Name == "core" || b.Name == "base" && a.Name != "core" || (a.Dependencies?.Any(x => x.TrimStart(' ', '?').StartsWith(b.Name)) ?? false))).ToArray();
            Mods = mods;

            var verStamp = mods.Aggregate(new StringBuilder("BluePrintAssembler.").Append(Assembly.GetEntryAssembly().GetName().Version), (builder, mod) => builder.Append('|').Append(mod.Name).Append('.').Append(mod.Version?.ToString() ?? "*"));
            verStamp.Append("|").Append(modSettings.ToString(Formatting.None));
            var verStampHash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(verStamp.ToString()))
                .Aggregate(new StringBuilder(), (builder, b) => builder.Append(b.ToString("x2"))).ToString();
            var cacheFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BluePrintAssembler", verStampHash + ".cache");
            if (File.Exists(cacheFile))
            {
                LoadStatus = Resources.Configuration.LoadingFromCache;
                RawData = JsonConvert.DeserializeObject<RawData>(File.ReadAllText(cacheFile));
            }
            else
            {
                LoadStatus = Resources.Configuration.LoadingFromData;
                var rawData = await LoadMods(modSettings, mods);
                RawData = rawData.ToObject<RawData>();
                if (!Directory.Exists(Path.GetDirectoryName(cacheFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(cacheFile));
                File.WriteAllText(cacheFile + "_raw", JsonConvert.SerializeObject(rawData, Formatting.Indented));
                File.WriteAllText(cacheFile, JsonConvert.SerializeObject(RawData, Formatting.Indented));
            }

            LoadStatus = Resources.Configuration.LoadingCommonIcons;

            StdIconSlot = await GetIcon(new GenericIconSource("__core__/graphics/slot.png", 36));

            LoadStatus = Resources.Configuration.LoadCompleted;

            //await GetIcon(RawData.Recipes["fill-water-light-mud-barrel"]);
            var nn = RawData.LocalisedName(RawData.Items["nitric-oxide-barrel"], null);
            Loaded?.Invoke(this, EventArgs.Empty);
        }

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
                            LoadStatus = string.Format(Resources.Configuration.UnpackingMod, mod.Name);
                            mod.TempPath = mod.LoadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BluePrintAssembler", $"UNPACKED_{mod.Name}_{mod.Version?.ToString() ?? "SRC"}");
                            if (!Directory.Exists(mod.LoadPath)) Directory.CreateDirectory(mod.LoadPath);

                            {
                                using (var zfs = File.OpenRead(mod.BasePath))
                                using (var ar = new ZipArchive(zfs))
                                {
                                    foreach (var entry in ar.Entries)
                                    {
                                        if (!entry.Name.EndsWith(".lua") && !entry.Name.EndsWith("cfg") && entry.Name != "info.json") continue;
                                        var fullPath = Path.Combine(mod.LoadPath, entry.FullName);
                                        if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                                            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                                        if (entry.Length > 0)
                                            using (var fs = entry.Open())
                                            using (var ofs = File.Create(fullPath))
                                            {
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
                            if (File.Exists(Path.Combine(mod.LoadPath, filename)))
                            {
                                LoadStatus = string.Format(message, mod.Name, filename);
                                SetupLuaParser(lua, mods.First(), mod);
                                lua.DoFile(Path.Combine(mod.LoadPath, filename));
                            }
                    }

                    ProcessFileInAllMods("settings.lua", Resources.Configuration.ExecutingLuaFromMod);
                    ProcessFileInAllMods("settings-updates.lua", Resources.Configuration.ExecutingLuaFromMod);
                    ProcessFileInAllMods("settings-final-fixes.lua", Resources.Configuration.ExecutingLuaFromMod);
                    ProcessFileInAllMods("data.lua", Resources.Configuration.ExecutingLuaFromMod);
                    ProcessFileInAllMods("data-updates.lua", Resources.Configuration.ExecutingLuaFromMod);
                    ProcessFileInAllMods("data-final-fixes.lua", Resources.Configuration.ExecutingLuaFromMod);

                    var locale = new JObject();
                    foreach (var mod in mods)
                    {
                        if (!Directory.Exists(Path.Combine(mod.LoadPath, "locale"))) continue;
                        foreach (var localeDir in Directory.GetDirectories(Path.Combine(mod.LoadPath, "locale")))
                        {
                            var localeProp = (JObject) locale.Property(Path.GetFileName(localeDir))?.Value;
                            if (localeProp == null)
                                locale.Add(new JProperty(Path.GetFileName(localeDir), localeProp = new JObject()));
                            foreach (var localeFile in Directory.GetFiles(localeDir, "*.cfg"))
                            {
                                var ini = new Ini(localeFile);
                                ini.Load();
                                foreach (var section in ini.GetSections())
                                {
                                    var sectionProp = (JObject) localeProp.Property(section)?.Value;
                                    if (sectionProp == null)
                                        localeProp.Add(new JProperty(section, sectionProp = new JObject()));
                                    foreach (var k in ini.GetKeys(section))
                                        sectionProp[k] = new JValue(ini.GetValue(k, section));
                                }

                                foreach (var rootKey in ini.GetKeys(""))
                                    localeProp[rootKey] = new JValue(ini.GetValue(rootKey));
                            }
                        }
                    }

                    LoadStatus = Resources.Configuration.ConvertingData;

                    JObject Convert(LuaTable table)
                    {
                        var rv = new JObject();
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
                    LoadStatus = Resources.Configuration.RemovingTempFiles;
                    foreach (var mod in mods)
                        if (mod.TempPath != null && mod.TempPath != mod.BasePath)
                            Directory.Delete(mod.TempPath, true);

                    rootObject.Add("Locale", locale);
                    return rootObject;
                }
            });
        }

        private void SetupLuaParser(Lua lua, ModInfo core, ModInfo mod)
        {
            //Initialize package.path
            lua.GetFunction("___BPA___pkgpath__set___").Call(mod.LoadPath);
            lua.GetFunction("___BPA___pkgpath__add___").Call(Path.Combine(core.BasePath, "lualib"));
            //Some mods (e.g. Angel's Refining) are doing imports with relative paths from within prototypes/ folder
            lua.GetFunction("___BPA___pkgpath__add___").Call(Path.Combine(mod.LoadPath, "prototypes"));
            //"Unload" all previously loaded packages
            lua.DoString("for name, version in pairs(package.loaded) do package.loaded[name]=false end");
        }

        private void InitLuaParser(Lua lua, ModInfo core, IEnumerable<ModInfo> enabledMods, JObject modSettings)
        {
            using (var reader = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("BluePrintAssembler.Resources.Lua.Init.lua")))
            {
                lua.DoString(reader.ReadToEnd());
            }

            lua.GetFunction("___BPA___pkgpath__add___").Call(Path.Combine(core.BasePath, "lualib"));
            using (var reader = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("BluePrintAssembler.Resources.Lua.FactorioDefs.lua")))
            {
                lua.DoString(reader.ReadToEnd());
            }

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal class MultiDisposeObject<T> : IDisposable where T : IDisposable
        {
            private readonly IDisposable[] _others;

            public MultiDisposeObject(T obj, params IDisposable[] others)
            {
                _others = others;
                Object = obj;
            }

            public T Object { get; }

            public void Dispose()
            {
                Object.Dispose();
                foreach (var o in _others)
                    o.Dispose();
            }
        }

        private class GenericIconSource : IWithIcon
        {
            public GenericIconSource(string file, float size)
            {
                Icon = file;
                IconSize = size;
            }

            public string Icon { get; }
            public float IconSize { get; }
            public Dictionary<string, IconPart> Icons { get; set; }

            public IWithIcon FallbackIcon => null;
        }
    }
}