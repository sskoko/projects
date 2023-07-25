using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Structures;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using TransactionUpload.DataAccess;
using TransactionUpload.Services;

namespace TransactionUploadWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public TransactionController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file)
        {
            string Errlogfolder = Directory.GetCurrentDirectory()+"\\log";

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file selected.");
            }

            // Validate file size
            if (file.Length > 1024 * 1024) // 1 MB limit
            {
                return BadRequest("File size exceeds the limit of 1 MB.");
            }

            // Read the uploaded file and determine its format
            string fileContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                fileContent = reader.ReadToEnd();
            }

            // Parse data based on the file format (CSV or XML)
            bool isCsv = file.FileName.EndsWith(".csv");
            bool isXml = file.FileName.EndsWith(".xml");

            if (isCsv)
            {
                var parser = new CsvTransactionParser(Errlogfolder);
                var transactions = parser.Parse(fileContent);

                if (parser.IsValid)
                {
                    _dbContext.TransactionUPD.AddRange(transactions);
                    _dbContext.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest(parser.GetValidationMessages());
                }
            }
            else if (isXml)
            {
                var parser = new XmlTransactionParser(Errlogfolder);
                var transactions = parser.Parse(fileContent);

                if (parser.IsValid)
                {
                    _dbContext.TransactionUPD.AddRange(transactions);
                    _dbContext.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest(parser.GetValidationMessages());
                }
            }
            else
            {
                return BadRequest("Unknown format");
            }
        }


        //Get all transactions. Create API methods:a)by Currency b)by date range c)by status
        [HttpGet("byCurrency/{currency}")]//GET /api/byCurrency/USD
        public IActionResult GetTransactionsByCurrency(string currency)
        {
            var transactions = _dbContext.TransactionUPD.Where(t => t.CurrencyCode == currency).ToList();
            return Ok(transactions);
        }

        [HttpGet("byDateRange")]//GET /api/byDateRange?fromDate=2023-07-01&toDate=2023-07-20
        public IActionResult GetTransactionsByDateRange(DateTime fromDate, DateTime toDate)
        {
            var transactions = _dbContext.TransactionUPD
                .Where(t => t.TransactionDate >= fromDate && t.TransactionDate <= toDate)
                .ToList();
            return Ok(transactions);
        }


        [HttpGet("byStatus/{status}")]//GET /api/byStatus/Approved
        public IActionResult GetTransactionsByStatus(string status)
        {
            if (status == "Approved")
            {
                status = "A";
            }
            else if (status == "Failed"|| status == "Rejected")
            {
                status = "R";
            }
            else if (status == "Finished"|| status == "Done")
            {
                status = "D";
            }

            var transactions = _dbContext.TransactionUPD.Where(t => t.Status == status).ToList();
            return Ok(transactions);
        }
    }
}
