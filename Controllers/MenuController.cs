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

    // =========================
    // GET: api/menu
    // =========================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.MenuItems
            .Where(x => x.IsAvailable)
            .ToListAsync();

        return Ok(items);
    }

    // =========================
    // POST: api/menu
    // =========================
    [HttpPost]
    public async Task<IActionResult> Create(MenuItem item)
    {
        _db.MenuItems.Add(item);
        await _db.SaveChangesAsync();
        return Ok(item);
    }

    // =========================
    // PUT: api/menu/79
    // =========================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, MenuItem updated)
    {
        var item = await _db.MenuItems.FindAsync(id);
        if (item == null)
            return NotFound();

        // Alanları güncelle
        item.Name = updated.Name;
        item.Category = updated.Category;
        item.Price = updated.Price;
        item.IsAvailable = updated.IsAvailable;
        item.ImageUrl = updated.ImageUrl;
        item.Description = updated.Description;

        await _db.SaveChangesAsync();
        return Ok(item);
    }

    // =========================
    // DELETE: api/menu/79
    // =========================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.MenuItems.FindAsync(id);
        if (item == null)
            return NotFound();

        _db.MenuItems.Remove(item);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
