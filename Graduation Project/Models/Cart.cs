public class Cart
{
    public int Id { get; set; }
    public string UserId { get; set; } // To associate the cart with a user
    public DateTime CreatedAt { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
