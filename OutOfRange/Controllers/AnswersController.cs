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
    public class AnswersController : ApiController
    {
        private OutOfRangeEntities db = new OutOfRangeEntities();

        // GET: api/Answers
        public JsonResult<IEnumerable<AnswerDTO>> GetAnswers()
        {
            return Json(db.Answers.ToList().Select(AnswerDTO.FromEntity));
        }
        //GET: api/AnswersQuestion/{GUID}
        [Route("api/AnswersQuestion/{id}")]
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

        // PUT: api/Answers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAnswer(Guid id, Answer answer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != answer.ID)
            {
                return BadRequest();
            }

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