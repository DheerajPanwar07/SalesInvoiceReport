using Microsoft.AspNetCore.Mvc;
using SalesInvoiceApp.Server.Models;
using SalesInvoiceApp.Server.Repositories;

namespace SalesInvoiceApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _repository;

        public InvoiceController(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> SaveInvoice([FromBody] InvoiceHeader invoice)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (invoice.Items == null || !invoice.Items.Any())
                return BadRequest(new { message = "At least one item is required." });

            int newId = await _repository.SaveInvoiceAsync(invoice);
            return Ok(new { message = "Invoice saved successfully.", id = newId });
        }

        [HttpGet("salesreport")]
        public async Task<IActionResult> GetSalesReport()
        {
            var report = await _repository.GetSalesReportAsync();
            return Ok(report);
        }
    }
}
