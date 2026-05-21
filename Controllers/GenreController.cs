using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop111.Constants;
using Shop111.Models;
using Shop111.Models.DTOs;
using Shop111.Repositories;

namespace Shop111.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Manager))]
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepo;
        private ApplicationDbContext _context;

        public GenreController(IGenreRepository genreRepo, ApplicationDbContext context)
        {
            _genreRepo = genreRepo;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _genreRepo.GetGenres();
            return View(genres);
        }

        public IActionResult AddGenre()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddGenre(GenreDTO genre)
        {
            if (!ModelState.IsValid)
            {
                return View(genre);
            }
            try
            {
                var genreToAdd = new Genre { GenreName = genre.GenreName, Id = genre.Id };
                await _genreRepo.AddGenre(genreToAdd);
                TempData["successMessage"] = "Genre added successfully";
                return RedirectToAction(nameof(AddGenre));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Genre could not added!";
                return View(genre);
            }

        }

        public async Task<IActionResult> UpdateGenre(int id)
        {
            var genre = await _genreRepo.GetGenreById(id);
            if (genre is null)
                throw new InvalidOperationException($"Genre with id: {id} does not found");
            var genreToUpdate = new GenreDTO
            {
                Id = genre.Id,
                GenreName = genre.GenreName
            };
            return View(genreToUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGenre(GenreDTO genreToUpdate)
        {
            if (!ModelState.IsValid)
            {
                return View(genreToUpdate);
            }
            try
            {
                var genre = new Genre { GenreName = genreToUpdate.GenreName, Id = genreToUpdate.Id };
                await _genreRepo.UpdateGenre(genre);
                TempData["successMessage"] = "Genre is updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Genre could not updated!";
                return View(genreToUpdate);
            }

        }

        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _genreRepo.GetGenreById(id);
            if (genre is null)
                throw new InvalidOperationException($"Genre with id: {id} does not found");
            await _genreRepo.DeleteGenre(genre);
            return RedirectToAction(nameof(Index));

        }
        ////✅ OnScreen Add Cetagory/Genre With Ajax Call Handler
        //public async Task<IActionResult> CreateGenreAjax(string genreName)
        //{
        //    if (string.IsNullOrWhiteSpace(genreName))
        //    {
        //        return Json(new { success = false, message = "Category name cannot be empty." });
        //    }

        //    var exists = await _context.Genres
        //                               .AnyAsync(g => g.GenreName.ToLower() == genreName.ToLower());
        //    if (exists)
        //    {
        //        return Json(new { success = false, message = "Category already exists." });
        //    }

        //    var genre = new Genre { GenreName = genreName };
        //    _context.Genres.Add(genre);
        //    await _context.SaveChangesAsync();

        //    return Json(new
        //    {
        //        success = true,   // ✅ should be true
        //        id = genre.Id,
        //        name = genre.GenreName
        //    });
        //}
        // ✅ OnScreen Add Category/Genre With Ajax Call Handler
        [HttpPost] // এটি নিশ্চিত করুন যাতে শুধু POST রিকোয়েস্ট অ্যালাউ করে
        public async Task<IActionResult> CreateGenreAjax(string genreName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(genreName))
                {
                    return Json(new { success = false, message = "Category name cannot be empty." });
                }

                // ডুপ্লিকেট চেক (আপনার লজিক ঠিক আছে)
                var exists = await _context.Genres
                                           .AnyAsync(g => g.GenreName.ToLower() == genreName.Trim().ToLower());

                if (exists)
                {
                    return Json(new { success = false, message = "Category already exists." });
                }

                var genre = new Genre { GenreName = genreName.Trim() }; // Trim ব্যবহার করা ভালো
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    id = genre.Id,
                    name = genre.GenreName
                });
            }
            catch (Exception ex)
            {
                // লগিং সিস্টেম থাকলে এখানে ex লগ করতে পারেন
                return Json(new { success = false, message = "Something went wrong while saving." });
            }
        }
    }
}


    

