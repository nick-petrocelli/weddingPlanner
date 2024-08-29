using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WeddingPlanner.Models;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;


namespace WeddingPlanner.Controllers;

public class WeddingController : Controller
{ 
    private readonly MyContext _context;  

    public WeddingController(MyContext context)
    {
        _context = context;
    }

    /* ---------- Dashboard ---------- */

    [HttpGet("Dashboard")]
    public IActionResult Dashboard()
    {
        var weddings = _context.Wedding
                    .Include(w => w.GuestList)
                    .Include(w => w.Planner)
                    .ToList();

    // Retrieve UserId and UserName from session
    int? userId = HttpContext.Session.GetInt32("UserId");
    string? userName = HttpContext.Session.GetString("UserName");

    // Pass UserId and UserName to ViewData
    ViewData["CurrentUserId"] = userId;
    ViewData["CurrentUserName"] = userName;
    
    if (!userId.HasValue)
        {
            return RedirectToAction("Index", "User"); 
        }

    return View("Dashboard", weddings);
    }


    /* ---------- Create Page & Process ---------- */
    [HttpGet("Wedding/new")]
    public IActionResult WeddingCreatePage(){
        
        int? userId = HttpContext.Session.GetInt32("UserId");
        string? userName = HttpContext.Session.GetString("UserName");
        ViewData["CurrentUserName"] = userName;

        if (!userId.HasValue)
        {
            return RedirectToAction("Index", "User"); 
        }
        return View();
    }

[HttpPost("Wedding/create")]
public IActionResult CreateWedding(Wedding newWedding)
{
    int? userId = HttpContext.Session.GetInt32("UserId");
    string? userName = HttpContext.Session.GetString("UserName");
    ViewData["CurrentUserName"] = userName;
    
    if (!userId.HasValue)
    {
        // Handle case where userId is null | user not logged in
        return RedirectToAction("Index", "User"); 
    }

    if (ModelState.IsValid)
    {   
        newWedding.UserId = userId.Value; 
        _context.Add(newWedding);
        _context.SaveChanges();       
        return RedirectToAction("WeddingDetailsPage", new { id = newWedding.WeddingId });  
    }

    // If ModelState is not valid, return to the create page with the model
    return View("WeddingCreatePage", newWedding); 
}

    /* ---------- Details Page ---------- */
    [HttpGet("Wedding/{id}")]
    public IActionResult WeddingDetailsPage(int id)
    {
        var wedding = _context.Wedding
            .Include(w => w.GuestList)
            .ThenInclude(g => g.User)
            .Include(w => w.Planner)
            .FirstOrDefault(w => w.WeddingId == id);


        int? userId = HttpContext.Session.GetInt32("UserId");
        string? userName = HttpContext.Session.GetString("UserName");
        ViewData["CurrentUserName"] = userName;

        if (!userId.HasValue)
        {
            return RedirectToAction("Index", "User"); 
        }

        return View("WeddingDetailsPage", wedding);
    }

    /*---------- Delete Action ----------*/
    [HttpPost("Wedding/delete/{id}")]
    public IActionResult Delete(int id)
    {
        int? currentUserId = HttpContext.Session.GetInt32("UserId");

        // Retrieve the wedding from the database
        var wedding = _context.Wedding.FirstOrDefault(w => w.WeddingId == id);

        // Check if the wedding exists and if the current user is the planner
        if (wedding != null && wedding.UserId == currentUserId)
        {
            _context.Wedding.Remove(wedding);
            _context.SaveChanges();
    }

    return RedirectToAction("Dashboard");
}

    /*---------- RSVP Action ----------*/

[HttpPost("Wedding/rsvp/{weddingId}")]
public IActionResult RSVP(int weddingId)
{
    int? userId = HttpContext.Session.GetInt32("UserId");

    // Check if the user has already RSVP'd to this wedding
    var existingGuest = _context.Guest.FirstOrDefault(g => g.WeddingId == weddingId && g.UserId == userId);

    if (existingGuest != null)
    {
        _context.Guest.Remove(existingGuest);
        _context.SaveChanges();
    }
    else
    {
        var newGuest = new Guest
        {
            WeddingId = weddingId,
            UserId = userId.Value 
        };

        _context.Add(newGuest);
        _context.SaveChanges();
    }

    return RedirectToAction("Dashboard");
}
}
