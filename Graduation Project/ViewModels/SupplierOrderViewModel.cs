public class SupplierOrderViewModel
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string orderStatus { get; set; }
    public string PharmacistName { get; set; }
    public string BranchName { get; set; }
    public string SupplierId { get; set; }
    public string SupplierName { get; set; }

    public List<OrderItemViewModel> OrderItems { get; set; }
}

public class OrderItemViewModel
{
    public string MedicationName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total => Quantity * Price;
}
