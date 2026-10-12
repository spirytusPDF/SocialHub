using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialHub.Context;
using SocialHub.Models;
using System.Security.Claims;

namespace SocialHub.Controllers
{
    public class SocialHubController : Controller
    {
        private readonly AppDbContext _context;
        public SocialHubController(AppDbContext context)
        {
            _context = context;
        }

        //Post management
        public async Task<IActionResult> Posts()
        {
            return View(await _context.Posts.Include(p=>p.comments).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Author,Title,Text,CreatedAt")] Post post)
        {
           
            if (post.Author == "")
            { 
            ModelState.Remove("Author");
            ModelState.Remove("user");
            ModelState.Remove("userId");
            post.Author = User.Identity.Name;
            post.userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            }
            post.CreatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
               
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Posts));
            }
            return View(post);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.comments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Posts));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Author,Title,Text,CreatedAt")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = post.Id });
            }
            return View(post);
        }


        public async Task<IActionResult> Search(string author = "")
        {
            var searchResult = await _context.Posts.Include(p => p.comments).Where(p=> EF.Functions.Like(p.Author, author)).ToListAsync();
            return View(searchResult);
            
        }


        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }



        //Comment Section
        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            if(comment.Author == "") {comment.Author = User.Identity.Name; }
            
            comment.CreatedAt = DateTime.Now;

            _context.Add(comment);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Details), new { id = comment.postId });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id, int postId)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = postId });
        }
    }
}
