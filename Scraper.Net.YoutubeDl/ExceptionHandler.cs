using System;
using System.Threading.Tasks;

namespace Scraper.Net.YoutubeDl
{
    internal static class ExceptionHandler
    {
        public static async Task<T> Do<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (YoutubeDlException e)
            {
                if (
                    e.Message.Contains(
                        "This video is only available for registered users. Use --username and -*** --netrc to provide account credentials."))
                {
                    throw new LoginRequiredException(e);
                }
                throw;
            }
        }
    }
}