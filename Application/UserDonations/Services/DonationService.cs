using Application.Account;
using Application.donationAnimal.Services;
using Application.UserDonations.DTOs;
using Application.UserDonations.Mappings;
using Core.Constants;
using Core.Entities.Animal;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserDonations.Services
{
    public class DonationService : IDonationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DonationService> _logger;
        private readonly IUserContext _userContext;
        private readonly IPaymentService _paymentService;

        public DonationService(
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            ILogger<DonationService> logger,
            IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _logger = logger;
            _paymentService = paymentService;
        }
        public async Task<string> CreateDonationPaymentAsync(CreateDonationPaymentDto dto)
        {
            var currentUser = _userContext.GetCurrentUser();

            if (currentUser == null)
                throw new UnauthorizedAccessException();
            var user = await _unitOfWork.AdminService.GetUserByIdAsync(currentUser.Id);

            var donation = dto.ToEntity(user);

            await _unitOfWork.Repository<Donation>()
                .AddAsync(donation);

            await _unitOfWork.CompleteAsync();

            var url = await _paymentService
                .CreateDonationCheckoutSessionAsync(
                    donation.Id,
                    dto.Amount);

            return url;
        }
        public async Task HandleDonationCompleted(Session session)
        {
            if (!session.Metadata.TryGetValue(
                "DonationId",
                out var donationIdValue))
            {
                return;
            }

            var donationId = int.Parse(donationIdValue);

            var donation =
                await _unitOfWork.Repository<Donation>()
                    .GetAsync(donationId);

            if (donation == null)
                return;

            if (donation.PaymentStatus ==
                DonationPaymentStatus.Succeeded)
            {
                return;
            }

            donation.PaymentStatus =
                DonationPaymentStatus.Succeeded;

            donation.PaidAt =
                DateTime.UtcNow;

            var animal =
                await _unitOfWork.DonationAnimals
                    .GetAsync(donation.DonationAnimalId);

            if (animal != null)
            {
                animal.CollectedAmount += donation.Amount;
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
