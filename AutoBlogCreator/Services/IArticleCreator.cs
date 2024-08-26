using AutoBlogCreator.Models;

namespace AutoBlogCreator.Services
{
    public interface IArticleCreator
    {
        Task CreateArticle(string content);

        Task CreateArticle(News news);
    }
}
