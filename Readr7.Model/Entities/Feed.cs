using System;
using System.Collections.Generic;

namespace Readr7.Model.Entites
{
    public class Feed
    {
        public String Id { get; set; }
        public String Title { get; set; }
        public String Continuation { get; set; }
        public String Author { get; set; }
        public DateTime Updated { get; set; }
        public List<Item> Items { get; set; }        
    }
}
