﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Net.Facebook
{
    internal class PageInfoScraper
    {
        private const string ScriptName = "get_page_info.py";

        private readonly ScriptExecutor _executor;
        private readonly FacebookConfig _config;

        public PageInfoScraper(
            ScriptExecutor executor,
            FacebookConfig config)
        {
            _executor = executor;
            _config = config;
        }

        public async Task<PageInfo> GetPageInfoAsync(
            string id,
            string proxy,
            CancellationToken ct)
        {
            GetPageInfoRequest request = CreateGetPageInfoRequest(id, proxy);

            string pageInfoJson = await GetPageInfoJson(request, ct).FirstOrDefaultAsync(ct);
            
            if (pageInfoJson == null)
            {
                ct.ThrowIfCancellationRequested();
            }

            return Deserialize(pageInfoJson, request);
        }

        private static PageInfo Deserialize(string json, GetPageInfoRequest request)
        {
            try
            {
                var post = JsonSerializer.Deserialize<PageInfo>(json);

                if (post?.Url == null)
                {
                    throw new JsonException();
                }
                
                return post;
            }
            catch (JsonException)
            {
                var exception = JsonSerializer.Deserialize<FacebookScraperException>(json);
                
                if (exception?.Type == null)
                {
                    throw;
                }
                
                ExceptionHandler.HandleException(request.UserId, request.Proxy, exception);
                throw; // HandleException should throw exception
            }
        }

        private IAsyncEnumerable<string> GetPageInfoJson(
            GetPageInfoRequest request,
            CancellationToken ct)
        {
            return _executor.ExecuteAsync(
                _config.PythonPath,
                ScriptName,
                request,
                ct);
        }

        private GetPageInfoRequest CreateGetPageInfoRequest(string id, string proxy)
        {
            return new()
            {
                UserId = id,
                Proxy = proxy,
                Timeout = _config.Timeout,
                CookiesFileNames = _config.CookiesFileNames
            };
        }
    }
}