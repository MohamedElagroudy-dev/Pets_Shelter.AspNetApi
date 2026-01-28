using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Constants
{
    public enum OrderStatus
    {
        //[EnumMember(Value = "Pending")]
        Pending,
        //[EnumMember(Value = "Payment Received")]
        PaymentReceived,
        //[EnumMember(Value = "Payment Failed")]
        PaymentFailed,
        Shipped,
        Delivered,
        PaymentMismatch,
        Refunded,
        Canceled
    }
}
