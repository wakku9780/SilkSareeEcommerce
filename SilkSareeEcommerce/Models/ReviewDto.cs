namespace SilkSareeEcommerce.Models
{
    // ReviewDto.cs
    public class ReviewDto
    {
        public int ProductId { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
    }

}
