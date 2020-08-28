using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lidia.Webhooks.Models
{
    public class Webhook
    {
        [Key]
        public int WebhookId { get; set; }

        public int EventId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public PayloadContentType ContentType { get; set; }

        public string Payload { get; set; }

        #region [ Navigation properties ]

        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

        public virtual Event Event { get; set; }

        public virtual List<WebhookLogEntry> Logs { get; set; } = new List<WebhookLogEntry>();

        #endregion
    }
}