using Adventure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Adventure.Controllers;
using Adventure.Services;

namespace Adventure.Controllers
{
    public class TestController : ApiController
    {
        public HttpResponseMessage Get()
        {
            int userId = 6;
            var adventureContext = new AdventureContext();
            var badgeFinder = new BadgeFinder( adventureContext);
            badgeFinder.VerifyBadges( userId );
            return null;
        }
    }
}