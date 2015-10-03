using System;
using System.Collections.Generic;
using LinqToTwitter;

namespace Adventure.Services
{
    public class Tweet
    {
        public string TwitterUserIdentifier { get; set; }
        public string ScreenName { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> HashTags { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
        public string TweetId { get; set; }
        public List<MediaEntity> Media { get; internal set; }
    }
}