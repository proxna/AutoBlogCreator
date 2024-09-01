using AutoBlogCreator.Models;
using LibGit2Sharp;

namespace AutoBlogCreator.Services
{
    public class GitConnector : IGitConnector
    {
        private readonly IConfiguration _configuration;

        public GitConnector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void AddFileToRepository(string path)
        {
            RepositoryInfo repositoryInfo = new RepositoryInfo();
            _configuration.GetSection("RepositoryInfo").Bind(repositoryInfo);
            string fileName = Path.GetFileName(path);
            StageFile(path, repositoryInfo);
            Commit(fileName, repositoryInfo);
            Push(repositoryInfo);
        }

        private void Push(RepositoryInfo repositoryInfo)
        {
            using var repo = new Repository(repositoryInfo.LocalPath);
            var options = new PushOptions
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                {
                    Username = repositoryInfo.UserName,
                    Password = repositoryInfo.Password
                }
            };
            repo.Network.Push(repo.Branches["main"], options);
        }

        public void StageFile(string path, RepositoryInfo repositoryInfo)
        {
            using var repo = new Repository(repositoryInfo.LocalPath);
            repo.Index.Add(path);
            repo.Index.Write();
        }

        public void Commit(string filename, RepositoryInfo repositoryInfo)
        {
            using var repo = new Repository(repositoryInfo.LocalPath);
            var author = new Signature(repositoryInfo.UserName, repositoryInfo.UserEmail, DateTimeOffset.Now);
            var committer = author;
            string message = $"Add new article {filename}";
            repo.Commit(message, author, committer);
        }

        public void AddFileWithoutCommit(string path)
        {
            RepositoryInfo repositoryInfo = new RepositoryInfo();
            _configuration.GetSection("RepositoryInfo").Bind(repositoryInfo);
            string fileName = Path.GetFileName(path);
            StageFile(path, repositoryInfo);
        }
    }
}