using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Scraper.Net.Facebook
{
    internal class ScriptExecutor
    {
        private readonly Dictionary<string, string> _scripts = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly ILogger<ScriptExecutor> _logger;

        public ScriptExecutor(ILogger<ScriptExecutor> logger)
        {
            _logger = logger;
        }
        
        public async IAsyncEnumerable<string> ExecuteAsync<TRequest>(
            string executablePath,
            string scriptName,
            TRequest request,
            [EnumeratorCancellation] CancellationToken ct = default) where TRequest : Request
        {
            string script = await GetScriptAsync(scriptName, ct);

            Process process = StartProcess(executablePath, script, GetRequestJson(request), ct);
            process.StandardError().Subscribe(Log(scriptName, request.UserId), ct);

            const string blockStart = "{";
            const string blockEnd = "}";

            await foreach (string block in GetOutputJsonBlocks(process, blockStart, blockEnd, ct))
            {
                yield return block;
            }
        }

        private async Task<string> GetScriptAsync(string scriptName, CancellationToken ct)
        {
            await _semaphore.WaitAsync(ct);

            try
            {
                if (_scripts.ContainsKey(scriptName))
                {
                    return _scripts[scriptName];
                }
            
                string script = await GetScriptFromResourcesAsync(scriptName);
            
                _scripts.Add(scriptName, script);
            
                return script;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private static async Task<string> GetScriptFromResourcesAsync(string scriptName)
        {
            Type type = typeof(ScriptExecutor);

            await using Stream scriptStream = type.Assembly
                .GetManifestResourceStream($"{type.Namespace}.{scriptName}");

            using var streamReader = new StreamReader(scriptStream);

            string script = await streamReader.ReadToEndAsync();
            
            return script.EscapeQuotes();
        }

        private static string GetRequestJson(object request)
        {
            return JsonSerializer.Serialize(request)
                .EscapeQuotes();
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
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };
        }

        private Action<string> Log(string scriptName, string category)
        {
            return line =>
            {
                string c = category == null 
                    ? string.Empty 
                    : $"[{category}]";
                _logger.LogDebug("[{}]: {} {}", scriptName, c, line);
            };
        }

        private static async IAsyncEnumerable<string> GetOutputJsonBlocks(
            Process process,
            string blockStart,
            string blockEnd,
            [EnumeratorCancellation] CancellationToken ct)
        {
            IAsyncEnumerable<string> standardOutput = process
                .StandardOutput()
                .ToAsyncEnumerable()
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
    }
}