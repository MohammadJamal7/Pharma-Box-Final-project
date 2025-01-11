public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public Cart Cart { get; set; } // Navigation property
    public int MedicineId { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int BranchId { get; set; }
}
