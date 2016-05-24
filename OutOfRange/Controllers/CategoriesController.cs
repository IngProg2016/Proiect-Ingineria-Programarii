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
using OutOfRange.Models;

namespace OutOfRange.Controllers
{
    public class CategoriesController : ApiController
    {
        private OutOfRangeEntities db = new OutOfRangeEntities();

        //GET: api/Categories
        public JsonResult<IEnumerable<CategoryDTO>> GetCategories()
        {
            return Json(db.Categories.ToList().Select(CategoryDTO.FromEntity));
        }

        // GET: api/Categories/5
        [ResponseType(typeof(CategoryDTO))]
        public IHttpActionResult GetCategory(Guid id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(new CategoryDTO(category));
        }

        

        // PUT: api/Categories/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCategory(Guid id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != category.ID)
            {
                return BadRequest();
            }

            db.Entry(category).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        [ResponseType(typeof(CategoryDTO))]
        public IHttpActionResult PostCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            category.ID = Guid.NewGuid();
            db.Categories.Add(category);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CategoryExists(category.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = category.ID }, new CategoryDTO(category));
        }

        // DELETE: api/Categories/5
        [ResponseType(typeof(Category))]
        public IHttpActionResult DeleteCategory(Guid id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            return Ok(category);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoryExists(Guid id)
        {
            return db.Categories.Count(e => e.ID == id) > 0;
        }
    }
}