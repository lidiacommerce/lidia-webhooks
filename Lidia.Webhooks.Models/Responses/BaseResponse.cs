using Lidia.Webhooks.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Webhooks.Models.Responses
{
    public class BaseResponse<T> : BaseResponse
    {
    }

    public class BaseResponse
    {
        public short Code { get; set; }

        public string Message { get; set; }

        public ResponseType Type { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        public List<ServiceLogEntry> Log { get; set; } = new List<ServiceLogEntry>();

        public long PreProcessingTook { get; set; }
    }
}
