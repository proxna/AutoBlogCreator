namespace AutoBlogCreator.Services
{
    public interface IGitConnector
    {
        void AddFileToRepository(string path);
    }
}
