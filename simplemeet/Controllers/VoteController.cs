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
    public class VoteController : Controller
    {
        private readonly simplemeetContext _context;
        public VoteController(simplemeetContext context)
        {
            _context = context;

        }

        public async Task<IActionResult> YesVote(int? id)
        {
            var user_email = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            var logged_user_id = (_context.User!).FirstOrDefault(m => m.EmailAddress == user_email)!.Id;

            var topic = await _context.Topic!
                .Include(t => t.Comments!)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            var topic_votes = _context.Vote!.Where(v => v.TopicId == topic.Id);
            var old_vote = await topic_votes.FirstOrDefaultAsync(v => v.UserId == logged_user_id);

            if (old_vote != null){
                if (old_vote.Choice == true)
                {
                    TempData["warning"] = "you already casted a yes vote!";
                }
                else
                {
                    TempData["success"] = "changed your vote from no to yes";
                    old_vote.Choice = true;
                    _context.Update(old_vote);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var new_vote = new Vote();
                new_vote.UserId = logged_user_id;
                new_vote.TopicId = topic.Id;
                new_vote.Choice = true;
                _context.Add(new_vote);
                await _context.SaveChangesAsync();
                TempData["success"] = "you casted yes vote";
            }

            return RedirectToAction("Details", "Topics", new { id = id });
        }


        public async Task<IActionResult> NoVote(int? id)
        {
            var user_email = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            var logged_user_id = (_context.User!).FirstOrDefault(m => m.EmailAddress == user_email)!.Id;

            var topic = await _context.Topic!
                .Include(t => t.Comments!)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            var topic_votes = _context.Vote!.Where(v => v.TopicId == topic.Id);
            var old_vote = await topic_votes!.FirstOrDefaultAsync(v => v.UserId == logged_user_id);
            if (old_vote != null)
            {
                if (old_vote.Choice == false)
                {
                    TempData["warning"] = "you already casted a no vote!";
                }
                else
                {
                    TempData["success"] = "changed your vote from yes to no";
                    old_vote.Choice = false;
                    _context.Update(old_vote);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var new_vote = new Vote();
                new_vote.UserId = logged_user_id;
                new_vote.TopicId = topic.Id;
                new_vote.Choice = false;
                _context.Add(new_vote);
                await _context.SaveChangesAsync();
                TempData["success"] = "you casted no vote";
            }
            return RedirectToAction("Details", "Topics", new { id = id });
        }

    }
}
