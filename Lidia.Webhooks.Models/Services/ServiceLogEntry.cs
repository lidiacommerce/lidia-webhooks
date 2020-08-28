﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Webhooks.Models.Services
{
    public class ServiceLogEntry
    {
        public string Type { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Body { get; set; }
    }
}
