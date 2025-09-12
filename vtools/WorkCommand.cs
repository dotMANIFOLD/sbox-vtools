using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using MANIFOLD.Utility;

namespace MANIFOLD {
    public abstract class WorkCommand : ICommand {
        [CommandOption("work-folder", 'w', IsRequired = false)]
        public string? WorkFolder { get; set; }
        
        public CommandSettings Settings { get; private set; }
        
        public ValueTask ExecuteAsync(IConsole console) {
            if (!Setup(console)) return default;
            return Run(console);
        }

        protected virtual bool Setup(IConsole console) {
            WorkFolder ??= Environment.CurrentDirectory;
            
            console.Output.WriteLine($"Work Folder: {WorkFolder}");
            
            string settingsPath = Path.Combine(WorkFolder, "settings.json");
            if (!File.Exists(settingsPath)) {
                console.Output.WriteLine("No settings file found. Please initialize the folder.");
                return false;
            }
            
            Settings = Json.Deserialize<CommandSettings>(File.ReadAllText(settingsPath));
            return true;
        }
        
        protected abstract ValueTask Run(IConsole console);
    }
}
