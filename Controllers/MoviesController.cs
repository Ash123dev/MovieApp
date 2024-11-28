using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.Data;
using System;

public class MoviesController : Controller
{
	private readonly MovieDbContext _context;

	public MoviesController(MovieDbContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Index(string search, int page = 1)
	{
		int pageSize = 5;
		var query = _context.MovieTables.AsQueryable();

		if (!string.IsNullOrEmpty(search))
		{
			query = query.Where(m => m.Name.Contains(search));
		}

		var movies = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
		ViewBag.Search = search;
		ViewBag.CurrentPage = page;
		ViewBag.TotalPages = (int)Math.Ceiling((double)await query.CountAsync() / pageSize);

		return View(movies);
	}
}