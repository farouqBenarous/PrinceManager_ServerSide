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
    public class ProjectsController : ApiController
    {
        private ApiManagerContext db = new ApiManagerContext();

        // GET: api/Projects
        public IQueryable<Project> GetProjects()
        {
            return db.Project
                .Include(b => b.User); 
        }

        [Route("api/Projects/Teams/{id}"), ResponseType(typeof(retT))]
        public async Task<IHttpActionResult> GetProjectTeams(int id)
        {

            var query = from tm in db.Teams
                        where tm.IdProject == id
                        select tm;
            IQueryable<Team> ret = query.AsQueryable();
            if (ret.Count() == 0)
            {
                return NotFound();
            }
            retT p = new retT();
            p.ListTeam = ret;
            return Ok(p);
        }


        // GET: api/Projects/5
        [Route("api/Projects/FindByOwner/{id}"),ResponseType(typeof (retP))]
        public async Task<IHttpActionResult> GetProjectsOwner(int id)
        {

            var query = from us in db.Project
                        where us.IdOwner == id
                        select us;
            IQueryable<Project> ret = query.AsQueryable();
            if (ret.Count() == 0)
            {
                return NotFound();
            }
            retP p = new retP();
            p.ListProjects = ret;
            return Ok(p);
        }


        // GET: api/Projects/5
        [ResponseType(typeof(Project))]
        public async Task<IHttpActionResult> GetProject(int id)
        {
            Project project = await db.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        // PUT: api/Projects/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProject(int id, Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != project.Id)
            {
                return BadRequest();
            }

            db.Entry(project).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // POST: api/Projects
        [ResponseType(typeof(Project))]
        public async Task<IHttpActionResult> PostProject(Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Project.Add(project);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = project.Id }, project);
        }

        // DELETE: api/Projects/5
        [ResponseType(typeof(Project))]
        public async Task<IHttpActionResult> DeleteProject(int id)
        {
            Project project = await db.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            db.Project.Remove(project);
            await db.SaveChangesAsync();

            return Ok(project);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProjectExists(int id)
        {
            return db.Project.Count(e => e.Id == id) > 0;
        }
    }
}