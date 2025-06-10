using CliFx;

namespace MANIFOLD;

class Program {
    static async Task<int> Main() {
        return await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .Build()
            .RunAsync();
    }
}
