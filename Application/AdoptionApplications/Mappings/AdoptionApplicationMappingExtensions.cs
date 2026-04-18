using Core.Entities.AdoptionApp;
using Ecom.Application.AdoptionApplications.DTOs;
using System.Linq;
using System;
using Core.Constants;
using Core.Entities;

namespace Ecom.Application.AdoptionApplications.Mappings
{

    public static class AdoptionApplicationMappingExtensions
    {
        private const string DefaultImagePath = "/Images/Defult/DefultUserPic.jpeg";
        public static AdoptionApplicationDto ToDto(this AdoptionApplication app)
        {
            return new AdoptionApplicationDto
            {
                Id = app.Id,
                AnimalId = app.AnimalId,
                AnimalName = app.Animal?.Name ?? string.Empty,
                ApplicantId = app.ApplicantId,
                ApplicantFirstName = app.ApplicantInfo.FirstName,
                ApplicantLastName = app.ApplicantInfo.LastName,
                ApplicantEmail = app.ApplicantInfo.Email,
                ApplicantPicture = string.IsNullOrWhiteSpace(app.ApplicantInfo.PersonalPicture) ? DefaultImagePath : app.ApplicantInfo.PersonalPicture,
                Status = app.Status,
                StatusName = app.Status.ToString(),
                SubmittedAt = app.SubmittedAt,
                AnimalPictureUrl = app.Animal?.Photos?.FirstOrDefault()?.ImageUrl ?? string.Empty
            };
        }

        public static AdoptionApplicationDetailsDto ToDetailsDto(this AdoptionApplication app)
        {
            return new AdoptionApplicationDetailsDto
            {
                Id = app.Id,
                Status = app.Status,
                SubmittedAt = app.SubmittedAt,
                AdminNotes = app.AdminNotes ?? string.Empty,
                ApplicantId = app.ApplicantId,
                FirstName = app.ApplicantInfo.FirstName,
                LastName = app.ApplicantInfo.LastName,
                PhoneNumber = app.ApplicantInfo.PhoneNumber,
                Email = app.ApplicantInfo.Email,
                Country = app.AddressInfo.Country,
                City = app.AddressInfo.City,
                ZipCode = app.AddressInfo.ZipCode,
                Address = app.AddressInfo.Address,
                AnimalId = app.AnimalId,
                AnimalName = app.Animal?.Name ?? string.Empty,
                AnimalDescription = app.Animal?.Description ?? string.Empty,
                AnimalPictureUrl = app.Animal?.Photos?.FirstOrDefault()?.ImageUrl ?? string.Empty
            };
        }

        public static AdoptionApplication ToEntity(this CreateAdoptionApplicationDto dto, AppUser user)
        {
            return new AdoptionApplication
            {
                AnimalId = dto.AnimalId,
                ApplicantId = user.Id,
                ApplicantInfo = new ApplicantInfo
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                    PersonalPicture = string.IsNullOrWhiteSpace(user.PictureUrl) ? DefaultImagePath : user.PictureUrl
                },
                AddressInfo = new AddressInfo
                {
                    Country = dto.Country,
                    City = dto.City,
                    ZipCode = dto.ZipCode,
                    Address = dto.Address
                },
                HouseholdInfo = new HouseholdInfo
                {
                    Details = dto.HouseholdDetails
                },
                PetCareInfo = new PetCareInfo
                {
                    ResponsiblePerson = dto.ResponsiblePerson,
                    AdoptionReason = dto.AdoptionReason,
                    AloneTimeDetails = dto.AloneTimeDetails,
                    LivingEnvironment = dto.LivingEnvironment
                },
                Preferences = new AdoptionPreferences
                {
                    Dog = dto.Dog,
                    Cat = dto.Cat,
                    Bird = dto.Bird,
                    Lizard = dto.Lizard,
                    Rabbit = dto.Rabbit,
                    Other = dto.Other,
                    HouseTrained = dto.HouseTrained,
                    Declawed = dto.Declawed,
                    Young = dto.Young,
                    MultiplePets = dto.MultiplePets,
                    SpecialConsiderations = dto.SpecialConsiderations
                },
                Agreement = new AgreementInfo
                {
                    Accepted = dto.Accepted
                },
                Status = ApplicationStatus.Pending,
                SubmittedAt = DateTime.UtcNow
            };
        }
    }
}
