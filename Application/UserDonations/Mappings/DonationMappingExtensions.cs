using Application.Account;
using Application.UserDonations.DTOs;
using Core.Constants;
using Core.Entities;
using Core.Entities.Animal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserDonations.Mappings
{
    public static class DonationMappingExtensions
    {
        private const string DefaultImagePath = "/Images/Defult/animal-default.jpg";
        private const string DefaultUserPicturePath = "/Images/Defult/DefultUserPic.jpeg";
        //public static UserDonationDto ToUserDonationDto(this Core.Entities.Donation donation)
        //{
        //    if (donation == null) return null;
        //    return new UserDonationDto
        //    {
        //        Id = donation.Id,
        //        UserId = donation.UserId,
        //        Amount = donation.Amount,
        //        DonationDate = donation.DonationDate,
        //        DonationType = donation.DonationType,
        //        PaymentMethod = donation.PaymentMethod,
        //        TransactionId = donation.TransactionId,
        //        Status = donation.Status
        //    };
        //}
        public static Donation ToEntity(this CreateDonationPaymentDto dto, AppUser currentUser)
        {
            var donation = new Donation
            {
                DonationAnimalId = dto.DonationAnimalId,

                DonorId = currentUser.Id,

                Amount = dto.Amount,

                Message = dto.Message,

                IsAnonymous = dto.IsAnonymous,

                DonorName = dto.IsAnonymous
                    ? null
                    : $"{currentUser.FirstName} {currentUser.LastName}",
                DonorProfilePicture = dto.IsAnonymous
                    ? null
                    : string.IsNullOrWhiteSpace(currentUser.PictureUrl) ? DefaultUserPicturePath : currentUser.PictureUrl,

                PaymentStatus = DonationPaymentStatus.Pending,

                DonatedAt = DateTime.UtcNow
            };
            return donation;
        }
    }
}
