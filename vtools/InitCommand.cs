using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using MANIFOLD.Utility;
using Microsoft.Win32;

namespace MANIFOLD {
    [Command("init", Description = "Creates a work folder and an settings file.")]
    public class InitCommand : ICommand {
        [CommandOption("work-folder", 'f', IsRequired = false)]
        public string? WorkFolder { get; init; }
        [CommandOption("sbox", IsRequired = false)]
        public string? SboxLocation { get; init; }
        [CommandOption("blender", IsRequired = false)]
        public string? BlenderExecutable { get; set; }
        [CommandParameter(0)]
        public required string SboxProject { get; init; }
        
        public ValueTask ExecuteAsync(IConsole console) {
            string workFolder = WorkFolder ?? Environment.CurrentDirectory;
            CommandSettings settings = new CommandSettings();

            string sboxLocation = SboxLocation ?? Path.GetDirectoryName(GetProgramByExtension(".sbproj"));
            if (sboxLocation == null) {
                console.Output.WriteLine("Could not find S&box install");
                return default;
            }
            settings.SBoxLocation = sboxLocation;
            
            string blenderExecutable = BlenderExecutable ?? GetProgramByExtension(".blend");
            if (blenderExecutable == null) {
                console.Output.WriteLine("Could not find blender install");
                return default;
            }
            settings.BlenderExecutable = blenderExecutable;
            settings.SBoxProject = SboxProject;
            
            string json = Json.Serialize(settings);
            File.WriteAllText(Path.Combine(workFolder, "settings.json"), json);
            console.Output.WriteLine("Settings file created");
            
            Directory.CreateDirectory(Path.Combine(workFolder, "tools"));
            console.Output.WriteLine("Tools extracted");
            
            Directory.CreateDirectory(Path.Combine(workFolder, "maps"));
            console.Output.WriteLine("Work folder initialized.");
            return default;
        }

        public string GetProgramByExtension(string ext) {
            using var regKey = Registry.ClassesRoot.OpenSubKey(ext, false);
            if (regKey == null) return null;
            using var anotherRegKey = Registry.ClassesRoot.OpenSubKey(regKey.GetValue(null) + @"\shell\open\command");
            string result = anotherRegKey.GetValue(null).ToString();
            return result.Replace("\"", "").Split(".exe")[0] + ".exe";
        }
        
        public IEnumerable<string> RecommendedPrograms(string ext)
        {
            List<string> progs = new List<string>();

            string baseKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\." + ext;

            using (RegistryKey rk = Registry.ClassesRoot.OpenSubKey(ext, false))
            {
                if (rk != null)
                {
                    string mruList = (string)rk.GetValue("MRUList");
                    if (mruList != null)
                    {
                        foreach (char c in mruList.ToString())
                            progs.Add(rk.GetValue(c.ToString()).ToString());
                    }
                }
            }

            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(baseKey + @"\OpenWithProgids"))
            {
                if (rk != null)
                {
                    foreach (string item in rk.GetValueNames())
                        progs.Add(item);
                }
                //TO DO: Convert ProgID to ProgramName, etc.
            }

            return progs;
        }
    }
}
