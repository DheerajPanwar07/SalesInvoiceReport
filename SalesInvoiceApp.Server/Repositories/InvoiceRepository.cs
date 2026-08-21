using System.Data;
using Dapper;
using SalesInvoiceApp.Server.Data;
using SalesInvoiceApp.Server.Models;

namespace SalesInvoiceApp.Server.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DapperContext _context;

        public InvoiceRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> SaveInvoiceAsync(InvoiceHeader invoice)
        {
            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();


            const string insertHeaderSql = @"
                INSERT INTO InvoiceHeaders (CustomerNo, CustomerName, InvoiceDate)
                VALUES (@CustomerNo, @CustomerName, @InvoiceDate);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            const string insertItemSql = @"
                INSERT INTO InvoiceItems (InvoiceHeaderId, ItemNo, ItemName, Qty, MRP, Amount)
                VALUES (@InvoiceHeaderId, @ItemNo, @ItemName, @Qty, @MRP, @Amount);";
            try
            {
                foreach (var item in invoice.Items)
                {
                    item.Amount = item.Qty * item.MRP;
                }

                int headerId = await connection.ExecuteScalarAsync<int>(insertHeaderSql, invoice, transaction);
                invoice.Id = headerId;

                foreach (var item in invoice.Items)
                {
                    item.InvoiceHeaderId = headerId;
                    await connection.ExecuteAsync(insertItemSql, item, transaction);
                }

                transaction.Commit();
                return headerId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<SalesReportDto>> GetSalesReportAsync()
        {
            const string querySql = @"
                SELECT 
                    CONVERT(VARCHAR(10), h.InvoiceDate, 120) AS InvoiceDate,
                    h.CustomerName,
                    i.ItemName,
                    i.Qty,
                    i.MRP,
                    i.Amount
                FROM InvoiceItems i
                INNER JOIN InvoiceHeaders h ON i.InvoiceHeaderId = h.Id
                ORDER BY h.InvoiceDate DESC, i.Id DESC;";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SalesReportDto>(querySql);
        }
    }
}
