using Lidia.Webhooks.Interfaces;
using Lidia.Webhooks.Models;
using Lidia.Webhooks.Models.Events;
using Lidia.Webhooks.Models.Requests;
using Lidia.Webhooks.Models.Responses;
using Lidia.Webhooks.Models.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lidia.Webhooks
{
    public class WebhookService : IWebhookService
    {
        #region [ Queries ]

        public WebhookServiceResponse<Event> GetEvents(EventQueryRequest req, List<ServiceLogEntry> log = null)
        {
            throw new NotImplementedException();
        }

        public WebhookServiceResponse<Webhook> GetWebhooks(WebhookQueryRequest req, List<ServiceLogEntry> log = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [ Events ]

        public WebhookServiceResponse<Event> TriggerEvent(EventTriggered ev, List<ServiceLogEntry> log = null)
        {
            // Create the watch
            var sw = new Stopwatch();
            sw.Start();

            // Create a log collection if necessary
            if (log == null)
            {
                log = new List<ServiceLogEntry>();
            }

            // Add log
            log.Add(new ServiceLogEntry()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = $"Event triggered. Event:{ev.Name}"
            });

            // Create a response object
            var response = new WebhookServiceResponse<Event>();

            #region [ Validate request ]

            // Check mandatory data
            List<string> dataErrors = new List<string>();

            //if (String.IsNullOrEmpty(ev.Owner.UserId) && String.IsNullOrEmpty(ev.Owner.UserToken))
            //{
            //    dataErrors.Add("No incoming user Id or token! You should provide a value to identify the order owner.");
            //}

            //if (String.IsNullOrEmpty(ev.Owner.UserIP))
            //{

            //    dataErrors.Add("No incoming user IP! For security audits you need to attach the user IP to order flows.");
            //}

            //if (String.IsNullOrEmpty(ev.Currency))
            //{

            //    dataErrors.Add("No incoming currency information! You should set the default currency for the order.");
            //}

            if (dataErrors.Count > 0)
            {
                // Add log
                log.Add(new ServiceLogEntry()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = dataErrors.Count + " error(s) found within the posted data! Terminating the process. Errors:" + String.Join(";", dataErrors)
                });

                // Stop the sw
                sw.Stop();

                response.Type = ResponseType.Error,
                response.Code = ((short)OrderServiceResponseCodes.Invalid_Request).ToString();
                response.PreProcessingTook = sw.ElapsedMilliseconds;
                response.Message = "There are some erros with the incoming request data!";
                response.Errors.AddRange(dataErrors);
                response.Log = log;

                return response;
            }

            #endregion

            #region [ Data manuplation ]

            #endregion

            // Stop the timer
            sw.Stop();

            // Set the pre-processing time and start the time
            response.PreProcessingTook = sw.ElapsedMilliseconds;
            sw.Start();

            #region [ Create order ]

            // Add log
            log.Add(new ServiceLogEntry()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = "Creating the order."
            });

            // Check ref number
            if (String.IsNullOrEmpty(ev.RefNumber))
            {
                // Get current date
                var currentDate = DateTime.UtcNow;

                // Create a ref number
                ev.RefNumber = ev.TenantId + "00" + ev.ApplicationId + currentDate.ToString("yyyyMMddHHmmss");
            }

            // Create the sales order
            var order = new Models.Order()
            {
                TenantId = ev.TenantId,
                ApplicationId = ev.ApplicationId,
                RefNumber = ev.RefNumber,
                Currency = ev.Currency,
                CurrencyRate = 1,
                IsCalculated = ev.IsCalculated,
                State = OrderState.Draft,
                Status = Framework.Models.EntityStatus.Active,
                OwnerIP = ev.Owner.UserIP,
                OwnerToken = ev.Owner.UserToken,
                OwnerSessionId = ev.Owner.SessionId,
                OwnerIsMember = ev.Owner.IsMember,
                OrderDate = DateTime.UtcNow
            };

            // Set parent if there is incoming data
            if (!String.IsNullOrEmpty(ev.ParentCode))
            {
                // Get the parent
                var parent = orderServiceBase.Get(o => o.OrderCode == ev.ParentCode).Result.FirstOrDefault();

                if (parent != null)
                {
                    order.ParentId = parent.OrderId;
                }
                else
                {
                    // Log the error
                }
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = string.Format("Order created. OrderCode:{0}", ev.OrderCode)
            });

            #endregion

            #region [ Add tags & properties ]

            // Add tags
            if (ev.Tags != null && ev.Tags.Any())
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "Adding order tags."
                });

                order.Tags = new List<Models.OrderTag>();

                foreach (var t in ev.Tags)
                {
                    var tag = new OrderTag()
                    {
                        Type = (TagType)(short)t.Type,
                        Value = t.Value,
                        Created = DateTime.UtcNow,
                        Status = Framework.Models.EntityStatus.Active
                    };

                    // Add order tag
                    order.Tags.Add(tag);

                    // Add log
                    logRecords.Add(new ServiceLogRecord()
                    {
                        Type = "DEBUG",
                        TimeStamp = DateTime.Now,
                        Body = string.Format("Order tag added. Type:" + t.Type.ToString() + "; Tag:" + t.Value)
                    });
                }
            }

            // Add properties
            if (ev.Properties != null && ev.Properties.Any())
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "DEBUG",
                    TimeStamp = DateTime.Now,
                    Body = "Adding order property."
                });

                order.Properties = new List<Models.OrderProperty>();

                foreach (var p in ev.Properties)
                {
                    var prop = new OrderProperty()
                    {
                        Key = p.Key,
                        Value = p.Value,
                        Extra = p.Extra,
                        Created = DateTime.UtcNow,
                        Status = Framework.Models.EntityStatus.Active
                    };

                    // Add order property
                    order.Properties.Add(prop);

                    // Add log
                    logRecords.Add(new ServiceLogRecord()
                    {
                        Type = "DEBUG",
                        TimeStamp = DateTime.Now,
                        Body = string.Format("Order property added. Key:" + p.Key + "; Value:" + p.Value + "; Extra:" + p.Extra)
                    });
                }
            }

            #endregion

            #region [ Add owner information ]

            // Check for existing owner
            var owner = orderOwnerServiceBase.Get(o => o.OwnerCode == ev.Owner.UserId).Result.FirstOrDefault();

            if (owner == null && !String.IsNullOrEmpty(ev.Owner.UserEmail)) owner = orderOwnerServiceBase.Get(o => o.OwnerEmail == ev.Owner.UserEmail).Result.FirstOrDefault();

            if (owner == null && !String.IsNullOrEmpty(ev.Owner.UserToken)) owner = orderOwnerServiceBase.Get(o => o.OwnerToken == ev.Owner.UserToken).Result.FirstOrDefault();

            if (owner != null)
            {
                order.OwnerId = owner.OwnerId;
            }
            else
            {
                order.Owner = new OrderOwner()
                {
                    OwnerCode = ev.Owner.UserId,
                    OwnerEmail = ev.Owner.UserEmail,
                    OwnerToken = ev.Owner.UserToken,
                    Gender = ev.Owner.Gender,
                    Age = ev.Owner.Age,
                    IsMember = ev.Owner.IsMember,
                    MemberSince = ev.Owner.MemberSince,
                    Segment1 = ev.Owner.Segment,
                    Segment2 = "", // Saved for later use
                    Segment3 = "", // Saved for later use
                    Status = Framework.Models.EntityStatus.Active,
                    Created = DateTime.UtcNow,
                };
            }

            // Add log
            logRecords.Add(new ServiceLogRecord()
            {
                Type = "DEBUG",
                TimeStamp = DateTime.Now,
                Body = string.Format("Order owner processed and set. OrderCode:{0}, OwnerId:{1}", ev.OrderCode, order.Owner != null ? order.Owner.OwnerId : order.OwnerId)
            });

            #endregion            

            #region [ Save order ]

            try
            {
                // Save the customer
                var baseServiceResponse = orderServiceBase.Create(order);

                if (baseServiceResponse.Type != Framework.Models.ServiceResponseTypes.Success)
                {
                    // Add log
                    logRecords.Add(new ServiceLogRecord()
                    {
                        Type = "ERROR",
                        TimeStamp = DateTime.Now,
                        Body = "There was an error while saving the order!"
                    });

                    // Stop the sw
                    sw.Stop();

                    response.Type = Common.Models.ServiceResponseTypes.Error;
                    response.Code = ((short)OrderServiceResponseCodes.General_Exception).ToString();
                    response.ServiceTook = sw.ElapsedMilliseconds;
                    response.Message = "There was an error while saving the order!";
                    response.Errors.Add("There was an error while saving the order!");
                    response.LogRecords = logRecords;

                    return response;

                }
                else
                {
                    // Add log
                    logRecords.Add(new ServiceLogRecord()
                    {
                        Type = "DEBUG",
                        TimeStamp = DateTime.Now,
                        Body = string.Format("Order successfuly created. OrderId:{0}; OrderCode:{1}",
                                                order.OrderId, order.OrderCode)
                    });

                    // Add the new object to the result
                    response.Result.Add(order);

                    // Set the response information
                    response.OrderId = order.OrderId;

                    // Add checkout start activity

                }
            }
            catch (Exception ex)
            {
                // Add log
                logRecords.Add(new ServiceLogRecord()
                {
                    Type = "ERROR",
                    TimeStamp = DateTime.Now,
                    Body = "There was an error while saving the order!"
                });

                // Stop the sw
                sw.Stop();

                response.Type = Common.Models.ServiceResponseTypes.Error;
                response.Code = ((short)OrderServiceResponseCodes.General_Exception).ToString();
                response.ServiceTook = sw.ElapsedMilliseconds;
                response.Message = "There was an error while saving the order! Exception:" + ex.ToString();
                response.Errors.Add("There was an error while saving the order! Exception:" + ex.ToString());
                response.LogRecords = logRecords;

                return response;
            }

            #endregion            

            // Stop the sw
            sw.Stop();

            response.Type = Common.Models.ServiceResponseTypes.Success;
            response.Code = ((short)OrderServiceResponseCodes.Request_Successfuly_Completed).ToString();
            response.ServiceTook = sw.ElapsedMilliseconds;
            response.Message = string.Format("Order successfuly created. SalesOrderId:{0}; OrderCode:{1}",
                                            order.OrderId, order.OrderCode);
            response.LogRecords = logRecords;

            return response;
        }

        #endregion

    }
}
