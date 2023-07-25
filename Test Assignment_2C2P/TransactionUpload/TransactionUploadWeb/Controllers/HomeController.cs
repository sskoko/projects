using TransactionUpload.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using TransactionUpload.DataAccess;
using System.Linq;
using System.Collections.Generic;
using TransactionUpload.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TransactionUploadWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult LoadDatabase()
        {
            var transdata = _dbContext.TransactionUPD.ToList();

            if (transdata == null)
            {
                var result = new[]
                {
                 new { TranId = "null", payment = "null", Status = "null" }
                };

                return Json(result);

            }
            else
            {
              
                var result = transdata.Select(trans =>
                {
                    if (trans.Status == "Approved")
                    {
                        trans.Status = "A";
                    }
                    else if (trans.Status == "Failed" || trans.Status == "Rejected")
                    {
                        trans.Status = "R";
                    }
                    else if (trans.Status == "Finished" || trans.Status == "Done")
                    {
                        trans.Status = "D";
                    }

                    return new
                    {
                        TranId = trans.TransactionId,
                        payment = trans.Amount+" " + trans.CurrencyCode,
                        Status = trans.Status
                    };
                }).ToList();

                return Json(result);
            }
        }
    }
}
