using System;
using System.Web.Routing;
using PayPalMvc;
using KJGarmentsWeb.Models;
using System.Collections.Generic;

namespace KJGarmentsWeb.Services
{
    public interface ITransactionService
    {
        SetExpressCheckoutResponse SendPayPalSetExpressCheckoutRequest(ApplicationCart cart, string serverURL, string userEmail = null);
        GetExpressCheckoutDetailsResponse SendPayPalGetExpressCheckoutDetailsRequest(string token);
        DoExpressCheckoutPaymentResponse SendPayPalDoExpressCheckoutPaymentRequest(ApplicationCart cart, string token, string payerId);
    } 

    /// <summary>
    /// The Transaction Service is used to transform a purchase object (eg cart, basket, or single item) into a sale request with PayPal (in this case a cart)
    /// It also allows your app to store the transactions in your database (create a table to match the PayPalTransaction model)
    /// 
    /// You should copy this file into your project and modify it to accept your purchase object, store PayPal transaction responses in your database,
    /// as well as log events with your favourite logger.
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private PayPalMvc.ITransactionRegistrar _payPalTransactionRegistrar = new PayPalMvc.TransactionRegistrar();

        public SetExpressCheckoutResponse SendPayPalSetExpressCheckoutRequest(ApplicationCart cart, string serverURL, string userEmail = null)
        {
            try
            {
                WebUILogging.LogMessage("SendPayPalSetExpressCheckoutRequest");

                // Optional handling of cart items: If there is only a single item being sold we don't need a list of expressCheckoutItems
                // However if you're selling a single item as a sale consider also adding it as an ExpressCheckoutItem as it looks better once you get to PayPal's site
                // Note: ExpressCheckoutItems are currently NOT stored by PayPal against the sale in the users order history so you need to keep your own records of what items were in a cart
                List<ExpressCheckoutItem> expressCheckoutItems = null;
                if (cart.Items != null)
                {
                    expressCheckoutItems = new List<ExpressCheckoutItem>();
                    foreach (ApplicationCartItem item in cart.Items)
                        expressCheckoutItems.Add(new ExpressCheckoutItem(item.Quantity, item.Price, item.Name, item.Description));
                }

                SetExpressCheckoutResponse response = _payPalTransactionRegistrar.SendSetExpressCheckout(cart.Currency, cart.TotalPrice, cart.PurchaseDescription, cart.Id.ToString(), serverURL, expressCheckoutItems, userEmail);

                // Add a PayPal transaction record
                DAL.PayPalTransaction transaction = new DAL.PayPalTransaction
                {
                    RequestId = response.RequestId,
                    TrackingReference = cart.Id.ToString(),
                    RequestTime = DateTime.Now,
                    RequestStatus = response.ResponseStatus.ToString(),
                    TimeStamp = response.TIMESTAMP,
                    RequestError = response.ErrorToString,
                    Token = response.TOKEN
                   
                };
                KJGarmentsWeb.DAL.peakzartEntities storedb = new KJGarmentsWeb.DAL.peakzartEntities();
                storedb.PayPalTransactions.Add(transaction);
                
                // Store this transaction in your Database
                storedb.SaveChanges();

                return response;
            }
            catch (Exception ex)
            {
                WebUILogging.LogException(ex.Message, ex);
            }
            return null;
        }

        public GetExpressCheckoutDetailsResponse SendPayPalGetExpressCheckoutDetailsRequest(string token)
        {
            try
            {
                WebUILogging.LogMessage("SendPayPalGetExpressCheckoutDetailsRequest");
                GetExpressCheckoutDetailsResponse response = _payPalTransactionRegistrar.SendGetExpressCheckoutDetails(token);

                // Add a PayPal transaction record
               DAL.PayPalTransaction transaction = new DAL.PayPalTransaction
                {
                    RequestId = response.RequestId,
                    TrackingReference = response.TrackingReference,
                    RequestTime = DateTime.Now,
                    RequestStatus = response.ResponseStatus.ToString(),
                    TimeStamp = response.TIMESTAMP,
                    RequestError = response.ErrorToString,
                    Token = response.TOKEN,
                    PayerId = response.PAYERID,
                    RequestData = response.ToString,
                };

                // Store this transaction in your Database
               KJGarmentsWeb.DAL.peakzartEntities storedb = new KJGarmentsWeb.DAL.peakzartEntities();
                storedb.PayPalTransactions.Add(transaction);

                DAL.SHIPDETAIL shipdetails = new DAL.SHIPDETAIL
                {
                    
                    TrackingReference = response.TrackingReference,
                    SHIPTONAME = response.PAYMENTREQUEST_0_SHIPTONAME,
                    SHIPTOSTREET = response.PAYMENTREQUEST_0_SHIPTOSTREET,
                    SHIPTOCITY = response.PAYMENTREQUEST_0_SHIPTOCITY,
                    SHIPTOSTATE = response.PAYMENTREQUEST_0_SHIPTOSTATE,
                    SHIPTOCOUNTRYCODE = response.PAYMENTREQUEST_0_SHIPTOCOUNTRYCODE,
                    SHIPTOZIP = response.PAYMENTREQUEST_0_SHIPTOZIP                    

                };
                storedb.SHIPDETAILS.Add(shipdetails);
                // Store this transaction in your Database
                storedb.SaveChanges();

                return response;
            }
            catch (Exception ex)
            {
                WebUILogging.LogException(ex.Message, ex);
            }
            return null;
        }

        public DoExpressCheckoutPaymentResponse SendPayPalDoExpressCheckoutPaymentRequest(ApplicationCart cart, string token, string payerId)
        {
            try
            {
                WebUILogging.LogMessage("SendPayPalDoExpressCheckoutPaymentRequest");
                DoExpressCheckoutPaymentResponse response = _payPalTransactionRegistrar.SendDoExpressCheckoutPayment(token, payerId, cart.Currency, cart.TotalPrice);

                // Add a PayPal transaction record
                DAL.PayPalTransaction transaction = new DAL.PayPalTransaction
                {
                    RequestId = response.RequestId,
                    TrackingReference = cart.Id.ToString(),
                    RequestTime = DateTime.Now,
                    RequestStatus = response.ResponseStatus.ToString(),
                    TimeStamp = response.TIMESTAMP,
                    RequestError = response.ErrorToString,
                    Token = response.TOKEN,
                    RequestData = response.ToString,
                    PaymentTransactionId = response.PaymentTransactionId,
                    PaymentError = response.PaymentErrorToString,
                };

                // Store this transaction in your Database
                KJGarmentsWeb.DAL.peakzartEntities storedb = new KJGarmentsWeb.DAL.peakzartEntities();
                storedb.PayPalTransactions.Add(transaction);
                // Store this transaction in your Database
                storedb.SaveChanges();


                return response;
            }
            catch (Exception ex)
            {
                WebUILogging.LogException(ex.Message, ex);
            }
            return null;
        }
    }
}