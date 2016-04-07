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

namespace OutOfRange.Controllers
{
    public class QuestionsController : ApiController
    {
        private OutOfRangeEntities db = new OutOfRangeEntities();

        // GET: api/Questions
        public JsonResult<IEnumerable<QuestionDTO>> GetQuestions()
        {
            //List<Question> questions=new List<Question>();
            //foreach (var question in db.Questions)
            //{
            //    questions.Add(new Question()
            //    {
            //        Added = question.Added,
            //        CategoryID = question.CategoryID,
            //        Category = question.Category,
            //        AspNetUser = question.AspNetUser,
            //        Description = question.Description,
            //        ID = question.ID,
            //        Modified = question.Modified,
            //        Title = question.Title,
            //        UserID = question.UserID,
            //        Tags = question.Tags.Select(tag => new Tag() {Name = tag.Name}).ToList()
            //    });
            //}
            return Json(db.Questions.ToList().Select(QuestionDTO.FromEntity));
        }

        // GET: api/Questions/5
        [ResponseType(typeof(Question))]
        public IHttpActionResult GetQuestion(Guid id)
        {
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
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

        // POST: api/Questions
        /* Model Json
        {"Title":"Intrebarea no1","Description":"Ceva text",
"Tags":
[ {"Name":"c#","description":"ceva cu c#"},{"name":"valoare","description":"ceva valoros"}
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
            

            question.ID=Guid.NewGuid();
            //guid hardcodat al unui user
            if (User.Identity.IsAuthenticated)
                question.UserID = User.Identity.GetUserId();
            else
                question.UserID = "9e03ab56-d1c8-460a-ad48-fa7c6e69bf18";
            question.Added=DateTime.Now;
            //Category ca sa mearga (bine ca nu's puse FK inca)
            question.CategoryID = Guid.Empty;
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
            db=new OutOfRangeEntities();
            question = db.Questions.Find(question.ID);
            return CreatedAtRoute("DefaultApi", new { id = question.ID }, QuestionDTO.FromEntity(question));
        }

        // DELETE: api/Questions/5
        [ResponseType(typeof(Question))]
        public IHttpActionResult DeleteQuestion(Guid id)
        {
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return NotFound();
            }
            db.Questions.Remove(question);
            db.SaveChanges();

            return Ok(question);
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