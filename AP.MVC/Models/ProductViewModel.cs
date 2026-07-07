using System;

namespace AP.MVC.Models
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> InventoryID { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Rating { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<DateTime> LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
    }
}
