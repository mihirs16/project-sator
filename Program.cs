using System.Diagnostics;
using Nerdbank.Streams;
using StreamJsonRpc;

class Program
{
    static async Task Main()
    {
        var psi = new ProcessStartInfo("pylsp", "-vv");
        psi.RedirectStandardInput = true;
        psi.RedirectStandardOutput = true;
        Process? process = Process.Start(psi);
        if (process != null) {
            var stdioStream = FullDuplexStream.Splice(
                process.StandardOutput.BaseStream, 
                process.StandardInput.BaseStream
            );
            await ActAsRpcClientAsync(stdioStream);
        }
    }

    private static async Task ActAsRpcClientAsync(Stream stream)
    {
        Console.WriteLine("Connected. Sending request...");
        using var jsonRpc = JsonRpc.Attach(stream);
        int sum = await jsonRpc.InvokeAsync<int>("initialize");
        Console.WriteLine($"3 + 5 = {sum}");
    }
}