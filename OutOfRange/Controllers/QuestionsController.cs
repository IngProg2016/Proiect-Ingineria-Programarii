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
            return Json(db.Questions.ToList().Select(question => QuestionDTO.FromEntity(question,1)));
        }
        
        // GET: api/Questions
        [Route("category/{id}")]
        public JsonResult<IEnumerable<QuestionDTO>> GetQuestionsCategory(Guid id)
        {
            return
                Json(
                    db.Questions.Where(question => question.CategoryID == id)
                        .ToList()
                        .Select(question => QuestionDTO.FromEntity(question)));
        }

        // GET: api/Questions
        [Route("tag/{tag}")]
        public JsonResult<IEnumerable<QuestionDTO>> GetQuestionsTag(string tag)
        {
            return
                Json(
                    db.Questions.Where(question => question.Tags.Select(x => x.Name).Contains(tag))
                        .ToList()
                        .Select(question => QuestionDTO.FromEntity(question)));
        }

        [Route("search/{q}")]
        public JsonResult<IEnumerable<QuestionDTO>> GetSearch(string q)
        {
            var searchResult = db.Search(q);
            var en=searchResult.GetEnumerator();
            List<QuestionDTO> results=new List<QuestionDTO>();
            while (en.MoveNext())
            {
                results.Add(QuestionDTO.FromEntity(en.Current));    
            }
            return Json(results.AsEnumerable());
        }

        // GET: api/Questions/5
        [ResponseType(typeof (QuestionDTO))]
        public IHttpActionResult GetQuestion(Guid id)
        {
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return NotFound();
            }

            string userId="";
            if (User.Identity.IsAuthenticated)
            {
                userId = User.Identity.GetUserId();
                var view = question.QuestionViews.Where(x => x.UserID == userId).ToList();
                if (view.Count == 0)
                {
                    question.QuestionViews.Add(new QuestionView()
                    {
                        UserID = userId,
                        Added = DateTime.Now,
                        LastVisit = DateTime.Now,
                    });
                    db.Entry(question).State = EntityState.Modified;
                }
                else
                {
                    view.First().LastVisit = DateTime.Now;
                    db.Entry(view.First()).State = EntityState.Modified;
                }
            }
            
            db.SaveChanges();
            db = new OutOfRangeEntities();
            question = db.Questions.Find(id);
            QuestionDTO jsonQuestion = new QuestionDTO(question);
            if (User.Identity.IsAuthenticated)
            {
                var qScore = question.ScoreItems.Where(x => x.UserID == userId).ToList();
                if (qScore.Count > 0)
                {
                    var score = qScore.First().Score;
                    jsonQuestion.ScoreGiven = decimal.ToInt32(score);
                }
                foreach (var comment in jsonQuestion.Comments)
                {
                    var comm = db.Comments.Find(comment.ID);
                    var qComScore = comm.ScoreItems.Where(x => x.UserID == userId).ToList();
                    if (qComScore.Count > 0)
                    {
                        var score = qComScore.First().Score;
                        comment.ScoreGiven = decimal.ToInt32(score);
                    }
                }
                foreach (var answer in jsonQuestion.Answers)
                {
                    var answ = db.Answers.Find(answer.ID);
                    var qAnswScore = answ.ScoreItems.Where(x => x.UserID == userId).ToList();
                    if (qAnswScore.Count > 0)
                    {
                        var score = qAnswScore.First().Score;
                        answer.ScoreGiven = decimal.ToInt32(score);
                    }
                    foreach (var comment in answer.Comments)
                    {
                        var comm = db.Comments.Find(comment.ID);
                        var qComScore = comm.ScoreItems.Where(x => x.UserID == userId).ToList();
                        if (qComScore.Count > 0)
                        {
                            var score = qComScore.First().Score;
                            comment.ScoreGiven = decimal.ToInt32(score);
                        }
                    }
                }
            }
            int viewsNumber = question.QuestionViews.Count();
            if (viewsNumber == 100)
            {
                PointsUtils.giveBadge(question.AspNetUser.Id, question.CategoryID, "views", 0, question.ID);
            }
            else if (viewsNumber == 300)
            {
                PointsUtils.giveBadge(question.AspNetUser.Id, question.CategoryID, "views", 1, question.ID);
            }
            else if (viewsNumber == 500)
            {
                PointsUtils.giveBadge(question.AspNetUser.Id, question.CategoryID, "views", 2, question.ID);
            }
            else if (viewsNumber == 1000)
            {
                PointsUtils.giveBadge(question.AspNetUser.Id, question.CategoryID, "views", 3, question.ID);
            }

            return Ok(jsonQuestion);
        }

        public class UpdateQuestion
        {
            public string QuestionBody { get; set; }
        }
        // PUT: api/Questions/5
        [ResponseType(typeof(void))]
        [HttpPut]
        public IHttpActionResult PutQuestion(Guid id, UpdateQuestion newQuestion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var question = db.Questions.Find(id);

            question.QuestionBody = newQuestion.QuestionBody;
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
                        .Select(question => QuestionDTO.FromEntity(question)));
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
                int addedScore = Math.Sign(score.score);
                question.ScoreItems.Add(new ScoreItem()
                {
                    Added = DateTime.Now,
                    Score = addedScore,
                    UserID = userId,
                    ItemID = question.ID,
                });

                question.Score += addedScore;
            }
            else
            {
                int addedScore = Math.Sign(score.score);
                var scoreItem = scoreitems.First();
                if (scoreItem.Score != addedScore)
                {
                    question.Score -= scoreItem.Score;
                    scoreItem.Score = addedScore;
                    question.Score += addedScore;
                    db.Entry(scoreItem).State = EntityState.Modified;
                }
                else
                {
                    question.Score -= scoreItem.Score;
                    db.Entry(scoreItem).State = EntityState.Deleted;
                }
            }
            db.Entry(question).State = EntityState.Modified;
            db.SaveChanges();

            int currentScore = question.Score;
            int currentBadgeRarity = PointsUtils.getCurrentScoreBadgeRarity(currentScore);

            UserLevel userLevel = question.AspNetUser.UserLevels.Single(x => x.CategoryID == question.CategoryID);
            UserBadge userBadge = userLevel.UserBadges.SingleOrDefault(x => x.ItemID == question.ID);
            if (userBadge == null)
            {
                PointsUtils.giveBadge(question.AspNetUser.Id, question.CategoryID, "score", 0, question.ID);
            }
            else if (userBadge.Badge.Rarity == currentBadgeRarity)
            {
                db.UserBadges.Remove(userBadge);
                PointsUtils.giveBadge(question.AspNetUser.Id, question.CategoryID, "score", currentBadgeRarity + 1, question.ID);
            }

            db.SaveChanges();
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

            string userId = User.Identity.GetUserId();
            AspNetUser user = db.AspNetUsers.Single(x => x.Id == userId);
            if (question.Bounty > user.Credits)
            {
                return BadRequest("Cannot add a bounty with more points than you have");
            }
            user.Credits -= question.Bounty;
            db.Entry(user).State = EntityState.Modified;
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

            int questionsNumber = user.Questions.Where(x => x.CategoryID == question.CategoryID).Count();
            int badgeRarity = PointsUtils.getQuestionBadgeRarity(questionsNumber);

            if (badgeRarity != -1)
            {
                PointsUtils.giveBadge(userId, question.CategoryID, "question", badgeRarity, question.ID);
            } 

            db =new OutOfRangeEntities();
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