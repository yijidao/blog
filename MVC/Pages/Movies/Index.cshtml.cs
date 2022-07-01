using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<Movie> Movie { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public SelectList Genres { get; set; }

        [BindProperty(SupportsGet = true)]
        public string MovieGenre { get; set; }

        public async Task OnGetAsync()
        {
            if (_context.Movie != null)
            {
                var movies = from m in _context.Movie select m;
                var genreQuery = from m in _context.Movie orderby m.Genre select m.Genre;

                if (!string.IsNullOrWhiteSpace(SearchString))
                {
                    movies = movies.Where(x => x.Title.Contains(SearchString));
                }

                if (!string.IsNullOrWhiteSpace(MovieGenre))
                {
                    movies = movies.Where(x => x.Genre == MovieGenre);
                }

                Genres = new SelectList(await genreQuery.Distinct().ToListAsync());
                Movie = await movies.ToListAsync();
            }
        }
    }
}
