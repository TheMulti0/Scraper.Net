using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Facebook
{
    internal static class ScriptExecutor
    {
        public static IAsyncEnumerable<string> Execute(
            string executablePath,
            string scriptName,
            object request,
            CancellationToken ct = default)
        {
            Process process = StartProcess(executablePath, scriptName, GetRequestJson(request), ct);

            const string blockStart = "{";
            const string blockEnd = "}";

            return GetOutputJsonBlocks(process, blockStart, blockEnd, ct);
        }

        private static async IAsyncEnumerable<string> GetOutputJsonBlocks(
            Process process,
            string blockStart,
            string blockEnd,
            [EnumeratorCancellation] CancellationToken ct)
        {
            IAsyncEnumerable<string> standardOutput = process.StandardOutput()
                .SkipWhile(l => l != blockStart);

            var block = string.Empty;

            await foreach (string line in standardOutput.WithCancellation(ct))
            {
                block += line + "\n";

                if (line != blockEnd)
                {
                    continue;
                }

                yield return block;

                block = string.Empty;
            }
        }

        private static string GetRequestJson(object request)
        {
            return JsonSerializer.Serialize(request).Replace("\"", "\\\"");
        }

        private static Process StartProcess(
            string executablePath,
            string scriptName,
            string parameters,
            CancellationToken token)
        {
            string[] arguments = {
                scriptName,
                parameters
            };

            ProcessStartInfo startInfo = CreateProcessStartInfo(executablePath, arguments);
            
            Process process = Process.Start(startInfo);
            
            token.Register(() => process?.Kill());
            
            return process;
        }

        private static ProcessStartInfo CreateProcessStartInfo(
            string command,
            IEnumerable<string> args)
        {
            return new()
            {
                FileName = command,
                Arguments = string.Join(' ', args),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
        }
    }
}