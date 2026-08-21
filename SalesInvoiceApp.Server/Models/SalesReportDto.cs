namespace SalesInvoiceApp.Server.Models
{
    public class SalesReportDto
    {
        public string InvoiceDate { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal MRP { get; set; }
        public decimal Amount { get; set; }
    }
}
