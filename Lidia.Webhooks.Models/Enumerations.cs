using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Webhooks.Models
{
    // Responses
    public enum ResponseType
    {
        Success = 100,

        Error = 200,
    }


    // Webhooks
    public enum PayloadContentType
    {
        // application/x-www-form-urlencoded
        FormData = 10,

        // text/plain
        PlainText = 2,

        // text/html
        HtmlText = 3,

        // application/javascript
        ApplicationJavascript = 4,

        // application/json
        ApplicationJson = 5,

        // application/xml
        ApplicationXml = 6
    }
}
