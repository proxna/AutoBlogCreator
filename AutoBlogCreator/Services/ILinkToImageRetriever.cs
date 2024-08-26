namespace AutoBlogCreator.Services
{
    public interface ILinkToImageRetriever
    {
        public Task<string> GetImageLink(string name);
    }
}
