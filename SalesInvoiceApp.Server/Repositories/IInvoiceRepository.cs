using SalesInvoiceApp.Server.Models;

namespace SalesInvoiceApp.Server.Repositories
{
    public interface IInvoiceRepository
    {
        Task<int> SaveInvoiceAsync(InvoiceHeader invoice);
        Task<IEnumerable<SalesReportDto>> GetSalesReportAsync();
    }
}
