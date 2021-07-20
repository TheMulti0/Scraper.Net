using System;

namespace Scraper.Net
{
    public class IdNotFoundException : Exception
    {
        public IdNotFoundException(string id) : base($"{id} could not be found")
        {
            
        }
        
        public IdNotFoundException(string id, Exception e) : base($"{id} could not be found", e)
        {
            
        }
    }
}