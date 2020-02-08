using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingCards.Models
{

    public class Must
    {
        public object match { get; set; }
    }

    public class Term
    {
        public object term { get; set; }
    }

    public class Filter
    {
        public object term { get; set; }
    }

    public class Bool
    {
        public List<Must> must { get; set; }
        public List<Filter> filter { get; set; }
    }

    public class Query
    {
        public Bool @bool { get; set; }
    }

    public class RootDSLObject
    {
        public Query query { get; set; }
        public int from { get; set; }
        public int size { get; set; }
    }
}
