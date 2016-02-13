using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KJGarmentsWeb.Models
{
    /// <summary>
    /// Create a Table in your database with these fields to store all transactions with PayPal
    /// </summary>
    public class PayPalTransaction
    {
        public string RequestId { get; set; }
        public string TrackingReference { get; set; }
        public DateTime RequestTime { get; set; }
        public string RequestStatus { get; set; }
        public string TimeStamp { get; set; }
        public string RequestError { get; set; }
        public string Token { get; set; }
        public string PayerId { get; set; }
        public string RequestData { get; set; }
        public string PaymentTransactionId { get; set; }
        public string PaymentError { get; set; }
    }
}