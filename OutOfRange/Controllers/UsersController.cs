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

namespace OutOfRange.Controllers
{
    [RoutePrefix("api/user")]
    public class UsersController : ApiController
    {
        private OutOfRangeEntities db = new OutOfRangeEntities();

        [Route("userdata/")]
        public object GetCurentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                string id = User.Identity.GetUserId();
                AspNetUser user = db.AspNetUsers.Find(id);
                return Json(new { id = id, roles = user.AspNetRoles.Select(x => x.Name) });
            }
            return null;
        }

        // GET: api/Users
        public JsonResult<IEnumerable<AspNetUserDTO>> GetAspNetUsers()
        {
            return Json(db.AspNetUsers.ToList().Select(AspNetUserDTO.FromEntity));
        }

        [Route("profile/")]
        [ResponseType(typeof(AspNetUserDTO))]
        public IHttpActionResult GetAspNetUser()
        {
            var userId = User.Identity.GetUserId();
            AspNetUser aspNetUser = db.AspNetUsers.FirstOrDefault(user => user.Id == userId);

            if (aspNetUser == null)
            {
                return NotFound();
            }
            return Ok(AspNetUserDTO.FromEntity(aspNetUser));
        }

        // GET: api/Users/5
        [Route("profile/{id}")]
        [ResponseType(typeof(AspNetUserDTO))]
        public IHttpActionResult GetAspNetUser(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            
            if (aspNetUser == null)
            {
                return NotFound();
            }

            return Ok(AspNetUserDTO.FromEntity(aspNetUser));
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAspNetUser(string id, AspNetUser aspNetUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != aspNetUser.Id)
            {
                return BadRequest();
            }

            db.Entry(aspNetUser).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetUserExists(id))
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

        // POST: api/Users
        [ResponseType(typeof(AspNetUser))]
        public IHttpActionResult PostAspNetUser(AspNetUser aspNetUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AspNetUsers.Add(aspNetUser);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (AspNetUserExists(aspNetUser.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = aspNetUser.Id }, aspNetUser);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(AspNetUser))]
        public IHttpActionResult DeleteAspNetUser(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            db.AspNetUsers.Remove(aspNetUser);
            db.SaveChanges();

            return Ok(aspNetUser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AspNetUserExists(string id)
        {
            return db.AspNetUsers.Count(e => e.Id == id) > 0;
        }
    }
}