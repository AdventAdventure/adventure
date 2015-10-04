using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Adventure.Controllers
{
    public class ChallengeController : ApiController
    {
        [Route("api/challenge/{day}")]
        public HttpResponseMessage Get(int day)
        {
            using (var db = new AdventureContext())
            {
                var challenge = db.Challenges.FirstOrDefault(d => d.ChallengeNumber == day);
                if (challenge == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Challenge not found", new JsonMediaTypeFormatter());
                }
                var responses = db.Responses.Where(r => r.ChallengeId == challenge.ChallengeId)
                    .ToList();
                var userIds = responses.Select(r => r.UserId);
                var users = db.Users.Where(u => userIds.Contains(u.UserId)).ToList();
                var result = responses.Select(r => new
                                      {
                                          Tweet = r.Tweet,
                                          TweetId = r.TweetId,
                                          User = users.First(u => u.UserId == r.UserId).ScreenName
                                      })
                    .ToArray();
                return Request.CreateResponse(HttpStatusCode.OK, result, new JsonMediaTypeFormatter());
            }
        }
    }
}
