using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHAPIServices
{
    public class AccountCustomField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public string External_id { get; set; }
    }

    public class AccountHistory
    {
        public string Name { get; set; }
        public string StatusCode { get; set; }
        public string Content { get; set; }
    }

    public class ProjectCustomField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public string External_id { get; set; }
    }

    public class ProjectHistory
    {
        public string Name { get; set; }
        public string StatusCode { get; set; }
        public string Content { get; set; }
    }
}

