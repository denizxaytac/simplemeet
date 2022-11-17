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
    public class CommentsController : Controller
    {
        private readonly simplemeetContext _context;

        public CommentsController(simplemeetContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Post([Bind("Text")] Comment comment, int id, string Text)
        {
            var commentText = HttpContext.Request.Form["Text"];
            comment.Content = commentText;
            comment.TopicId = id;
            var realTopic = _context.Topic!.Find(id);

            var useremail = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            var user = (_context.User!).First(u => u.EmailAddress == useremail);
            comment.UserId = user.Id;
            _context.Add(comment);
            _context.SaveChanges();
            (realTopic!).Comments!.Add(comment);
            _context.Update(realTopic);
            _context.SaveChanges();
            TempData["success"] = "comment posted!";
            return RedirectToAction("Details", "Topics", new { id = id });
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,CommentDate,TopicId,UserId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TopicId"] = new SelectList(_context.Topic, "Id", "Id", comment.TopicId);
            return View(comment);
        }
        public IActionResult Delete(int? id)
        {
            var comment = (_context.Comment!).FirstOrDefault(m => m.Id == id);
            var commentUser = (_context.User!).FirstOrDefault(m => m.Id == comment!.UserId);
            var commentIssue = (_context.Topic!).Include(c => c.Comments).FirstOrDefault(m => m.Id == comment!.TopicId);

            // Check if the owner of the comment is the same as the current login
            var LoggerEmail = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            if (commentUser!.EmailAddress != LoggerEmail)
            {
                return NotFound();
            }
            commentIssue!.Comments!.Remove(comment!);
            _context!.Comment!.Remove(comment!);
            _context.Update(comment!);
            _context!.Update(commentIssue);
            _context.SaveChanges();
            TempData["success"] = "comment deleted!";
            return RedirectToAction("Details", "Topics", new { id = commentIssue.Id });
        }


        private bool CommentExists(int id)
        {
          return _context.Comment.Any(e => e.Id == id);
        }
    }
}
