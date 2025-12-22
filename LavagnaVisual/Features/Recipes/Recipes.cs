using Bridge.Models;
using LavagnaVisual.Features.Recipes.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LavagnaVisual.Features.Recipes
{
    public static class Recipes
    {
        public static void MapRecipesEndpoints(this RouteGroupBuilder builder)
        {
            var group = builder.MapGroup("recipes")
                .MapToApiVersion(1, 0)
                .WithTags(Tags.Recipes);

            group.MapGet("", GetAllRecipesAsync)
                .WithDescription("CRUD operation to get a list of all recipes");

            group.MapGet("{placeId}", GetRecipeByIdAsync)
                .WithDescription("CRUD operation to get the recipe with specific Id");
        }

        static async Task<Ok<IEnumerable<Recipe>>> GetAllRecipesAsync(RecipeServiceProvider serviceProvider)
        {
            return TypedResults.Ok(await serviceProvider.GetAllRecipesAsync());
        }

        static async Task<Results<Ok<Recipe>, NotFound>> GetRecipeByIdAsync(RecipeServiceProvider serviceProvider, int placeId)
        {
            var recipe = await serviceProvider.GetRecipeByPlaceIdAsync(placeId);
            if (recipe == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(recipe);
        }
    }
}
