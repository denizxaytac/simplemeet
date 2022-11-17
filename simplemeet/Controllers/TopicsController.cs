using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using simplemeet.Data;
using simplemeet.Models;

namespace simplemeet
{
    public class TopicsController : Controller
    {
        private readonly simplemeetContext _context;

        public TopicsController(simplemeetContext context)
        {
            _context = context;
        }

        // GET: Topics
        public async Task<IActionResult> Index()
        {
              return View(await _context.Topic.ToListAsync());
        }

        // GET: Topics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.LoggedInUserEmail = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            if (id == null || _context.Topic == null)
            {
                return NotFound();
            }
            var topic = await _context.Topic!
                .Include(t => t.Comments!)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            topic.Creator = (_context.User!).FirstOrDefault(m => m.Id == topic.CreatorId)!;
            if (topic == null)
            {
                return NotFound();
            }

            return View(topic);
        }

        // GET: Topics/Create
        public IActionResult Create()
        {
            var user_email = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            ViewBag.CreatorId = (_context.User!).FirstOrDefault(m => m.EmailAddress == user_email)!.Id;
            ViewBag.Creator = (_context.User!).FirstOrDefault(m => m.EmailAddress == user_email)!;

            ViewBag.Users = new SelectList(_context.User!.ToList(), "Id", "Name");
            return View();
        }

        // POST: Topics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,StartTime,EndTime,CreatorId")] Topic topic)
        {
            topic.Creator = (_context.User!).FirstOrDefault(m => m.Id == topic.CreatorId)!;
            var UserIds = this.HttpContext.Request.Form["TopicUsers"].ToString().Split(',');
            foreach (var UserId in UserIds)
            {
                if (UserId != "")
                {
                    var usr = _context.User!.Find(int.Parse(UserId));
                    topic.Users!.Add(usr!);
                }
            }
            _context.Add(topic);
            await _context.SaveChangesAsync();
            TempData["success"] = "topic created!";
            return RedirectToAction("Details", "Topics", topic);

            //return RedirectToAction(nameof(Index));

            //return View(topic);
        }

        // GET: Topics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Topic == null)
            {
                return NotFound();
            }

            var topic = await _context.Topic.FindAsync(id);

            List<User> SelectedUsers = new List<User>();
            List<User> UnSelectedUsers = new List<User>();
            var AllUsers = _context.User!.Include(x => x.Topics);
            foreach (var usr in AllUsers)
            {
                foreach (var iss in usr.Topics)
                {
                    if (SelectedUsers.Contains(usr))
                    {
                        continue;
                    }
                    if (iss.Id == id)
                    {
                        SelectedUsers.Add(usr);
                    }
                }
            }
            foreach (var usr in AllUsers)
            {
                if (!SelectedUsers.Contains(usr))
                {
                    UnSelectedUsers.Add(usr);
                }
            }
            ViewBag.SelectedUsers = SelectedUsers;
            ViewBag.UnSelectedUsers = new SelectList(UnSelectedUsers, "Id", "Name");
            if (topic == null)
            {
                return NotFound();
            }
            return View(topic);
        }

        // POST: Topics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,StartTime,EndTime,CreatorId")] Topic topic)
        {
           
            var db_topic = _context.Topic!.Include(u => u.Users).First(p => p.Id == topic.Id);
            db_topic.Title = topic.Title;
            db_topic.Content = topic.Content;
            db_topic.StartTime = topic.StartTime;
            db_topic.EndTime = topic.EndTime;

            foreach (var usr in db_topic.Users!)
            {
                db_topic.Users.Remove(usr);
            }
            _context.Update(db_topic);

            var UserIds = this.HttpContext.Request.Form["TopicUsers"].ToString().Split(',');
            foreach (var UserId in UserIds)
            {
                if (UserId != "")
                {
                    var usr = _context.User!.Find(int.Parse(UserId));
                    db_topic.Users!.Add(usr!);
                }
            }
            try
            {
                _context.Update(db_topic);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TopicExists(db_topic.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            TempData["success"] = "changes saved!";
            return RedirectToAction("Details", "Topics", db_topic);
            
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Topic == null)
            {
                return Problem("Entity set 'simplemeetContext.Topic'  is null.");
            }
            var topic = await _context.Topic.FindAsync(id);
            if (topic != null)
            {
                _context.Topic.Remove(topic);
            }
            TempData["success"] = "topic deleted!";
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool TopicExists(int id)
        {
          return _context.Topic.Any(e => e.Id == id);
        }
    }
}
