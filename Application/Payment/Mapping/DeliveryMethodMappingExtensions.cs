using Application.Payment.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payment.Mapping
{
    public static class DeliveryMethodMappingExtensions
    {
        public static DeliveryMethodDTO ToDto(this DeliveryMethod method)
        {
            return new DeliveryMethodDTO
            {
                Id = method.Id,
                ShortName = method.ShortName,
                DeliveryTime = method.DeliveryTime,
                Description = method.Description,
                Price = method.Price
            };
        }

        public static IReadOnlyList<DeliveryMethodDTO> ToDtoList(this IReadOnlyList<DeliveryMethod> methods)
        {
            return methods.Select(m => m.ToDto()).ToList();
        }
    }
}
