using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scraper.Net.Tests
{
    [TestClass]
    public class MediaItemConverterTests
    {
        [TestMethod]
        public void SerializeDeserializePhotoItem()
        {
            var original = new PhotoItem("my-url");

            string json = JsonSerializer.Serialize(original);

            var deserialized = JsonSerializer.Deserialize<PhotoItem>(json);
            
            Assert.AreEqual(original, deserialized);
        }
        
        [TestMethod]
        public void SerializeDeserializeVideoItem()
        {
            var original = new VideoItem(
                "my-url",
                UrlType.DirectUrl,
                "thumbnail-url",
                TimeSpan.FromMinutes(1),
                1920,
                1080);

            string json = JsonSerializer.Serialize(original);

            var deserialized = JsonSerializer.Deserialize<VideoItem>(json);
            
            Assert.AreEqual(original, deserialized);
        }
    }
}