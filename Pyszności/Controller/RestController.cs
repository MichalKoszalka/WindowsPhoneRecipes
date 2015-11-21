using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Pyszności.Domain;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Pyszności.Controller
{
    public class RecipesModel
    {
        [JsonProperty("recipes")]
        public List<Recipe> Recipes { get; set; }
    }

    public class RecipeModel
    {
        [JsonProperty("recipe")]
        public Recipe Recipe { get; set; }
    }

    public sealed class RestController
    {
        private static RestController _restController = new RestController();

        private static string URL = "https://community-food2fork.p.mashape.com/search?key=0e72defab142ebbf3abc7765270c758d&q=";

        private static string GET_SINGLE_URL = "https://community-food2fork.p.mashape.com/get?key=0e72defab142ebbf3abc7765270c758d&rId=";

        public ObservableCollection<Recipe> PopularRecipes = new ObservableCollection<Recipe>();

        public Recipe RecipeItem;

        private RestController()
        {

        }

        public static async Task<Recipe> GetRecipeAsync(string id)
        {
            await _restController.ExecuteGetRecipeAsync(id);
            return _restController.RecipeItem;
        }
    
        public static async Task<IList<Recipe>> GetRecipesAsync(string keyWord)
        {
            await _restController.ExecuteGetRecipesAsync(keyWord);
            return _restController.PopularRecipes;
        }

        public async Task ExecuteGetRecipeAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Mashape-Key", "B96xQQNUeFmshzTERF80qvDxvveAp1SZYaJjsnJmsAuxCzt3Wi");
                using (HttpResponseMessage response = await client.GetAsync(GET_SINGLE_URL + id.ToString()))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine(content);
                        RecipeModel model= await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<RecipeModel>(content));
                        RecipeItem = model.Recipe;
                    }
                }
            }
        }

        public async Task ExecuteGetRecipesAsync(string keyWord)
        {
            Debug.WriteLine("one");
            
            using (HttpClient client = new HttpClient())
            {
                Debug.WriteLine("two");

                client.DefaultRequestHeaders.Add("X-Mashape-Key", "B96xQQNUeFmshzTERF80qvDxvveAp1SZYaJjsnJmsAuxCzt3Wi");
                using (HttpResponseMessage response = await client.GetAsync(URL+keyWord))
                {
                    Debug.WriteLine("three");

                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("inside three");
                        string content = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine(content);
                        RecipesModel model = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<RecipesModel>(content));
                        Debug.WriteLine(model.Recipes);
                        PopularRecipes.Clear();
                        foreach(var item in model.Recipes)
                        {
                            PopularRecipes.Add(item);
                        }
                    }
                }
            }
        }
    }
}
