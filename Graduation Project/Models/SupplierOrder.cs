public class SupplierOrder
{
    public int Id { get; set; }
    public string PharmacyId { get; set; }  // The pharmacy placing the order
    public Branch Branch { get; set; }
    public DateTime OrderDate { get; set; }
    public ICollection<SupplierOrderItem> SupplierOrderItems { get; set; }
}

public class SupplierOrderItem
{
    public int Id { get; set; }
    public int SupplierOrderId { get; set; }
    public SupplierOrder SupplierOrder { get; set; }
    public int MedicationId { get; set; }
    public Medicine Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
