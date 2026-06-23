using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Entities.Product;
using Ecom.Core.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IGenericRepository<Rating> Ratings { get; }
        IImageManagementService Images { get; }
        ICartService Cart { get; }
        IOrderRepository Orders { get; }
        IAdminService AdminService { get; }
        IAnimalRepository Animals { get; } // Animals repository
        IFosterAnimalRepository FosterAnimals { get; } // FosterAnimals repository
        IEmailService EmailService { get; }
        IAdoptionApplicationRepository AdoptionApplications { get; }
        IDonationAnimalRepository DonationAnimals { get; }
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        Task<int> CompleteAsync();

    }
}
