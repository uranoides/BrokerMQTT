using System.Windows.Input;

namespace Client.LavagnaVisual.Test.Commands
{
    public static class ApiCommands
    {
        public static readonly RoutedCommand GetRecipes = new RoutedCommand("GetRecipes", typeof(ApiCommands));
        public static readonly RoutedCommand GetRecipeById = new RoutedCommand("GetRecipeById", typeof(ApiCommands));
    }
}
