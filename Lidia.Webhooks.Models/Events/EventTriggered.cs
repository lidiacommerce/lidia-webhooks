using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Webhooks.Models.Events
{
    public class EventTriggered : BaseEvent
    {
        public string Name { get; set; }

        public string Code { get; set; }
    }
}
