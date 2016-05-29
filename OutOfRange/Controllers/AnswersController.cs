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
    [RoutePrefix("api/answers")]
    public class AnswersController : ApiController
    {
        private OutOfRangeEntities db = new OutOfRangeEntities();

        // GET: api/Answers
        public JsonResult<IEnumerable<AnswerDTO>> GetAnswers()
        {
            return Json(db.Answers.ToList().Select(AnswerDTO.FromEntity));
        }
        //GET: api/AnswersQuestion/{GUID}
        [Route("AnswersQuestion/{id}")]
        public JsonResult<IEnumerable<AnswerDTO>> GetAnswersQuestion(Guid id)
        {
            return Json(db.Answers.Where(x=>x.QuestionID==id).ToList().Select(AnswerDTO.FromEntity));
        }

        // GET: api/Answers/5
        [ResponseType(typeof(AnswerDTO))]
        public IHttpActionResult GetAnswer(Guid id)
        {
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return NotFound();
            }

            return Ok(new AnswerDTO(answer));
        }

        public class UpdateAnwser
        {
            public string AnswerBody { get; set; }
        }
        // PUT: api/Answers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAnswer(Guid id,UpdateAnwser newAnswer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var answer = db.Answers.Find(id);


            answer.AnswerBody = newAnswer.AnswerBody;
            db.Entry(answer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnswerExists(id))
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

        //POST: api/answers/Accept/:id
        [ResponseType(typeof(AnswerDTO))]
        [HttpPost]
        [Route("Accept/{id}")]
        public IHttpActionResult Accept(Guid id)
        {
            Answer answer = db.Answers.Find(id);
            Question question = answer.Question;

            if (answer.Accepted==false)
            {
                var questionHasAnwser = question.Answers.Any(x => x.Accepted);
                if (!questionHasAnwser)
                {
                    string userId = User.Identity.GetUserId();
                    if (question.UserID != userId)
                        return Unauthorized();

                    answer.AspNetUser.Credits += question.Bounty;
                    answer.Accepted = true;
                    db.Entry(answer.AspNetUser).State = EntityState.Modified;
                    db.Entry(answer).State = EntityState.Modified;
                    PointsUtils.AddCreditsAndXP(answer.AspNetUser.Id,answer.Question.CategoryID,25,40);
                    db.SaveChanges();
                }
            }

            return Ok(new AnswerDTO(answer));
        }


        //POST: api/answers/AddScore
        [ResponseType(typeof(AnswerDTO))]
        [HttpPost]
        [Route("AddScore")]
        public IHttpActionResult AddScore(ItemScore score)
        {
            Answer answer = db.Answers.Find(score.id);
            string userId = User.Identity.GetUserId();
            var scoreitems = answer.ScoreItems.Where(x => x.UserID == userId).ToList();
            if (scoreitems.Count == 0)
            {
                int addedScore = Math.Sign(score.score);
                answer.ScoreItems.Add(new ScoreItem()
                {
                    Added = DateTime.Now,
                    Score = addedScore,
                    UserID = userId,
                    ItemID = answer.ID,
                });

                answer.Score += addedScore;
            }
            else
            {
                int addedScore = Math.Sign(score.score);
                var scoreItem = scoreitems.First();
                if (scoreItem.Score != addedScore)
                {
                    answer.Score -= decimal.ToInt32(scoreItem.Score);
                    scoreItem.Score = addedScore;
                    answer.Score += addedScore;
                    db.Entry(scoreItem).State = EntityState.Modified;
                }
                else
                {
                    answer.Score -= addedScore;
                    db.Entry(scoreItem).State = EntityState.Deleted;
                }
            }
            db.Entry(answer).State = EntityState.Modified;
            db.SaveChanges();


            if (answer.Score == 10)
            {
                UserLevel userLevel = answer.AspNetUser.UserLevels.Single(x => x.CategoryID == answer.Question.CategoryID);
                UserBadge userBadge = userLevel.UserBadges.SingleOrDefault(x => x.ItemID == answer.ID);
                if (userBadge == null)
                {
                    PointsUtils.giveBadge(answer.AspNetUser.Id, answer.Question.CategoryID, "score", 0, answer.ID);
                }
            }
            else if (answer.Score == 50)
            {
                UserLevel userLevel = answer.AspNetUser.UserLevels.Single(x => x.CategoryID == answer.Question.CategoryID);
                UserBadge userBadge = userLevel.UserBadges.SingleOrDefault(x => x.ItemID == answer.ID);
                if (userBadge.Badge.Rarity == 0)
                {
                    PointsUtils.giveBadge(answer.AspNetUser.Id, answer.Question.CategoryID, "score", 1, answer.ID);
                    db.UserBadges.Remove(userBadge);
                }
            }
            else if (answer.Score == 100)
            {
                UserLevel userLevel = answer.AspNetUser.UserLevels.Single(x => x.CategoryID == answer.Question.CategoryID);
                UserBadge userBadge = userLevel.UserBadges.SingleOrDefault(x => x.ItemID == answer.ID);
                if (userBadge.Badge.Rarity == 1)
                {
                    PointsUtils.giveBadge(answer.AspNetUser.Id, answer.Question.CategoryID, "score", 2, answer.ID);
                    db.UserBadges.Remove(userBadge);
                }
            }
            else if (answer.Score == 500)
            {
                UserLevel userLevel = answer.AspNetUser.UserLevels.Single(x => x.CategoryID == answer.Question.CategoryID);
                UserBadge userBadge = userLevel.UserBadges.SingleOrDefault(x => x.ItemID == answer.ID);
                if (userBadge.Badge.Rarity == 2)
                {
                    PointsUtils.giveBadge(answer.AspNetUser.Id, answer.Question.CategoryID, "score", 3, answer.ID);
                    db.UserBadges.Remove(userBadge);
                }
            }
            db.SaveChanges();
            return Ok(new AnswerDTO(answer));
        }

        // POST: api/Answers
        [Authorize]
        [ResponseType(typeof(AnswerDTO))]
        public IHttpActionResult PostAnswer(Answer answer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            answer.ID = Guid.NewGuid();
            answer.Added=DateTime.Now;
            answer.UserID = User.Identity.GetUserId();

            db.Answers.Add(answer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (AnswerExists(answer.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
           
            db=new OutOfRangeEntities();
            answer = db.Answers.Find(answer.ID);

            Category category = db.Categories.Single(x => x.ID == answer.Question.CategoryID);
            PointsUtils.AddCreditsAndXP(answer.UserID, category.ID, 10, 15);

            return CreatedAtRoute("DefaultApi", new { id = answer.ID }, AnswerDTO.FromEntity(answer));
        }

        // DELETE: api/Answers/5
        [ResponseType(typeof(AnswerDTO))]
        public IHttpActionResult DeleteAnswer(Guid id)
        {
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return NotFound();
            }
            Category category = db.Categories.Single(x => x.ID == answer.Question.CategoryID);
            PointsUtils.AddCreditsAndXP(answer.UserID, category.ID, -50, 0);

            db.Answers.Remove(answer);
            db.SaveChanges();

            return Ok(new AnswerDTO(answer));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AnswerExists(Guid id)
        {
            return db.Answers.Count(e => e.ID == id) > 0;
        }
    }
}