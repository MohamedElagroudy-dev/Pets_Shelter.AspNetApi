using Ecom.Application.AdoptionApplications.DTOs;
using Ecom.Application.AdoptionApplications.Mappings;
using Application.Common;
using Application.Common.Pagination;
using Core.Constants;
using Core.Entities.AdoptionApp;
using Core.Entities.Animal;
using Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Ecom.Application.AdoptionApplications.Services
{
    public class AdoptionApplicationService : IAdoptionApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdoptionApplicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateAsync(CreateAdoptionApplicationDto dto, string userId)
        {
            var animal = await _unitOfWork.Repository<Animal>().GetAsync(dto.AnimalId);
            if (animal == null) throw new ArgumentException("Animal not found");

            // ?? Prevent same user from applying twice for same animal
            var existing = await _unitOfWork.Repository<AdoptionApplication>().GetByAsync(
                a => a.AnimalId == dto.AnimalId && a.ApplicantId == userId
            );
            if (existing != null)
                throw new ArgumentException("You have already submitted an application for this animal.");



            var entity = dto.ToEntity(userId);

            await _unitOfWork.Repository<AdoptionApplication>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Id;
        }

        public async Task<PagedResult<AdoptionApplicationDto>> GetMyApplicationsAsync(string userId, AdoptionApplicationParams @params)
        {
            var (items, total) = await _unitOfWork.AdoptionApplications.GetAllAsync(@params.PageNumber, @params.PageSize, @params.Search, userId, @params.Status, @params.Sort);

            var dtos = items.Select(a => a.ToDto()).ToList();
            return new PagedResult<AdoptionApplicationDto>(dtos, total, @params.PageSize, @params.PageNumber);
        }

        public async Task<AdoptionApplicationDetailsDto?> GetMyApplicationByIdAsync(string userId, int id)
        {
            var app = await _unitOfWork.Repository<AdoptionApplication>().GetByAsync(a => a.Id == id && a.ApplicantId == userId, a => a.Animal, a => a.Animal.Photos);
            return app?.ToDetailsDto();
        }

        // ADMIN
        public async Task<PagedResult<AdoptionApplicationDto>> GetAllAsync(AdoptionApplicationParams @params)
        {
            var (items, total) = await _unitOfWork.AdoptionApplications.GetAllAsync(@params.PageNumber, @params.PageSize, @params.Search, null, @params.Status, @params.Sort);
            var dtos = items.Select(a => a.ToDto()).ToList();
            return new PagedResult<AdoptionApplicationDto>(dtos, total, @params.PageSize, @params.PageNumber);
        }

        public async Task<AdoptionApplicationDetailsDto?> GetByIdAsync(int id)
        {
            var app = await _unitOfWork.Repository<AdoptionApplication>().GetByAsync(a => a.Id == id, a => a.Animal, a => a.Animal.Photos);
            return app?.ToDetailsDto();
        }
    }
}
