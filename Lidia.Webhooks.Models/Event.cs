using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Webhooks.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        public int? TenantId { get; set; }

        public int? ApplicationId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        #region [ Navigation properties ]

        public List<Webhook> Webhooks { get; set; } = new List<Webhook>();

        #endregion
    }
}
