using System.Net.Http.Headers;
using System.Text.Json;

namespace AutoBlogCreator.Services
{
    public class LinkToImageRetriever : ILinkToImageRetriever
    {
        private readonly string clientId = "ihtghq2i0jklk0y5yezs7jdbmtlvvk";
        private readonly string clientSecret = "hqxulqufbtx3xablefcksb33ntnqir";
        private readonly string tokenUrl = "https://id.twitch.tv/oauth2/token";
        private readonly string igdbUrl = "https://api.igdb.com/v4";

        public async Task<string> GetImageLink(string name)
        {
            string accessToken = await GetAccessToken();
            return await GetGameArtworkUrl(name, accessToken);
        }

        private async Task<string> GetAccessToken()
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                HttpResponseMessage response = await client.PostAsync(tokenUrl, content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument json = JsonDocument.Parse(responseBody);
                return json.RootElement.GetProperty("access_token").GetString();
            }
        }

        private async Task<string> GetGameArtworkUrl(string gameName, string accessToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Client-ID", clientId);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Fetch game ID
                string gameQuery = $"search \"{gameName}\"; fields id,name; limit 1;";
                HttpContent gameContent = new StringContent(gameQuery);
                gameContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

                HttpResponseMessage gameResponse = await client.PostAsync($"{igdbUrl}/games", gameContent);
                gameResponse.EnsureSuccessStatusCode();

                string gameResponseBody = await gameResponse.Content.ReadAsStringAsync();
                using JsonDocument gameJson = JsonDocument.Parse(gameResponseBody);
                if (gameJson.RootElement.GetArrayLength() == 0) throw new Exception("Game not found");

                int gameId = gameJson.RootElement[0].GetProperty("id").GetInt32();

                // Fetch artwork information
                string artworkQuery = $"fields url,width,image_id; where game = {gameId};";
                HttpContent artworkContent = new StringContent(artworkQuery);
                artworkContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

                HttpResponseMessage artworkResponse = await client.PostAsync($"{igdbUrl}/artworks", artworkContent);
                artworkResponse.EnsureSuccessStatusCode();

                string artworkResponseBody = await artworkResponse.Content.ReadAsStringAsync();
                using JsonDocument artworkJson = JsonDocument.Parse(artworkResponseBody);
                if (artworkJson.RootElement.GetArrayLength() == 0) throw new Exception("Artwork not found");

                // Find the artwork with the largest width
                string largestArtworkUrl = "";
                int maxWidth = 0;
                foreach (JsonElement artwork in artworkJson.RootElement.EnumerateArray())
                {
                    int width = artwork.GetProperty("width").GetInt32();
                    if (width > maxWidth)
                    {
                        maxWidth = width;
                        largestArtworkUrl = artwork.GetProperty("url").GetString().Replace("t_thumb", "t_1080p");
                    }
                }

                return "https:" + largestArtworkUrl; // URLs from IGDB need to be prefixed with https:
            }
        }
    }
}
