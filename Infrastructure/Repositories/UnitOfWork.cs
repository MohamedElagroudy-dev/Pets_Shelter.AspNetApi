using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Entities.Product;
using Core.Interfaces;
using Ecom.Core.Entities.Product;
using Infrastructure.Persistence;
using System.Collections.Concurrent;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ConcurrentDictionary<string, object> _repositories = new();
        private readonly ApplicationDbContext _context;
        public IProductRepository Products { get; }
        public IOrderRepository Orders { get; }
        public IImageManagementService Images { get; }
        public ICartService Cart { get; }
        public IGenericRepository<Rating> Ratings { get; }
        public IAdminService AdminService { get; }
        public IAnimalRepository Animals { get; }
        public IFosterAnimalRepository FosterAnimals { get; }
        public IAdoptionApplicationRepository AdoptionApplications { get; }
        public IEmailService EmailService { get; }
        public UnitOfWork(ApplicationDbContext context,
                          IProductRepository productRepository,
                          IGenericRepository<Rating> RatingRepo,
                          IImageManagementService _ImageService,
                          ICartService _CartService,
                          IOrderRepository orderRepository,
                          IAnimalRepository animalRepository,
                          IFosterAnimalRepository fosterAnimalRepository,
                          IAdoptionApplicationRepository adoptionApplicationRepository,
                          IAdminService adminService,
                          IEmailService emailService
                          )
        {
            _context = context;
            Products = productRepository;
            Orders = orderRepository;       
            Images = _ImageService;
            Cart = _CartService;
            Ratings = RatingRepo;
            AdminService = adminService;
            Animals = animalRepository;
            FosterAnimals = fosterAnimalRepository;
            AdoptionApplications = adoptionApplicationRepository;
            EmailService = emailService;
        }
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity).Name;

            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(type, t =>
            {
                var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(TEntity));
                return Activator.CreateInstance(repositoryType, _context)
                       ?? throw new InvalidOperationException($"Could not create repository instance for {t}.");
            });
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
