using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS
{
    public class AWS06Secretes
    {
        public string project { get; set; }
        public string Description { get; set; }
        public Buckets buckets { get; set; }
        public Users users { get; set; }
    }

    public class Buckets
    {
        public string bucket { get; set; }
    }

    public class Users
    {
        public string user { get; set; }
        public string username { get; set; }
        public string accesskeyid { get; set; }
        public string secretaccesskey { get; set; }
    }

    public class Secrets
    {
        public AWS06Secretes AWS06Secretes { get; set; }
    }
}
