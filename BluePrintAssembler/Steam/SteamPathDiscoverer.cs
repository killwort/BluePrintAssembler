using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace BluePrintAssembler.Steam
{
    class SteamPathDiscoverer
    {
        public string GetBasePath()
        {
            var xfolders = EnumerateSteamLibraryFolders().ToArray();
            var baseFolder = xfolders.First(x => File.Exists(Path.Combine(x, "Factorio", "bin", "x64", "factorio.exe")));
            return Path.Combine(baseFolder, "Factorio", "data");
        }

        private IEnumerable<string> EnumerateSteamLibraryFolders()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam", RegistryRights.QueryValues | RegistryRights.ReadKey))
            {
                var steamPath = (string) key.GetValue("SteamPath");
                yield return Path.Combine(steamPath, "steamapps", "common");
                var parser = new Parser(new Scanner(Path.Combine(steamPath, "steamapps", "libraryfolders.vdf")));
                parser.errors.errorStream=new StringWriter();
                parser.Parse();
                if(parser.Name!="\"LibraryFolders\"")throw new FormatException("LibraryFolders.vdf does not contain LibraryFolders object.");
                foreach (var prop in parser.Result.Values)
                {
                    if (int.TryParse(prop.Item1.Substring(1, prop.Item1.Length - 2), out var _))
                        yield return Path.Combine(prop.Item2.UnescapedValue, "steamapps", "common");
                }
            }
        }

        public IEnumerable<string> GetModsPaths()
        {
            var baseFolder = EnumerateSteamLibraryFolders().First(x => File.Exists(Path.Combine(x, "Factorio", "bin", "x64", "factorio.exe")));
            if(Directory.Exists(Path.Combine(baseFolder, "Factorio", "mods")))
                yield return Path.Combine(baseFolder, "Factorio", "mods");
            baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if(Directory.Exists(Path.Combine(baseFolder, "Factorio", "mods")))
                yield return Path.Combine(baseFolder, "Factorio", "mods");
            baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if(Directory.Exists(Path.Combine(baseFolder, "Factorio", "mods")))
                yield return Path.Combine(baseFolder, "Factorio", "mods");
        }
    }
}
