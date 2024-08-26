using System.Text.RegularExpressions;

namespace AutoBlogCreator.Services
{
    public class ArticleAdjuster : IArticleAdjuster
    {
        public string AdjustArticle(string article)
        {
            foreach (var youtubeVideoLink in GetYoutubeLinks(article))
            {
                string embedHtml = $"<iframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/{youtubeVideoLink.Groups[1].Value}\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>";
                article = article.Replace(youtubeVideoLink.Value, embedHtml);
            }

            foreach (var tweetLink in GetTweetLinks(article))
            {
                string urlEncodedTweetLink = System.Web.HttpUtility.UrlEncode(tweetLink);
                string embedHtml = $"<iframe border=0 frameborder=0 height=250 width=550\r\n src=\"https://twitframe.com/show?url={urlEncodedTweetLink}\"></iframe>";
                article = article.Replace(tweetLink, embedHtml);
            }

            return article;
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
            string pattern = @"(?:https?:\/\/)?(?:www\.)?(twitter|x)\.com\/(?:#!\/)?(\w+)\/status(?:es)?\/(\d+)";

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
