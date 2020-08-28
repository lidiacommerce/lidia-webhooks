using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lidia.Webhooks.Models
{
    [Table("WebhookLog")]
    public class WebhookLogEntry
    {
        [Key]
        public int LogId { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }
    }
}