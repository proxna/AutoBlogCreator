using AutoBlogCreator;

namespace AutoBlogCreator.UnitTests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("![NBA 2K25](https://images.igdb.com/igdb/image/upload/t_1080p/ar30rv.jpg)", "https://images.igdb.com/igdb/image/upload/t_1080p/ar30rv.jpg")]
        [InlineData("https://images.igdb.com/igdb/image/upload/t_1080p/ar1mlw.jpg", "https://images.igdb.com/igdb/image/upload/t_1080p/ar1mlw.jpg")]
        public void ExtractUrlFromString_WithProperParameters_ReturnsUrl(string input, string expected)
        {
            string output = input.ExtractUrlFromString();
            Assert.Equal(expected, output);
        }
    }
}