using AutoBlogCreator.Models;
using System.Text;

namespace AutoBlogCreator.Services
{
    public class ArticleCreator : IArticleCreator
    {
        private readonly IGitConnector _gitConnector;
        private readonly IConfiguration _configuration;

        public ArticleCreator(IGitConnector gitConnector, IConfiguration configuration)
        {
            _gitConnector = gitConnector;
            _configuration = configuration;
        }

        public async Task CreateArticle(string content)
        {
            string directoryName = GetTitle(content);
            string localPath = _configuration.GetLocalPath();
            string pathInRepository = Path.Combine("content", "posts", directoryName);
            string pathInFileRepository = Path.Combine(pathInRepository, "index.md");
            Directory.CreateDirectory(Path.Combine(localPath, pathInRepository));
            string pathToFile = Path.Combine(localPath, pathInFileRepository);
            await File.WriteAllTextAsync(pathToFile, content);
            _gitConnector.AddFileToRepository(pathInFileRepository);
        }

        public async Task CreateArticle(News news)
        {
            string directoryName = GetTitle(news.Text);
            string localPath = _configuration.GetLocalPath();
            string pathInRepository = Path.Combine("content", "posts", directoryName); ;
            Directory.CreateDirectory(Path.Combine(localPath, pathInRepository));
            string imageExtension = await SaveImageFromUrl(news.ImageUrl, pathInRepository);
            string pathInFileRepository = Path.Combine(pathInRepository, "index.md");
            string pathToFile = Path.Combine(localPath, pathInFileRepository);
            string fullArticle = GetArticleHeader(news, imageExtension) + news.Text;
            await File.WriteAllTextAsync(pathToFile, fullArticle);
            _gitConnector.AddFileToRepository(pathInFileRepository);
        }

        private string GetArticleHeader(News news, string imageExtension)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("---");
            stringBuilder.AppendLine("title: " + news.Text.Split('\n').First().Substring(2));
            stringBuilder.AppendLine("date: " + DateTime.Now.ToString("yyyy-MM-dd"));
            stringBuilder.AppendLine("tags: " + news.Tags);
            stringBuilder.AppendLine("image: image" + imageExtension);
            stringBuilder.AppendLine("---");
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        private async Task<string> SaveImageFromUrl(string imageUrl, string pathToRepo)
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(imageUrl);
            if (response.IsSuccessStatusCode)
            {
                string extension = response.Content.Headers.ContentType.MediaType switch
                {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    "image/gif" => ".gif",
                    "image/webp" => ".webp",
                    _ => throw new NotSupportedException("Unsupported image format.")
                };
                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                string pathInRepository = Path.Combine(pathToRepo, "image" + extension);
                string pathToImage = Path.Combine(_configuration.GetLocalPath(), pathInRepository);
                await File.WriteAllBytesAsync(pathToImage, imageBytes);
                _gitConnector.AddFileWithoutCommit(pathInRepository);
                return extension;
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetTitle(string content)
        {
            var firstSentence = content.Split('\n').First().Substring(2);
            return firstSentence.RemoveWhitespaces().RemovePolishCharacters();
        }
    }
}
