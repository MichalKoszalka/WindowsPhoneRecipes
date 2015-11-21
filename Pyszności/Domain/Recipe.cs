using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Pyszności.Domain
{
    public class Recipe
    {
        [JsonProperty("recipe_id")]
        public string Id { get; set; }
        [JsonProperty("publisher")]
        public string Publisher { get; set; }
        [JsonProperty("ingredients")]
        public List<string> Ingredients { get; set; }
        [JsonProperty("source_url")]
        public Uri Url { get; set; }
        [JsonProperty("image_url")]
        public Uri ImageURL { get; set; }
        [JsonProperty("social_rank")]
        public double SocialRank { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("favourite")]
        public bool Favourite { get; set; }

    }


}

