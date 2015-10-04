using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Adventure.Models;

namespace Adventure.Controllers
{
    public class DaysController : ApiController
    {
        // GET api/days
        public HttpResponseMessage Get()
        {
            IEnumerable<object> challenges;
            using (var db = new AdventureContext())
            {
                if (!db.Challenges.Any())
                {
                    var days = Enumerable
                        .Range(1, 24)
                        .Select(n => new
                        {
                            Day = n,
                            Challenge = new Challenge
                            {
                                Name = "Challenge for December " + n,
                                ChallengeNumber = n
                            }
                        });

                    foreach (var day in days)
                    {
                        db.Challenges.Add(day.Challenge);
                    }
                    db.SaveChanges();
                }
                challenges = db.Challenges
                    .ToList()
                    .OrderBy(d => d.ChallengeNumber)
                    .Select(c => 
                        (object) new 
                        {
                            Day = c.ChallengeNumber,
                            Challenge = c
                        });
            }
            return Request.CreateResponse(HttpStatusCode.OK, challenges, new JsonMediaTypeFormatter());
        }
    }
}
