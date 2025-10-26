using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Entities.Product;
using Core.Interfaces;
using Ecom.Core.Entities.Product;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IProductRepository Products { get; }
        public IGenericRepository<Photo> Photos { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<DeliveryMethod> DeliveryMethods { get; }

        public IOrderRepository Orders { get; }
        public IGenericRepository<OrderItem> OrderItems { get; }
        
        public IImageManagementService Images { get; }
        public ICartService Cart { get; }

        public IGenericRepository<Rating> Ratings { get; }


        public UnitOfWork(ApplicationDbContext context,
                          IProductRepository productRepository,
                          IGenericRepository<Photo> photoRepository,
                          IGenericRepository<Category> categoryRepository,
                          IGenericRepository<DeliveryMethod> DeliveryMethodsRepo,
                          IGenericRepository<Rating> RatingRepo,
                          IImageManagementService _ImageService,
                          ICartService _CartService,
                          IGenericRepository<OrderItem> orderItemsRepo,
                          IOrderRepository orderRepository
                          )
        {
            _context = context;
            Products = productRepository;
            Photos = photoRepository;
            Categories = categoryRepository;
            DeliveryMethods = DeliveryMethodsRepo;
            Orders = orderRepository;       
            OrderItems = orderItemsRepo;
            Images = _ImageService;
            Cart = _CartService;
            Ratings = RatingRepo;
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
