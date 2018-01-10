using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BluePrintAssembler.Domain
{
    public class ModInfo
    {
        public string Name;
        public Version Version;
        public string BasePath,LoadPath,TempPath;
        public bool Enabled;

        public string[] Dependencies;

        public Task MapToFilesystem(string[] folders)
        {
            return Task.Run(() =>
            {
                foreach (var folder in folders.Reverse())
                {
                    if (Directory.Exists(Path.Combine(folder, Name)))
                    {
                        BasePath = Path.Combine(folder, Name);
                        using (var fs = File.OpenRead(Path.Combine(BasePath, "info.json")))
                            LoadInfoJson(fs);
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
                        using (var fs = File.OpenRead(Path.Combine(BasePath, "info.json")))
                            LoadInfoJson(fs);
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
                                    LoadInfoJson(fs);
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
                        using (var zfs = File.OpenRead(BasePath))
                        using (var zfile = new ZipArchive(zfs, ZipArchiveMode.Read))
                        {
                            var zEntry = zfile.Entries.FirstOrDefault(x => x.Name == "info.json");
                            if (zEntry != null)
                                using (var fs = zEntry.Open())
                                {
                                    LoadInfoJson(fs);
                                }
                        }
                        break;
                    }
                }
            });
        }

        private void LoadInfoJson(Stream fs)
        {
            using (var reader = new StreamReader(fs))
            {
                var info = ((JObject) JsonConvert.DeserializeObject(reader.ReadToEnd()));
                var verString = info["version"]?.Value<string>();
                if (verString != null && Version.TryParse(verString, out var ver)) Version = ver;
                Dependencies = ((JArray) info["dependencies"])?.Select(x => x.Value<string>()).ToArray();
            }
        }
    }
}