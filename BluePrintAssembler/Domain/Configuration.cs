using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BluePrintAssembler.Annotations;
using BluePrintAssembler.Steam;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BluePrintAssembler.Domain
{
    class Configuration:INotifyPropertyChanged
    {
        private Configuration(){}
        private static Configuration _instance;
        private string _loadStatus;
        public static Configuration Instance => _instance ?? (_instance = new Configuration());
        public Task<Tuple<string[],string[]>> DiscoverFolders()
        {
            return Task.Run(() =>
            {
                LoadStatus = "Discovering game/mods location…";
                var sd=new SteamPathDiscoverer();
                var basePaths=sd.GetBasePath().ToArray();
                var modsPaths = sd.GetModsPaths().ToArray();
                LoadStatus = $"Discovering game/mods location… {basePaths.First()}";
                return Tuple.Create(basePaths, modsPaths);
            });
        }

        public async Task<IEnumerable<string>> GetEnabledMods(string[] modFolders)
        {
            var allItems = await Task.WhenAll(modFolders.Select(folder => Task.Run(() =>
            {
                if (File.Exists(Path.Combine(folder, "mod-list.json")))
                    return ((JArray) ((JObject) JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(folder, "mod-list.json"))))["mods"])
                        .Where(x => x["enabled"].Value<bool>())
                        .Select(x => x["name"].Value<string>());
                else
                    return new string[0];
            })).ToArray());
            return allItems.SelectMany(x => x);
        }

        /*public Task<string[]> DiscoverMods(string[] modPaths)
        {

        }*/
        
        public async Task Load()
        {
            var folders = await DiscoverFolders();
            var enabledMods=await GetEnabledMods(folders.Item2);
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
}
