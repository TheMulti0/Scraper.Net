using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
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
            CancellationToken token = default)
        {
            Process process = StartProcess(executablePath, scriptName, GetRequestJson(request), token);

            IObservable<string> standardOutput = process.StandardOutput();

            const string startOfPost = "{";
            const string endOfPost = "}";
            
            return standardOutput
                .SkipWhile(s => s != startOfPost)
                .TakeUntil(s => s == endOfPost)
                .Aggregate(string.Empty, (lhs, rhs) => lhs + "\n" + rhs)
                .Retry()
                .Where(s => s.EndsWith(endOfPost))
                .ToAsyncEnumerable();
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
            
            token.Register(() => process.Kill());
            
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