using Application.Account;
using Application.donationAnimal.Services;
using Application.UserDonations.DTOs;
using Application.UserDonations.Mappings;
using Core.Constants;
using Core.Entities.Animal;
using Core.Exceptions;
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
        private readonly IAuthService _authService;

        public DonationService(
            IUnitOfWork unitOfWork,
            IUserContext userContext,
            ILogger<DonationService> logger,
            IPaymentService paymentService,
            IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _logger = logger;
            _paymentService = paymentService;
            _authService = authService;
        }
        public async Task<string> CreateDonationPaymentAsync(CreateDonationPaymentDto dto)
        {
            if (dto.Amount < 50)
                throw new InvalidOperationException("Minimum donation amount is 50 EGP.");

            var animal = await _unitOfWork.DonationAnimals
                .GetAsync(dto.DonationAnimalId);

            if (animal == null)
                throw new NotFoundException(nameof(DonationAnimal),
                    dto.DonationAnimalId.ToString());

            if (animal.DonationStatus == DonationStatus.Funded)
                throw new InvalidOperationException(
                    "This donation campaign has already been funded.");

            var currentUser = _userContext.GetCurrentUser();

            if (currentUser == null)
                throw new UnauthorizedAccessException();
            var user = await _unitOfWork.AdminService.GetUserByIdAsync(currentUser.Id);

            var (User, roles) = await _authService.GetUserByEmailWithAddress(currentUser.Email!);

            var donation = dto.ToEntity(User);

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

                if (animal.CollectedAmount >= animal.GoalAmount)
                {
                    animal.CollectedAmount = animal.GoalAmount;

                    animal.DonationStatus = DonationStatus.Funded;
                }
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
