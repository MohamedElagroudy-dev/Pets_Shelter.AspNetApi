using Application.Common;
using Application.Common.Pagination;
using Application.donationAnimal.DTOs;

namespace Application.donationAnimal.Services
{
    public interface IDonationAnimalService
    {
        Task<PagedResult<DonationAnimalDTO>> GetAllAsync(DonationAnimalParams animalParams);
        Task<DonationAnimalDTO?> AddAsync(AddDonationAnimalDTO dto);
        Task<bool> UpdateAsync(UpdateDonationAnimalDTO dto);
        Task<DonationAnimalDTO?> DeleteAsync(int id);
        Task<DonationAnimalDTO?> GetAnimalAsync(int id);
    }
}
