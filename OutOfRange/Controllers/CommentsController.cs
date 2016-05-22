using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using Microsoft.AspNet.Identity;
using OutOfRange.Models;
using OutOfRange.Utils;

namespace OutOfRange.Controllers
{
    [RoutePrefix("api/comments")]
    public class CommentsController : ApiController
    {
        private OutOfRangeEntities db = new OutOfRangeEntities();

        // GET: api/Comments
        public JsonResult<IEnumerable<CommentDTO>> GetComments()
        {
            return Json(db.Comments.ToList().Select(CommentDTO.FromEntity));
        }

        // GET: api/Comments/5
        [ResponseType(typeof(CommentDTO))]
        public IHttpActionResult GetComment(Guid id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(CommentDTO.FromEntity(comment));
        }

        // PUT: api/Comments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutComment(Guid id, Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comment.ID)
            {
                return BadRequest();
            }

            db.Entry(comment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        //POST: api/comments/AddScore
        [ResponseType(typeof(CommentDTO))]
        [HttpPost]
        [Route("AddScore")]
        public IHttpActionResult AddScore(ItemScore score)
        {
            Comment comment = db.Comments.Find(score.id);
            string userId = User.Identity.GetUserId();
            var scoreitems = comment.ScoreItems.Where(x => x.UserID == userId).ToList();
            if (scoreitems.Count == 0)
            {
                int addedScore = Math.Sign(score.score);
                comment.ScoreItems.Add(new ScoreItem()
                {
                    Added = DateTime.Now,
                    Score = addedScore,
                    UserID = userId,
                    ItemID = comment.ID,
                });

                comment.Score += addedScore;
                
            }
            else
            {
                int addedScore = Math.Sign(score.score);
                var scoreItem = scoreitems.First();
                if (scoreItem.Score != addedScore)
                {
                    comment.Score -= decimal.ToInt32(scoreItem.Score);
                    scoreItem.Score = addedScore;
                    comment.Score += addedScore;
                    db.Entry(scoreItem).State = EntityState.Modified;
                }
                else
                {
                    comment.Score -= addedScore;
                    db.Entry(scoreItem).State = EntityState.Deleted;
                }
            }
            db.Entry(comment).State = EntityState.Modified;
            db.SaveChanges();
            return Ok(new CommentDTO(comment));
        }

        // POST: api/Comments
        [ResponseType(typeof(CommentDTO))]
        public IHttpActionResult PostComment(Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            comment.ID = Guid.NewGuid();
            comment.Added = DateTime.Now;
            comment.UserID = User.Identity.GetUserId();

            db.Comments.Add(comment);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CommentExists(comment.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            db = new OutOfRangeEntities();
            comment = db.Comments.Find(comment.ID);

            Guid categoryId = Guid.Empty;
            if (comment.Answer != null)
            {
                categoryId = comment.Answer.Question.CategoryID;

            }
            else if (comment.Question != null)
            {
                categoryId = comment.Question.CategoryID;
            }

            PointsUtils.AddCreditsAndXP(comment.UserID, categoryId, 4, 7);

            return CreatedAtRoute("DefaultApi", new { id = comment.ID }, CommentDTO.FromEntity(comment));
        }

        // DELETE: api/Comments/5
        [ResponseType(typeof(Comment))]
        public IHttpActionResult DeleteComment(Guid id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }
            Guid categoryId = Guid.Empty;
            if (comment.Answer != null)
            {
                categoryId = comment.Answer.Question.CategoryID;

            }
            else if (comment.Question != null)
            {
                categoryId = comment.Question.CategoryID;
            }
                PointsUtils.AddCreditsAndXP(comment.UserID, categoryId, -30, 0);
            
                db.Comments.Remove(comment);
            db.SaveChanges();

            return Ok(comment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CommentExists(Guid id)
        {
            return db.Comments.Count(e => e.ID == id) > 0;
        }
    }
}