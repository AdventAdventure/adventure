using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Adventure.Controllers
{
    public class UsersController : ApiController
    {
        [Route("api/user/{userName}")]
        public HttpResponseMessage Get(string userName = "")
        {
            object result;
            using (var db = new AdventureContext())
            {
                if (string.IsNullOrEmpty(userName))
                {
                    result = List(db);
                }
                else
                {
                    var user = db.Users.FirstOrDefault(u => u.UserName == userName);
                    if (user == null) return Request.CreateResponse(HttpStatusCode.OK, (object) null);

                    var responses = db.Responses
                        .Where(c => c.UserId == user.UserId)
                        .ToArray();
                    var responseChallenges = responses.Select(r => r.ChallengeId);
                    var challenges = db.Challenges
                        .Where(c => responseChallenges
                        .Contains(c.ChallengeId))
                        .ToArray();
                    result = new
                             {
                                 User = user,
                                 Responses = responses,
                                 Challenges = challenges
                             };
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, result, new JsonMediaTypeFormatter());
        }

        [Route("api/users")]
        [HttpGet]
        public HttpResponseMessage List()
        {
            object result;
            using (var db = new AdventureContext())
            {
                result = List(db);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result, new JsonMediaTypeFormatter());
        }

        private static object List(AdventureContext db)
        {
            object result;
            var users = db.Users.Select(user => new {Name = user.UserName, Id = user.TwitterId})
                .ToArray();
            if (users.Any())
            {
                result = users;
            }
            else
            {
                result = new {Error = "User not found"};
            }
            return result;
        }
    }
}