
using Bridge.Models;
using LavagnaVisual.Data;

namespace LavagnaVisual.Features.Recipes.Services
{
    public class RecipeServiceProvider()
    {
        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            return DataStore.Recipes;    
        }
        public async Task<Recipe?> GetRecipeByPlaceIdAsync(int placeId)
        {
            var recipe = DataStore.Recipes.FirstOrDefault(r => r.PlaceId == placeId);
            if (recipe == null)
                return null;
            
            return recipe;
        }
    }

}
