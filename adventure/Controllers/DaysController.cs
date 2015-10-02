using System;
using System.Collections.Generic;
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
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Days = new[] {
                            new {Day = 1 },
                            new {Day = 2}
                   }
            });
        }
    }
}
