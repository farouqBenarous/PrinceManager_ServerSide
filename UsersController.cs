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

namespace ApiManager.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class retT
    {
        public IQueryable<TeamMemberDTO> ListTeam;
    }
    public class retP
    {
        public IQueryable<Project> ListProjects;
    }
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class Login {
       public string username { get; set; }
       public string password { get; set; }
    }

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UsersController : ApiController
    {
        private ApiManagerContext db = new ApiManagerContext();

        // GET: api/Users
        [HttpGet]
        public IQueryable<User> GetUsers()
        {
            return db.User;
        }

        // GET: api/Users/5
        //[Route("api/users"), ResponseType(typeof(User))]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            User users = await db.User.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }


        [Route("api/Users/Projects/{id}"), ResponseType(typeof(IQueryable<Project>))]
        public async Task<IHttpActionResult> GetProjectsByUser(int id)
        {


            var query = from p in db.Project
                        where p.IdOwner == id
                        select p;
            var s = query.AsQueryable<Project>();
            retP r = new retP();
            r.ListProjects = s;
            if (s == null)
            {
                return NotFound();
            }

            return Ok(r);
        }

        [Route("api/Users/Teams/{id}"), ResponseType(typeof(retT))]
        public async Task<IHttpActionResult> GetTeamsByUser(int id)
        {

            var query = from tm in db.TeamMembers
                        join te in db.Teams on tm.IdTeam equals te.Id
                        where tm.IdUser == id
                        select new TeamMemberDTO()
                        {
                            Id = tm.Id,
                            IdTeam = tm.IdTeam,
                            IdUser = tm.IdUser,
                            CreationDate = te.CreationDate,
                            Description = te.Description,
                            IdProject = te.IdProject,
                            IdTeamLeader = te.IdTeamLeader,

                            Name = te.Name
                        };
            if (query.Count() == 0)
            {
                return NotFound();
            }
            retT a = new retT();
            a.ListTeam = query;
            

            return Ok(a);
        }


        

        [Route("api/Login"), ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUsers(Login l)
        {

            var query = from us in db.User
                        where us.Username == l.username && us.Password == l.password
                        select us;
            User users = query.FirstOrDefault();


            if (users == null)
            {
                return NotFound();
            }
            
            return Ok(users);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(int id, User user)
        {
            user = Completenulss(user);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.User.Add(user);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                var c = e.InnerException.InnerException.Message.Contains("username") ? "username" : "email";
                var resp = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(string.Format("This email or username already exists" )),
                    ReasonPhrase = "Forbbiden"
                };
                throw new HttpResponseException(resp);

            }


            var a = CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
            return a;
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            User user = await db.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.User.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        User Completenulss(User user)
        {
            User u = db.User.Find(user.Id);
            user.Name = user.Name == null ? u.Name : user.Name;
            user.Password = user.Password == null ? u.Password : user.Password;
            user.Phone = user.Phone == null ? u.Phone : user.Phone;
            user.Username = user.Username == null ? u.Username : user.Username;
            user.BornDate = user.BornDate == null ? u.BornDate : user.BornDate;
            user.Email = user.Email == null ? u.Email : user.Email;
            user.Gender = user.Gender == null ? u.Gender : user.Gender;
            db.Entry(u).State = EntityState.Detached;
            return user;

        }


        private bool UserExists(int id)
        {
            return db.User.Count(e => e.Id == id) > 0;
        }
    }
}