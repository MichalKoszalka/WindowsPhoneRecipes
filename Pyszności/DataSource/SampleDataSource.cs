using Newtonsoft.Json;
using Pyszności.Controller;
using Pyszności.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace Pyszności.DataSource
{

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private StorageFolder local = ApplicationData.Current.LocalFolder;
        private const string FAV_FILE = "favourites.json";

        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<Recipe> _recipes = new ObservableCollection<Recipe>();

        private SampleDataSource()
        {
           GetSampleDataAsync();
        }

        public ObservableCollection<Recipe> Recipes
        {
            get { return this._recipes; }
        }

        public static IEnumerable<Recipe> GetFavouriteRecipes()
        {
            return _sampleDataSource.Recipes;
        }

        public static async void AddToFavouritesAsync(Recipe recipe)
        {
           await _sampleDataSource.ExecuteAddFavouriteRecipe(recipe);
        }

        public static async void RemoveFromFavouritesAsync(Recipe recipe)
        {
            await _sampleDataSource.ExecuteRemoveFavouriteRecipe(recipe);
        }

        private async Task ExecuteAddFavouriteRecipe(Recipe recipe)
        {
            Recipes.Add(recipe);
            await ExecuteSaveFavouriteRecipes();
        }

        private async Task ExecuteRemoveFavouriteRecipe(Recipe recipe)
        {
            try
            {
                var toRemove = Recipes.First(x => x.Id.Equals(recipe.Id));
                Recipes.Remove(toRemove);
                await ExecuteSaveFavouriteRecipes();
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine("There was no favourite item with id = " + recipe.Id);
            }
        }

        private async Task ExecuteSaveFavouriteRecipes()
        {
            string content = JsonConvert.SerializeObject(Recipes);
            byte[] fileBytes = Encoding.UTF8.GetBytes(content.ToCharArray());
            StorageFile file = await local.CreateFileAsync(FAV_FILE, CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                stream.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        public static async Task<IEnumerable<Recipe>> GetRecipesAsync()
        {
            await _sampleDataSource.GetSampleDataAsync();

            return _sampleDataSource.Recipes;
        }

        public static async Task<Recipe> GetItemAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var match = _sampleDataSource.Recipes.Select(recipe => recipe).Where((recipe) => recipe.Id == uniqueId);
            if (match.Count() == 1) return match.First();
            return null;
        }

        private async Task GetSampleDataAsync()
        {
            if (this._recipes.Count != 0)
                return;

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(FAV_FILE));
            string jsonText = await FileIO.ReadTextAsync(file);
            IEnumerable<Recipe> model = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<IEnumerable<Recipe>>(jsonText));
            foreach(var recipe in model)
            {
                Recipes.Add(recipe);
            }
        }
    }
}
