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
            Dictionary<int, IEnumerable<Challenge>> challenges;
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
                                Name = "Challenge for November " + n,
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
                    .GroupBy(d => d.ChallengeNumber)
                    .ToDictionary(g => g.Key, g => g.Select(c => c));
            }
            return Request.CreateResponse(HttpStatusCode.OK, challenges, new JsonMediaTypeFormatter());
        }
    }
}
