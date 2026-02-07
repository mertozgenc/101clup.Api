using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _101clup.Api.Data;
using _101clup.Api.Models;

namespace _101clup.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly AppDbContext _db;

    public MenuController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.MenuItems
            .Where(x => x.IsAvailable)
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(MenuItem item)
    {
        _db.MenuItems.Add(item);
        await _db.SaveChangesAsync();
        return Ok(item);
    }
}
