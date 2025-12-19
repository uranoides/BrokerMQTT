namespace Bridge.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public int? PlaceId { get; set; }
        public required Udc Udc { get; set; }
        public int? DestinationPlaceId { get; set; }
        public int? State { get; set; }
    }
}
