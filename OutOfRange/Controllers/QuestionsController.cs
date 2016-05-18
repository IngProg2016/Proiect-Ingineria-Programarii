using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using OutOfRange.Models;
using OutOfRange.Utils;

namespace OutOfRange.Controllers
{
    public class ItemScore
    {
        public Guid id { get; set; }
        public int score { get; set; }
    }
    [RoutePrefix("api/questions")]
    public class QuestionsController : ApiController
    {
        private OutOfRangeEntities db = new OutOfRangeEntities();

        // GET: api/Questions
        public JsonResult<IEnumerable<QuestionDTO>> GetQuestions()
        {
            return Json(db.Questions.ToList().Select(QuestionDTO.FromEntity));
        }

        // GET: api/Questions/5
        [ResponseType(typeof(QuestionDTO))]
        public IHttpActionResult GetQuestion(Guid id)
        {
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return NotFound();
            }
            string userId = User.Identity.GetUserId();
            var view = question.QuestionViews.Where(x => x.UserID == userId).ToList();
            if (view.Count == 0)
            {
                question.QuestionViews.Add(new QuestionView()
                {
                    UserID = userId,
                    Added = DateTime.Now,
                    LastVisit = DateTime.Now,
                });
                db.Entry(question).State=EntityState.Modified;
            }
            else
            {
                view.First().LastVisit = DateTime.Now;
                db.Entry(view.First()).State=EntityState.Modified;
            }
            db.SaveChanges();
            db=new OutOfRangeEntities();
            question = db.Questions.Find(id);

            QuestionDTO jsonQuestion=new QuestionDTO(question);
            var qScore = question.ScoreItems.Where(x => x.UserID == userId).ToList();
            if (qScore.Count > 0)
            {
                var score = qScore.First().Score;
                if (score != null) jsonQuestion.ScoreGiven = decimal.ToInt32(score.Value);
            }

            return Ok(jsonQuestion);
        }

        // PUT: api/Questions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutQuestion(Guid id, Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != question.ID)
            {
                return BadRequest();
            }

            
            db.Entry(question).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
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

        [HttpGet]
        [Route("Bounties")]
        public JsonResult<IEnumerable<QuestionDTO>> GetBounties()
        {
            return
                Json(
                    db.Questions.Where(question => question.Bounty > 0)
                        .OrderByDescending(x => x.Bounty)
                        .ToList()
                        .Select(QuestionDTO.FromEntity));
        }

        //POST: api/questions/AddScore
        [ResponseType(typeof(QuestionDTO))]
        [HttpPost]
        [Route("AddScore")]
        public IHttpActionResult AddScore(ItemScore score)
        {
            Question question = db.Questions.Find(score.id);
            string userId = User.Identity.GetUserId();
            var scoreitems = question.ScoreItems.Where(x => x.UserID == userId).ToList();
            if (scoreitems.Count == 0)
            {
                decimal addedScore = Math.Sign(score.score);
                question.ScoreItems.Add(new ScoreItem()
                {
                    Added = DateTime.Now,
                    Score = addedScore,
                    UserID = userId,
                    ItemID = question.ID,
                });

                if (question.Score.HasValue == false)
                    question.Score = 0;

                question.Score += addedScore;
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Ok(new QuestionDTO(question));
        }
        // POST: api/Questions
        /* Model Json
        {
            "Title":"Intrebarea no1",
            "Description":"Ceva text",
            "Tags": [
                {"Name":"c#","description":"ceva cu c#"},
                {"name":"valoare","description":"ceva valoros"}
            ]
        }
        */
        [ResponseType(typeof(QuestionDTO))]
        public IHttpActionResult PostQuestion(Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            List<Tag> removingTags=new List<Tag>();
            List<Tag> addingTags=new List<Tag>();
            
            foreach (var tag in question.Tags)
            {
                var currentTag = db.Tags.SingleOrDefault(x => x.Name.ToLower() == tag.Name.ToLower());
                if (currentTag==null)
                {
                    tag.ID = Guid.NewGuid();

                    //Tag newTag = new Tag()
                    //{
                    //    Name = tag.Name.ToLower(),
                    //    Description = tag.Description,
                    //    ID = Guid.NewGuid()
                    //};
                    //db.Tags.Add(newTag);
                }
                else
                {
                    removingTags.Add(tag);
                    addingTags.Add(currentTag);
                }
            }

            foreach (var removingTag in removingTags)
            {
                question.Tags.Remove(removingTag);
            }

            foreach (var addingTag in addingTags)
            {
                question.Tags.Add(addingTag);
            }

            string userId = User.Identity.GetUserId();

            question.ID=Guid.NewGuid();

            //guid hardcodat al unui user 
            if (User.Identity.IsAuthenticated)
                question.UserID = userId;
            else
                question.UserID = "9e03ab56-d1c8-460a-ad48-fa7c6e69bf18";

            question.Added=DateTime.Now;
            question.Modified=DateTime.Now;
            //Category ca sa mearga (bine ca nu's puse FK inca)
            db.Questions.Add(question);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                if (QuestionExists(question.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw e;
                }
            }
            PointsUtils.AddCreditsAndXP(userId, question.CategoryID, 10, 15);

            db=new OutOfRangeEntities();
            question = db.Questions.Find(question.ID);
            return CreatedAtRoute("DefaultApi", new { id = question.ID }, QuestionDTO.FromEntity(question));
        }
        
        // DELETE: api/Questions/5
        [ResponseType(typeof(QuestionDTO))]
        public IHttpActionResult DeleteQuestion(Guid id)
        {
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return NotFound();
            }
            PointsUtils.AddCreditsAndXP(question.UserID, question.CategoryID, -100, 0);
            db.Questions.Remove(question);
            db.SaveChanges();

            return Ok(new QuestionDTO(question));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuestionExists(Guid id)
        {
            return db.Questions.Count(e => e.ID == id) > 0;
        }
    }
}