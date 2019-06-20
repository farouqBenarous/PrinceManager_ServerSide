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
    public class retTa
    {
        public IQueryable<Tasks> ListTasks;
    }
    public class TasksController : ApiController
    {
        private ApiManagerContext db = new ApiManagerContext();

        // GET: api/Tasks
        public IQueryable<Tasks> GetTasks()
        {
            return db.Tasks.
                Include(b => b.Responsible).
                Include(b => b.Assigned).
                Include(b => b.Team);
        }

        // GET: api/Tasks/5
        [ResponseType(typeof(Tasks))]
        public async Task<IHttpActionResult> GetTasks(int id)
        {
            Tasks tasks = await db.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }

            return Ok(tasks);
        }

        // PUT: api/Tasks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTasks(int id, Tasks tasks)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tasks.Id)
            {
                return BadRequest();
            }

            db.Entry(tasks).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TasksExists(id))
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

        // POST: api/Tasks
        [ResponseType(typeof(Tasks))]
        public async Task<IHttpActionResult> PostTasks(Tasks tasks)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tasks.Add(tasks);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = tasks.Id }, tasks);
        }

        // DELETE: api/Tasks/5
        [ResponseType(typeof(Tasks))]
        public async Task<IHttpActionResult> DeleteTasks(int id)
        {
            Tasks tasks = await db.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }

            db.Tasks.Remove(tasks);
            await db.SaveChangesAsync();

            return Ok(tasks);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TasksExists(int id)
        {
            return db.Tasks.Count(e => e.Id == id) > 0;
        }

        [Route("api/Tasks/FindByTeam/{id}"), ResponseType(typeof(retTa))]
        public async Task<IHttpActionResult> GetTasksTeam(int id)
        {

            var query = from us in db.Tasks 
                        where us.IdTeam == id 
                        select us;
            IQueryable<Tasks> ret = query.AsQueryable();
            if (ret.Count() == 0)
            {
                return NotFound();
            }
            retTa p = new retTa();
            p.ListTasks = ret;
            return Ok(p);
        }

        [Route("api/Tasks/FindByTeamAssigned/{id}/{idU}"), ResponseType(typeof(IQueryable<Tasks>))]
        public async Task<IHttpActionResult> GetTasksTeamUser(int id, int idU)
        {

            var query = from us in db.Tasks join tm in db.TeamMembers on us.IdAssigned equals tm.Id
                        where us.IdTeam == id && tm.IdUser == idU
                        select us;
            IQueryable<Tasks> ret = query.AsQueryable();
            if (ret.Count() == 0)
            {
                return NotFound();
            }

            retTa a = new retTa() { ListTasks = ret };
            return Ok(a);
        }

    }
}