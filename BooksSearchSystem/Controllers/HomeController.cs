using BooksSearchSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplicationTest;

namespace BooksSearchSystem.Controllers
{
    public class HomeController : Controller
    {
        BooksCrawler BooksInfo = new BooksCrawler();
        string id = "0010764130";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public async Task<IActionResult>  Index()
        {
            string jsonData = await BooksInfo.booksInfo(id);
            ViewBag.JsonData = jsonData;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
