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
    /*public class retT
    {
        public IQueryable<Team> ListTeam;
    }
    public class retTM
    {
        public IQueryable<TeamMember> ListTeam;
    }
    public class retP
    {
        public IQueryable<Project> ListProjects;
    }
    public class retu
    {
        public Team team;
    }*/
    public class TeamsController : ApiController
    {
        private ApiManagerContext db = new ApiManagerContext();

        // GET: api/Teams
        public retT GetTeams()
        {

            return new retT() {ListTeam =  db.Teams.AsQueryable<Team>()};
        }


        [Route("api/Teams/Users/{id}"), ResponseType(typeof(retTM))]
        public async Task<IHttpActionResult> GetTeamsUsers(int id)
        {

            var query = from tm in db.TeamMembers
                        where tm.IdTeam == id
                        select tm;
            IQueryable<TeamMember> ret = query.AsQueryable();
            if (ret.Count() == 0)
            {
                return NotFound();
            }

            return Ok(new retTM() { ListTeam = ret});
        }


        // GET: api/Teams/5
        [ResponseType(typeof(Team))]
        public async Task<IHttpActionResult> GetTeam(int id)
        {
            Team team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            return Ok(team);
        }

        // PUT: api/Teams/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTeam(int id, Team team)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != team.Id)
            {
                return BadRequest();
            }

            db.Entry(team).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
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

        // POST: api/Teams
        [ResponseType(typeof(Team))]
        public async Task<IHttpActionResult> PostTeam(Team team)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Teams.Add(team);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = team.Id }, team);
        }

        // DELETE: api/Teams/5
        [ResponseType(typeof(Team))]
        public async Task<IHttpActionResult> DeleteTeam(int id)
        {
            Team team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            db.Teams.Remove(team);
            await db.SaveChangesAsync();

            return Ok(team);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TeamExists(int id)
        {
            return db.Teams.Count(e => e.Id == id) > 0;
        }

        [Route("api/Teams/FindByProject/{id}"), ResponseType(typeof(retT))]
        public async Task<IHttpActionResult> GetTeamsProject(int id)
        {

            var query = from us in db.Teams
                        where us.IdProject == id
                        select us;
            IQueryable<Team> ret = query.AsQueryable();
            if (ret.Count() == 0)
            {
                return NotFound();
            }

            return Ok(new retT() {ListTeam = ret});
        }

    }
}