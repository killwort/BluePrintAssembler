using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BluePrintAssembler.Annotations;
using BluePrintAssembler.Steam;
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
            var verStamp = mods.Where(x => x.Enabled)
                .Aggregate(
                    new StringBuilder("BluePrintAssembler.").Append(Assembly.GetEntryAssembly().GetName().Version),
                    (builder, mod) => builder.Append('|').Append(mod.Name).Append('.')
                        .Append(mod.Version?.ToString() ?? "*"));
            verStamp.Append("|").Append(modSettings.ToString(Formatting.None));
            var verStampHash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(verStamp.ToString()))
                .Aggregate(new StringBuilder(), (builder, b) => builder.Append(b.ToString("x2"))).ToString();
            var cacheFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BluePrintAssembler", verStampHash + ".cache");
            if (File.Exists(cacheFile))
            {
                LoadStatus = "Loading data from cache…";
            }
            else
            {
                LoadStatus = "Loading mods…";
                await LoadMods(modSettings, mods.Where(x => x.Enabled));
                //Directory.CreateDirectory(Path.GetDirectoryName(cacheFile));
                //File.WriteAllText(cacheFile,verStamp.ToString());
            }
        }

        private Task LoadMods(JObject modSettings, IEnumerable<ModInfo> mods)
        {
            return Task.Run(() =>
            {
                using (var lua = new Lua())
                {
                    InitLuaParser(modSettings);
                    foreach (var mod in mods.Where(x => x.Enabled))
                    {
                        LoadStatus = $"Loading mod {mod.Name}…";
                        var loadPath = mod.BasePath;
                        if (File.Exists(loadPath))
                        {
                            LoadStatus = $"Loading mod {mod.Name}… Unpacking…";
                            loadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"BluePrintAssembler", $"UNPACKED_{mod.Name}_{mod.Version?.ToString() ?? "SRC"}");
                            if (!Directory.Exists(loadPath))
                                Directory.CreateDirectory(loadPath);
                            using (var zfs = File.OpenRead(mod.BasePath))
                            using (var ar = new ZipArchive(zfs))
                            {
                                foreach (var entry in ar.Entries)
                                {
                                    var fullPath = Path.Combine(loadPath, entry.FullName);
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
                        LoadStatus = $"Loading mod {mod.Name}… Executing…";
                        SetupLuaParser(mod,loadPath);

                        if (loadPath != mod.BasePath)
                            Directory.Delete(loadPath, true);
                    }
                }
            });
        }

        private void SetupLuaParser(ModInfo mod, string loadPath)
        {
            
        }

        private void InitLuaParser(JObject modSettings)
        {
            
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
    }

    public class ModInfo
    {
        public string Name;
        public Version Version;
        public string BasePath;
        public bool Enabled;

        public Task MapToFilesystem(string[] folders)
        {
            return Task.Run(() =>
            {
                foreach (var folder in folders.Reverse())
                {
                    if (Directory.Exists(Path.Combine(folder, Name)))
                    {
                        BasePath = Path.Combine(folder, Name);
                        if (File.Exists(Path.Combine(BasePath, "info.json")))
                            using (var fs = File.OpenRead(Path.Combine(BasePath, "info.json")))
                                LoadVersionFromInfoJson(fs);
                        break;
                    }

                    var versionedDir = Directory.GetDirectories(folder, $"{Name}_*.*.*")
                        .Select(x => Tuple.Create(x, Version.TryParse(Path.GetFileName(x).Substring(Name.Length + 1), out var ver), ver))
                        .Where(x => x.Item2)
                        .OrderByDescending(x => x.Item3)
                        .FirstOrDefault();

                    if (versionedDir != null)
                    {
                        BasePath = versionedDir.Item1;
                        Version = versionedDir.Item3;
                        break;
                    }

                    if (File.Exists(Path.Combine(folder, $"{Name}.zip")))
                    {
                        BasePath = Path.Combine(folder, Name);
                        using (var zfs = File.OpenRead(BasePath))
                        using (var zfile = new ZipArchive(zfs, ZipArchiveMode.Read))
                        {
                            var zEntry = zfile.Entries.FirstOrDefault(x => x.Name == "info.json");
                            if (zEntry != null)
                                using (var fs = zEntry.Open())
                                {
                                    LoadVersionFromInfoJson(fs);
                                }
                        }

                        break;
                    }

                    var versionedFile = Directory.GetFiles(folder, $"{Name}_*.*.*.zip")
                        .Select(x => Tuple.Create(x, Version.TryParse(Path.GetFileNameWithoutExtension(x).Substring(Name.Length + 1), out var ver), ver))
                        .Where(x => x.Item2)
                        .OrderByDescending(x => x.Item3)
                        .FirstOrDefault();

                    if (versionedFile != null)
                    {
                        BasePath = versionedFile.Item1;
                        Version = versionedFile.Item3;
                        break;
                    }
                }
            });
        }

        private void LoadVersionFromInfoJson(Stream fs)
        {
            using (var reader = new StreamReader(fs))
            {
                var verString = ((JObject) JsonConvert.DeserializeObject(reader.ReadToEnd()))["version"]?.Value<string>();
                if (verString != null && Version.TryParse(verString, out var ver)) Version = ver;
            }
        }
    }
}