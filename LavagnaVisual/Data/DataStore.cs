using Bridge.Models;

namespace LavagnaVisual.Data
{
    public static class DataStore
    {
        public static List<Support> Supports = new List<Support>
        {
            new Support { Id = 1, Name = "Europallet", EmptyColor = "#c1742f" },
        };
        public static List<SupportCategory> SupportCategories = new List<SupportCategory>
        {
            new SupportCategory { Id = 1, Name = "Pallet", CategoryId = 1 },
        };
        public static List<Udc> Udcs = new List<Udc>
        {
            new Udc { Id = 1, Code = 100, SupportId = 1, ReferenceId = null },
            new Udc { Id = 2, Code = 200, SupportId = 1, ReferenceId = null },
            new Udc { Id = 3, Code = 300, SupportId = 1, ReferenceId = null },
        };
        public static List<Recipe> Recipes = new List<Recipe>
        {
            new Recipe { Id = 1, PlaceId = 1, Udc = Udcs[0], DestinationPlaceId = 55, State = 0 },
            new Recipe { Id = 2, PlaceId = 2, Udc = Udcs[1], DestinationPlaceId = 66, State = 0 },
            new Recipe { Id = 3, PlaceId = 3, Udc = Udcs[2], DestinationPlaceId = 81, State = 0 },
        };
        public static List<Place> Places = new List<Place>
        {
            new Place { Id = 1, X = 5000, Y = 45000 },
            new Place { Id = 2, X = 8000, Y = 24000 },
            new Place { Id = 3, X = 7000, Y = 21000 },
        };
    }
}
