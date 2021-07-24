using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Facebook
{
    internal static class ScriptExecutor
    {
        public static async IAsyncEnumerable<string> Execute(
            string executablePath,
            string scriptName,
            object request,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            string script = await GetScript(scriptName);

            Process process = StartProcess(executablePath, script, GetRequestJson(request), ct);

            const string blockStart = "{";
            const string blockEnd = "}";

            await foreach (string block in GetOutputJsonBlocks(process, blockStart, blockEnd, ct))
            {
                yield return block;
            }
        }

        private static async Task<string> GetScript(string scriptName)
        {
            Type type = typeof(ScriptExecutor);

            await using Stream scriptStream = type.Assembly
                .GetManifestResourceStream($"{type.Namespace}.{scriptName}");
            
            using var streamReader = new StreamReader(scriptStream);
            
            return await streamReader.ReadToEndAsync();
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
            string script,
            string parameters,
            CancellationToken token)
        {
            string[] arguments = {
                "-u", // python -u forces the stdout and stderr streams to be unbuffered, without this option the output will only be received when process has exited
                "-c", // python -c is required when running a script as argument
                $"\"{script}\"",
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
                RedirectStandardError = false
            };
        }
    }
}