using Core.Entities.AdoptionApp;
using System.Linq;
using System;
using Core.Constants;
using Core.Entities;
using Ecom.Application.AnimalApplications.DTOs;

namespace Ecom.Application.AnimalApplications.Mappings
{

    public static class AnimalApplicationMappingExtensions
    {
        private const string DefaultImagePath = "/Images/Defult/DefultUserPic.jpeg";
        public static AnimalApplicationDto ToDto(this AdoptionApplication app)
        {
            return new AnimalApplicationDto
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

        public static AnimalApplicationDetailsDto ToDetailsDto(this AdoptionApplication app)
        {
            return new AnimalApplicationDetailsDto
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
                AnimalPictureUrl = app.Animal?.Photos?.FirstOrDefault()?.ImageUrl ?? string.Empty,

                // Include owned sections so consumer receives full application info
                HouseholdInfo = app.HouseholdInfo ?? new HouseholdInfo(),
                PetCareInfo = app.PetCareInfo ?? new PetCareInfo(),
                Preferences = app.Preferences ?? new AdoptionPreferences(),
                Agreement = app.Agreement ?? new AgreementInfo()
            };
        }

        public static AdoptionApplication ToEntity(this CreateAnimalApplicationDto dto, AppUser user)
        {
            return new AdoptionApplication
            {
                AnimalId = dto.AnimalId,
                ApplicantId = user.Id,
                ApplicationType = dto.ApplicationType, // ensure application type is preserved
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
