using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public static class ScriptExecutor
    {
        public static Task<string> ExecutePython(
            string fileName,
            CancellationToken token = default,
            params object[] parameters)
        {
            return Execute("python3", fileName, token, parameters);
        }
        
        public static async Task<string> Execute(
            string command,
            string fileName,
            CancellationToken token = default,
            params object[] parameters)
        {
            var arguments = new[] { fileName }
                .Concat(
                    parameters
                        .Where(o => o != null)
                        .Select(o => o.ToString()));
            
            ProcessStartInfo startInfo = CreateProcessStartInfo(
                command,
                arguments);
            
            using Process process = Process.Start(startInfo);

            token.Register(() => process.Kill());

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync(token);
            
            if (string.IsNullOrEmpty(output))
            {
                throw new InvalidOperationException($"Failed to execute script (no output) {error}");
            }

            return output;
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