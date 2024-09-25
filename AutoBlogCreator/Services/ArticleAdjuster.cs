using System.Text.RegularExpressions;

namespace AutoBlogCreator.Services
{
    public class ArticleAdjuster : IArticleAdjuster
    {
        public async Task<string> AdjustArticle(string article)
        {
            foreach (var youtubeVideoLink in GetYoutubeLinks(article))
            {
                bool isValid = await IsValidYoutubeId(youtubeVideoLink.Groups[1].Value);
                if (isValid)
                {
                    string embedHtml = $"<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/{youtubeVideoLink.Groups[1].Value}\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>";
                    article = article.Replace(youtubeVideoLink.Value, embedHtml);
                }
                else
                {
                    article = article.Replace(youtubeVideoLink.Value, string.Empty);
                }
            }

            foreach (var tweetLink in GetTweetLinks(article))
            {
                bool isValid = IsValidTwitterLink(tweetLink);
                if (isValid)
                {
                    string urlEncodedTweetLink = System.Web.HttpUtility.UrlEncode(tweetLink.Replace("x.com", "twitter.com"));
                    string embedHtml = $"<iframe border=0 frameborder=0 height=250 width=550\r\n src=\"https://twitframe.com/show?url={urlEncodedTweetLink}\"></iframe>";
                    article = article.Replace(tweetLink, embedHtml);
                }
                else
                {
                    article = article.Replace(tweetLink, string.Empty);
                }
            }

            article = Regex.Replace(article, @"(?:https?:\/\/)?(?:www\.)?(?:\S+\.)*x\.com\S*", string.Empty);

            return article;
        }

        private async Task<bool> IsValidYoutubeId(string id)
        {
            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"http://img.youtube.com/vi/{id}/mqdefault.jpg");
            return response.IsSuccessStatusCode;
        }

        private bool IsValidTwitterLink(string link)
        {
            if (string.IsNullOrWhiteSpace(link)) return false;
            string tweetId = link.Split('/').Last();
            if (string.IsNullOrWhiteSpace(tweetId)) return false;
            if (!int.TryParse(tweetId.Substring(0, 2), out int idPrefix)) return false;
            return idPrefix >= 18;
        }

        private IEnumerable<Match> GetYoutubeLinks(string text)
        {
            // Regular expression pattern to match YouTube links
            string pattern = @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com|youtu\.be)\/(?:watch\?v=|embed\/|v\/)?([^\s&]+)";

            // Match YouTube links in the text
            MatchCollection matches = Regex.Matches(text, pattern);

            // Add the matched links to the list
            foreach (Match match in matches)
            {
                yield return match;
            }
        }

        public IEnumerable<string> GetTweetLinks(string text)
        {
            // Regular expression pattern to match tweet links
            string pattern = @"(?:https?:\/\/)(?:www\.)?(twitter|x)\.com\/.+";

            // Match tweet links in the text
            MatchCollection matches = Regex.Matches(text, pattern);

            // Add the matched links to the list
            foreach (Match match in matches)
            {
                yield return match.Value;
            }
        }
    }
}
