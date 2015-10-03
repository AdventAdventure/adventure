using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Adventure.Controllers
{
    public class UsersController : ApiController
    {
        public HttpResponseMessage Get(string userName = "")
        {
            object result = null;
            using (var db = new AdventureContext())
            {
                if (string.IsNullOrEmpty(userName))
                {
                    var users = db.Users.Select(user => new { Name = user.UserName, Id = user.TwitterId });
                    if (users.Any())
                    {
                        result = users;
                    }
                    else
                    {
                        result = new {Error = "User not found"};
                    }
                }
                else
                {
                    var user = db.Users.FirstOrDefault(u => u.UserName == userName);
                    if (user == null) return Request.CreateResponse(HttpStatusCode.OK, result);

                    var responses = db.Responses
                        .Where(c => c.UserId == user.UserId);
                    var challenges = db.Challenges
                        .Where(c => responses.Select(r => r.ChallengeId).Contains(c.ChallengeId));
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
    }
}