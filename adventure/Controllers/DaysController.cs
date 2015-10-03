using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Adventure.Controllers
{
    public class DaysController : ApiController
    {
        // GET api/days
        public HttpResponseMessage Get()
        {
            var result = new
            {
                Days = Enumerable
                    .Range(1, 24)
                    .Select(n => new
                    {
                        Day = n,
                        Challenges = new[]
                        {
                            new { Challenge = new
                                              {
                                                  Title = "Challenge for November " + n,
                                                  Hashtag = "#AdventureDay" + n
                                              }
                            },
                            new {Challenge = new
                                             {
                                                 Title = "Bonus challenge for December " + n,
                                                 Hashtag = "#AdventureDay" + n + "_Bonus1"
                                             }}
                        }
                    })
            };
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
