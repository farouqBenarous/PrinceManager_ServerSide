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
        public class retT
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

    public class TeamMembersController : ApiController
    {
        private ApiManagerContext db = new ApiManagerContext();
        // GET: api/TeamMembers
        public retTM GetTeamMembers()
        {
            retTM a = new retTM();
            a.ListTeam = db.TeamMembers.AsQueryable<TeamMember>();
            return a;/*.
                Include(b => b.User).
                Include(b => b.Team);*/
        }

        // GET: api/TeamMembers/5
        [ResponseType(typeof(TeamMember))]
        public async Task<IHttpActionResult> GetTeamMember(int id)
        {
            TeamMember teamMember = await db.TeamMembers.FindAsync(id);
            if (teamMember == null)
            {
                return NotFound();
            }

            return Ok(teamMember);
        }

        // PUT: api/TeamMembers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTeamMember(int id, TeamMember teamMember)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != teamMember.Id)
            {
                return BadRequest();
            }

            db.Entry(teamMember).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamMemberExists(id))
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

        // POST: api/TeamMembers
        [ResponseType(typeof(TeamMember))]
        public async Task<IHttpActionResult> PostTeamMember(TeamMember teamMember)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TeamMembers.Add(teamMember);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = teamMember.Id }, teamMember);
        }

        // DELETE: api/TeamMembers/5
        [ResponseType(typeof(TeamMember))]
        public async Task<IHttpActionResult> DeleteTeamMember(int id)
        {
            TeamMember teamMember = await db.TeamMembers.FindAsync(id);
            if (teamMember == null)
            {
                return NotFound();
            }

            db.TeamMembers.Remove(teamMember);
            await db.SaveChangesAsync();

            return Ok(teamMember);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TeamMemberExists(int id)
        {
            return db.TeamMembers.Count(e => e.Id == id) > 0;
        }
    }
}