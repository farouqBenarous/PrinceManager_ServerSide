using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using ApiManager.Models;


namespace BookService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TaskFieldsController : ApiController
    {
        private ApiManagerContext db = new ApiManagerContext();

        // GET: api/TaskFields
        public IQueryable<TaskField> GetTaskFields()
        {
            return db.TaskFields;
        }

        // GET: api/TaskFields/5
        [ResponseType(typeof(TaskField))]
        public async Task<IHttpActionResult> GetTaskField(int id)
        {
            TaskField taskField = await db.TaskFields.FindAsync(id);
            if (taskField == null)
            {
                return NotFound();
            }

            return Ok(taskField);
        }

        // PUT: api/TaskFields/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTaskField(int id, TaskField taskField)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != taskField.Id)
            {
                return BadRequest();
            }

            db.Entry(taskField).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskFieldExists(id))
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

        // POST: api/TaskFields
        [ResponseType(typeof(TaskField))]
        public async Task<IHttpActionResult> PostTaskField(TaskField taskField)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TaskFields.Add(taskField);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = taskField.Id }, taskField);
        }

        // DELETE: api/TaskFields/5
        [ResponseType(typeof(TaskField))]
        public async Task<IHttpActionResult> DeleteTaskField(int id)
        {
            TaskField taskField = await db.TaskFields.FindAsync(id);
            if (taskField == null)
            {
                return NotFound();
            }

            db.TaskFields.Remove(taskField);
            await db.SaveChangesAsync();

            return Ok(taskField);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TaskFieldExists(int id)
        {
            return db.TaskFields.Count(e => e.Id == id) > 0;
        }
    }
}