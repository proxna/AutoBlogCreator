namespace AutoBlogCreator.Services
{
    public interface IArticleAdjuster
    {
        Task<string> AdjustArticle(string article);
    }
}
