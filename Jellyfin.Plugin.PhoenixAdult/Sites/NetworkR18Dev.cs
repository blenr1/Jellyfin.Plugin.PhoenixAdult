using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhoenixAdult.Configuration;
using PhoenixAdult.Helpers;
using PhoenixAdult.Helpers.Utils;

namespace PhoenixAdult.Sites
{
    public class NetworkR18Dev : IProviderBase
    {
        private class Jav2
        {
            [JsonProperty("actors")]
            public List<object> Actors { get; set; }

            [JsonProperty("actresses")]
            public List<Actress> Actresses { get; set; }

            [JsonProperty("authors")]
            public List<object> Authors { get; set; }

            [JsonProperty("categories")]
            public List<Category> Categories { get; set; }

            [JsonProperty("comment_en")]
            public object CommentEn { get; set; }

            [JsonProperty("content_id")]
            public string ContentId { get; set; }

            [JsonProperty("directors")]
            public List<Actress> Directors { get; set; }

            [JsonProperty("dvd_id")]
            public string DvdId { get; set; }

            [JsonProperty("gallery")]
            public List<Gallery> Gallery { get; set; }

            [JsonProperty("histrions")]
            public List<object> Histrions { get; set; }

            [JsonProperty("jacket_full_url")]
            public Uri JacketFullUrl { get; set; }

            [JsonProperty("jacket_thumb_url")]
            public Uri JacketThumbUrl { get; set; }

            [JsonProperty("label_id")]
            public long LabelId { get; set; }

            [JsonProperty("label_name_en")]
            public string LabelNameEn { get; set; }

            [JsonProperty("label_name_ja")]
            public string LabelNameJa { get; set; }

            [JsonProperty("maker_id")]
            public long MakerId { get; set; }

            [JsonProperty("maker_name_en")]
            public string MakerNameEn { get; set; }

            [JsonProperty("maker_name_ja")]
            public string MakerNameJa { get; set; }

            [JsonProperty("release_date")]
            public DateTimeOffset ReleaseDate { get; set; }

            [JsonProperty("runtime_mins")]
            public long RuntimeMins { get; set; }

            [JsonProperty("sample_url")]
            public Uri SampleUrl { get; set; }

            [JsonProperty("series_id")]
            public object SeriesId { get; set; }

            [JsonProperty("service_code")]
            public string ServiceCode { get; set; }

            [JsonProperty("site_id")]
            public long SiteId { get; set; }

            [JsonProperty("title_en")]
            public string TitleEn { get; set; }

            [JsonProperty("title_en_is_machine_translation")]
            public bool TitleEnIsMachineTranslation { get; set; }

            [JsonProperty("title_ja")]
            public string TitleJa { get; set; }
        }

        private class Actress
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("image_url", NullValueHandling = NullValueHandling.Ignore)]
            public string ImageUrl { get; set; }

            [JsonProperty("name_kana")]
            public string NameKana { get; set; }

            [JsonProperty("name_kanji")]
            public string NameKanji { get; set; }

            [JsonProperty("name_romaji")]
            public string NameRomaji { get; set; }
        }

        private class Category
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("name_en")]
            public string NameEn { get; set; }

            [JsonProperty("name_en_is_machine_translation")]
            public bool NameEnIsMachineTranslation { get; set; }

            [JsonProperty("name_ja")]
            public string NameJa { get; set; }
        }

        private class Gallery
        {
            [JsonProperty("image_full")]
            public Uri ImageFull { get; set; }

            [JsonProperty("image_thumb")]
            public Uri ImageThumb { get; set; }
        }

        private class Jav
        {
            [JsonProperty("actresses")]
            public Label[] Actresses { get; set; }

            [JsonProperty("categories")]
            public Label[] Categories { get; set; }

            [JsonProperty("content_id")]
            public string ContentId { get; set; }

            [JsonProperty("director")]
            public string Director { get; set; }

            [JsonProperty("images")]
            public Images Images { get; set; }

            [JsonProperty("label")]
            public Label Label { get; set; }

            [JsonProperty("maker")]
            public Label Maker { get; set; }

            [JsonProperty("release_date")]
            public DateTimeOffset ReleaseDate { get; set; }

            [JsonProperty("runtime_minutes")]
            public long RuntimeMinutes { get; set; }

            [JsonProperty("sample")]
            public Sample Sample { get; set; }

            [JsonProperty("series")]
            public object Series { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }
        }

        private class Label
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        private class Images
        {
            [JsonProperty("jacket_image")]
            public JacketImage JacketImage { get; set; }
        }

        private class JacketImage
        {
            [JsonProperty("large")]
            public string Large { get; set; }

            [JsonProperty("large2")]
            public Uri Large2 { get; set; }
        }

        private class Sample
        {
            [JsonProperty("high")]
            public Uri High { get; set; }
        }

        private static readonly IDictionary<string, string> CensoredWords = new Dictionary<string, string>
        {
            { "A*****t", "Assault" },
            { "A****p", "Asleep" },
            { "A***e", "Abuse" },
            { "B***d", "Blood" },
            { "B**d", "Bled" },
            { "C***d", "Child" },
            { "C*ck", "Cock" },
            { "D******e", "Disgrace" },
            { "D***king", "Drinking" },
            { "D***k", "Drunk" },
            { "D**g", "Drug" },
            { "F*****g", "Forcing" },
            { "F***e", "Force" },
            { "G*******g", "Gangbang" },
            { "G******g", "Gang Bang" },
            { "H*********n", "Humiliation" },
            { "H*******e", "Hypnotize" },
            { "H*******m", "Hypnotism" },
            { "H**t", "Hurt" },
            { "I****t", "Incest" },
            { "K****p", "Kidnap" },
            { "K****r", "Killer" },
            { "K**l", "Kill" },
            { "K*d", "Kid" },
            { "M************n", "Mother And Son" },
            { "M****t", "Molest" },
            { "P********t", "Passed Out" },
            { "P****h", "Punish" },
            { "R****g", "Raping" },
            { "R**e", "Rape" },
            { "RStepB****************r", "Stepbrother and Sister" },
            { "S*********l", "School Girl" },
            { "S********l", "Schoolgirl" },
            { "S******g", "Sleeping" },
            { "S*****t", "Student" },
            { "S***e", "Slave" },
            { "S**t", "Scat" },
            { "Sch**l", "School" },
            { "StepM************n", "Stepmother and Son" },
            { "T******e", "Tentacle" },
            { "T*****e", "Torture" },
            { "U*********s", "Unconscious" },
            { "V*****e", "Violate" },
            { "V*****t", "Violent" },
            { "Y********l", "Young Girl" },
        };

        public async Task<List<RemoteSearchResult>> Search(int[] siteNum, string searchTitle, DateTime? searchDate, CancellationToken cancellationToken)
        {
            var result = new List<RemoteSearchResult>();
            if (siteNum == null || string.IsNullOrEmpty(searchTitle))
            {
                return result;
            }

            string searchJAVID = null;
            var splitedTitle = searchTitle.Split();
            if (splitedTitle.Length > 1 && int.TryParse(splitedTitle[1], out _))
            {
                searchJAVID = $"{splitedTitle[0]}-{splitedTitle[1]}";
                searchTitle = searchJAVID;
            }

            var url = Helper.GetSearchSearchURL(siteNum) + "dvd_id=" + searchTitle + "/json";
            HttpClient client = new HttpClient();
            var resultString = client.GetStringAsync(url, cancellationToken).Result;

            Logger.Info(resultString);

            if (resultString == string.Empty)
            {
                return result;
            }

            Jav data = JsonConvert.DeserializeObject<Jav>(resultString);

            url = Helper.GetSearchSearchURL(siteNum) + "combined=" + data.ContentId + "/json";
            resultString = client.GetStringAsync(url, cancellationToken).Result;

            Logger.Info(resultString);

            if (resultString == string.Empty)
            {
                return result;
            }

            Jav2 data2 = JsonConvert.DeserializeObject<Jav2>(resultString);

            var sceneURL = new Uri(url);
            string curID = Helper.Encode(sceneURL.AbsolutePath),
                sceneName = Decensor(data2.TitleEn),
                scenePoster = data2.JacketFullUrl.ToString(),
                javID = data2.DvdId;

            var res = new RemoteSearchResult
            {
                ProviderIds = { { Plugin.Instance.Name, curID } },
                Name = $"{javID} {sceneName}",
                ImageUrl = scenePoster,
            };

            if (!string.IsNullOrEmpty(searchJAVID))
            {
                res.IndexNumber = 100 - LevenshteinDistance.Calculate(searchJAVID, javID, StringComparison.OrdinalIgnoreCase);
            }

            result.Add(res);

            return result;
        }

        public async Task<MetadataResult<BaseItem>> Update(int[] siteNum, string[] sceneID, CancellationToken cancellationToken)
        {
            var result = new MetadataResult<BaseItem>()
            {
                Item = new Movie(),
                People = new List<PersonInfo>(),
            };

            if (sceneID == null)
            {
                return result;
            }

            var url = Helper.Decode(sceneID[0]);
            HttpClient client = new HttpClient();
            var resultString = client.GetStringAsync(Helper.GetSearchBaseURL(siteNum) + url, cancellationToken).Result;

            Logger.Info(resultString);

            if (resultString == string.Empty)
            {
                return result;
            }

            Jav2 data = JsonConvert.DeserializeObject<Jav2>(resultString);

            url = Helper.GetSearchSearchURL(siteNum) + "combined=" + data.ContentId + "/json";
            var sceneURL = new Uri(url);
            result.Item.ExternalId = url;

            var javID = data.DvdId;

            result.Item.OriginalTitle = javID.ToUpperInvariant();
            result.Item.Name = Decensor(data.TitleEn);

            var studio = data.LabelNameEn;
            if (!string.IsNullOrEmpty(studio))
            {
                result.Item.AddStudio(studio);
            }

            var date = data.ReleaseDate.DateTime.ToString();
            if (!string.IsNullOrEmpty(date))
            {
                date = date
                    .Replace(".", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace(",", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("Sept", "Sep", StringComparison.OrdinalIgnoreCase)
                    .Replace("June", "Jun", StringComparison.OrdinalIgnoreCase)
                    .Replace("July", "Jul", StringComparison.OrdinalIgnoreCase)
                    .Trim();

                if (DateTime.TryParseExact(date, "MMM dd yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var sceneDateObj))
                {
                    result.Item.PremiereDate = sceneDateObj;
                }
            }

            var genreNode = data.Categories.Select(x => x.NameEn);
            foreach (var genreLink in genreNode)
            {
                var genreName = genreLink;
                genreName = Decensor(genreName);

                result.Item.AddGenre(genreName);
            }

            var actorsNode = data.Actresses;
            foreach (var actorLink in actorsNode)
            {
                var actorName = actorLink.NameRomaji;

                if (actorName != "----")
                {
                    switch (Plugin.Instance.Configuration.JAVActorNamingStyle)
                    {
                        case JAVActorNamingStyle.JapaneseStyle:
                            actorName = string.Join(" ", actorName.Split().Reverse());
                            break;
                    }

                    var actor = new PersonInfo
                    {
                        Name = actorName,
                    };

                    actor.ImageUrl = "https://pics.dmm.co.jp/mono/actjpgs/" + actorLink.ImageUrl;

                    result.People.Add(actor);
                }
            }

            return result;
        }

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(int[] siteNum, string[] sceneID, BaseItem item, CancellationToken cancellationToken)
        {
            var result = new List<RemoteImageInfo>();

            if (sceneID == null)
            {
                return result;
            }

            var sceneURL = Helper.Decode(sceneID[0]);
            if (!sceneURL.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                sceneURL = Helper.GetSearchBaseURL(siteNum) + sceneURL;
            }

            HttpClient client = new HttpClient();
            var resultString = client.GetStringAsync(sceneURL, cancellationToken).Result;

            if (resultString == string.Empty)
            {
                return result;
            }

            Jav2 data = JsonConvert.DeserializeObject<Jav2>(resultString);

            var img = data.JacketFullUrl.ToString();
            result.Add(new RemoteImageInfo
            {
                Url = img,
                Type = ImageType.Primary,
            });

            return result;
        }

        private static string Decensor(string text)
        {
            var result = text;

            foreach (var word in CensoredWords)
            {
                if (!result.Contains('*', StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                result = result.Replace(word.Key, word.Value, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }
    }
}
