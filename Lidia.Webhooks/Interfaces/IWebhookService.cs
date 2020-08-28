using Lidia.Webhooks.Models;
using Lidia.Webhooks.Models.Events;
using Lidia.Webhooks.Models.Requests;
using Lidia.Webhooks.Models.Responses;
using Lidia.Webhooks.Models.Services;
using System.Collections.Generic;

namespace Lidia.Webhooks.Interfaces
{
    public interface IWebhookService
    {
        #region [ Queries ]

        WebhookServiceResponse<Event> GetEvents(EventQueryRequest req, List<ServiceLogEntry> log = null);

        WebhookServiceResponse<Webhook> GetWebhooks(WebhookQueryRequest req, List<ServiceLogEntry> log = null);

        #endregion

        #region [ Events ]

        WebhookServiceResponse<Event> TriggerEvent(EventTriggered ev, List<ServiceLogEntry> log = null);

        #endregion
    }
}
