using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using coba.Models;
using coba.Models.Request;
using coba.Repositories.Domain;
using coba.Models.Database;

namespace coba.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IContactRepository _repo;

    public HomeController(ILogger<HomeController> logger, IContactRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(ContactRequest body)
    {
        if (!ModelState.IsValid)
        {
            return View(body);
        }

        // Process the data 
        _repo.SaveMessage(new ContactMessage()
        {
            Id = System.Guid.NewGuid().ToString(),
            Email = body.Email,
            Name = body.Name,
            Message = body.Message
        });

        return View("/Views/Home/ContactResult.cshtml", body);
    }

    public async Task<IActionResult> ContactList()
    {
        var data = await _repo.GetListMessageAsync("SELECT * FROM contact");
        return View("/Views/Home/ContactList.cshtml", data);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
