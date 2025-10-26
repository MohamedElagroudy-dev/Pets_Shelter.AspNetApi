using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class AddressExtensions
    {
        public static void UpdateFrom(this Address entity, Address newAddress)
        {
            entity.Line1 = newAddress.Line1;
            entity.Line2 = newAddress.Line2;
            entity.City = newAddress.City;
            entity.State = newAddress.State;
            entity.PostalCode = newAddress.PostalCode;
            entity.Country = newAddress.Country;
        }
    }
}
