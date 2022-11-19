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
            var user_email = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            var logged_user_id = (_context.User!).FirstOrDefault(m => m.EmailAddress == user_email)!.Id;
            ViewBag.LoggedInUserEmail = user_email;
            if (id == null || _context.Topic == null)
            {
                return NotFound();
            }
            var topic = await _context.Topic!
                .Include(t => t.Users!)
                .Include(t => t.Votes)
                .Include(t => t.Comments!)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            topic.Creator = (_context.User!).FirstOrDefault(m => m.Id == topic.CreatorId)!;
            if (topic == null)
            {
                return NotFound();
            }
            ViewBag.TopicUsers = topic.Users;
            // gets votes
            var topic_votes = _context.Vote!.Where(v => v.TopicId == topic.Id);
            ViewBag.TopicVotes = topic_votes;
            int yes_votes = 0;
            int no_votes = 0;
            bool votes_visible = false;
            foreach (var vote in topic_votes)
            {
                // only makes votes visible if the user already votes
                if (vote.UserId == logged_user_id)
                {
                    votes_visible = true;
                }
                if (vote.Choice == true)
                {
                    yes_votes += 1;
                }
                else
                {
                    no_votes += 1;
                }
            }
            if (!votes_visible)
            {
                ViewBag.votes_no = "";
                ViewBag.votes_yes = "";
            }
            else
            {
                ViewBag.votes_no = no_votes;
                ViewBag.votes_yes = yes_votes;
            }
            //


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

            var user_email = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            int logged_in_id = (_context.User!).FirstOrDefault(m => m.EmailAddress == user_email)!.Id;

            if (topic.CreatorId != logged_in_id)
            {
                TempData["warning"] = "you're not allowed to edit this topic!";
                return RedirectToAction("Details", "Topics", topic);
            }

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

            var user_email = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            int logged_in_id = (_context.User!).FirstOrDefault(m => m.EmailAddress == user_email)!.Id;
            if (topic.CreatorId != logged_in_id)
            {
                TempData["warning"] = "you're not allowed to delete this topic!";
                return RedirectToAction("Details", "Topics", topic);
            }
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
