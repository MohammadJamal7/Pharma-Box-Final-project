public class OrderViewModel
{
    public List<OrderItemViewModel2> Items { get; set; }
    public decimal TotalAmount { get; set; }
    //public int BranchId { get; set; } // Added BranchId

}

public class OrderItemViewModel2
{
    public int MedicineId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

