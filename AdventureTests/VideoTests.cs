using System.Collections.Generic;
using Adventure.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventureTests
{
    [TestClass]
    public class VideoTests
    {
        [TestMethod]
        public void Can_determine_submit_hashtag_missing()
        {
            // Execute
            var isMissing = TweetParser
                .IsSubmitHashtagMissing(new List<string>
                {
                    "Not",
                    "Present"
                });
            // Test
            Assert.IsTrue(isMissing);
        }

        [TestMethod]
        public void Can_determine_submit_hashtag_present()
        {
            // Execute
            var isMissing = TweetParser
                .IsSubmitHashtagMissing(new List<string>
                {
                    "submit",
                    "Present"
                });
            // Test
            Assert.IsFalse(isMissing);
        }
    }
}
