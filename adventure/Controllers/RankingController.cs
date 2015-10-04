using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Adventure.Models;

namespace Adventure.Controllers
{
    public class RankingController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var result = new Ranking();

            using (var context = new AdventureContext())
            {
                var t = context.Users.First();

                result.Positions = ( from r in context.UserChallenges.Where( x => x.IsComplete == true)
                                    from c in context.Challenges.Where(x => x.ChallengeId == r.ChallengeId)
                                    from u in context.Users.Where(x => x.UserId == r.UserId)
                                    group c by new { u.UserName } into g
                                    select new Position
                                    {
                                        Points = g.Sum(x => x.Value) ?? 0,
                                        UserName = g.Key.UserName
                                    }).OrderByDescending(x => x.Points).ToList();
            }

            if (result.Positions.Count() > 1)
                return Request.CreateResponse(HttpStatusCode.OK, result);

            return Request.CreateResponse(HttpStatusCode.OK, new Ranking());
        }
    }
}
