using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AppleMusicRPC
{
    internal class TrackExtras
    {

        protected class iTunesSearchResponse
        {
            [JsonProperty("resultCount")]
            public int ResultCount { get; set; }

            [JsonProperty("results")]
            public ItunesSearchResult[] Results { get; set; }
        }

        protected class ItunesSearchResult
        {
            [JsonProperty("trackName")]
            public string TrackName { get; set; }

            [JsonProperty("collectionName")]
            public string CollectionName { get; set; }

            [JsonProperty("artworkUrl100")]
            public string ArtworkUrl { get; set; }

            [JsonProperty("trackViewUrl")]
            public string TrackViewUrl { get; set; }
        }


        public string ArtworkUrl { get; private set; }
        public string ItunesUrl { get; private set; }

        public static async Task<TrackExtras> GetTrackExtras(string? song, string? artist, string? album)
        {
            // GET JSON
            var searchQuery = $"{song} {artist} {album}".Replace("*", "");
            HttpClient httpClient = new HttpClient();

            var query = new Dictionary<string, string>
            {
                ["media"] = "music",
                ["entity"] = "song",
                ["term"] = searchQuery.Replace("#", "%23").Replace("&", "%26")
            };

            var data = await httpClient.GetAsync(QueryHelpers.AddQueryString("https://itunes.apple.com/search", query!));
            var response = JsonConvert.DeserializeObject<iTunesSearchResponse>(await data.Content.ReadAsStringAsync());


            // Get Track
            ItunesSearchResult? result = null;
            if (response.ResultCount == 1)
            {
                result = response.Results[0];
            }
            else if (response.ResultCount > 1)
            {
                result = response.Results.FirstOrDefault(x =>
                        x.CollectionName.ToLower().Contains(album.ToLower())
                    && x.TrackName.ToLower().Contains(song.ToLower())
                    ) ?? response.Results[0];
            }
            else if (Regex.Match(album, @"\(.*\)").Success)
            {
                return await GetTrackExtras(song, artist, Regex.Replace(album, @"\(.*\)", ""));
            }

            return new TrackExtras
            {
                ArtworkUrl = result?.ArtworkUrl,
                ItunesUrl = result?.TrackViewUrl
            };
        }
    }
}
