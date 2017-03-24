using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bumper.Models
{
    public class FormDisaster
    {
        public string instance { get; set; }

        public string quarantine { get; set; }
        public string snapshot { get; set; }
        public string dumpRam { get; set; }
        public string logExtract { get; set; }
        public string revokeKeys { get; set; }
        public string revokePolicy { get; set; }
        public string revokeSession { get; set; }
    }
}