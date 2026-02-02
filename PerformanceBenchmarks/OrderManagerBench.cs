using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using Moq;

// ✅ Replace these with your actual namespaces:
using ECommerceBusinessLogic;                 // OrderManager
using ECommerceBuinessDTO;                    // OrderBusinessDTO, ProductBusinessDTO, OrderStatus
using ECommerceDataAccessDTO;                 // OrderDataDto, ProductDataDto
using ECommerceDataAccessAbstraction;         // IUnitOfWork + repositories
using ECommerceInfrastructureAbstraction;
using ECommerceDataAccess.ProoductRepository;
using ECommerceDataAccess.OrderRepository;
using ECommerceEvents;
using ECommerceBusinessLogic.Mapping_Profiles;
using ECommwerceWebAPI.Mapping_Profiles;
using ECommerceDataAccess.Mapping_Profiles;
using AutoMapper.EquivalencyExpression;     // IEventBus

// If needed:
// using ECommerceEvents;                    // OrderCreatedEvent

[MemoryDiagnoser] // shows allocations + GC behavior
public class OrderManagerBench
{
    private OrderManager _sut = default!;
    private IMapper _mapper = default!;

    private Mock<IUnitOfWork> _uowMock = default!;
    private Mock<IEventBus> _eventBusMock = default!;

    // Repos: adjust interface names if different in your project
    private Mock<IProductRepository<ProductDataDto>> _productRepoMock = default!;
    private Mock<IOrderRepository<OrderDataDto>> _orderRepoMock = default!;

    // Template order -> we clone per iteration because CreateOrder mutates input
    private OrderBusinessDTO _templateOrder = default!;
    private OrderBusinessDTO _order = default!;

    // In-memory “DB”
    private List<ProductDataDto> _retrievedProducts = default!;

    [Params(1, 5, 20, 100)]
    public int ProductsCount;

    [GlobalSetup]
    public void Setup()
    {
        // 1) Build AutoMapper (real mapping)
        _mapper = CreateMapper();

        // 2) Build in-memory product data
        _retrievedProducts = BuildRetrievedProducts(ProductsCount);

        // 3) Prepare template order input
        _templateOrder = BuildOrderDto(ProductsCount);

        // 4) Setup mocks for UoW + repos + event bus
        _productRepoMock = new Mock<IProductRepository<ProductDataDto>>(MockBehavior.Strict);
        _orderRepoMock = new Mock<IOrderRepository<OrderDataDto>>(MockBehavior.Strict);
        _uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _eventBusMock = new Mock<IEventBus>(MockBehavior.Strict);

        // ProductRepository.GetListProductsById(ids) => return in-memory list filtered by ids
        _productRepoMock
            .Setup(r => r.GetListProductsById(It.IsAny<List<int>>()))
            .Returns((List<int> ids) => _retrievedProducts.Where(p => ids.Contains(p.Id)));

        // ProductRepository.UpdateProductsStockQuantity(list) => no-op true
        _productRepoMock
            .Setup(r => r.UpdateProductsStockQuantity(It.IsAny<List<ProductDataDto>>()))
            .ReturnsAsync(true);

        // OrderRepository.AddOrder(order) => no-op
        _orderRepoMock
            .Setup(r => r.AddOrder(It.IsAny<OrderDataDto>()))
            .Returns(Task.CompletedTask);

        // UoW wiring
        _uowMock.SetupGet(u => u.ProductRepository).Returns(_productRepoMock.Object);
        _uowMock.SetupGet(u => u.OrderRepository).Returns(_orderRepoMock.Object);

        // unitOfWork.Complete() => no-op
        _uowMock.Setup(u => u.Complete()).ReturnsAsync(1);

        // eventBus.Publish(event) => no-op
        // If Publish is generic in your project, adjust accordingly.
        _eventBusMock
         .Setup(b => b.Publish(It.IsAny<OrderCreatedEvent>()))
         .Returns(Task.CompletedTask);

        // 5) Instantiate SUT
        _sut = new OrderManager(_uowMock.Object, _mapper, _eventBusMock.Object);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        // fresh input every iteration (because CreateOrder mutates it)
        _order = CloneOrder(_templateOrder);
    }

    [Benchmark]
    public async Task CreateOrder_CPUOnly()
    {
        await _sut.CreateOrder(_order);
    }

    // -------------------------
    // Helpers
    // -------------------------

    private static IMapper CreateMapper()
    {
        var cfg = new MapperConfiguration(c =>
        {
            // You are mapping ProductDataDto into existing ProductBusinessDTO in your loop:
            // mapper.Map(productDataDto, targetProduct);
            c.AddProfile(new ProductMappingProfile()); // Add your profiles here
            c.AddProfile(new OrderAPIMappingProfile()); // Add your profiles here
            c.AddProfile(new OrderDataMappingProfile()); // Add your profiles here
            c.AddProfile(new OrderMappingProfile()); // Add your profiles here
            c.AddProfile(new ProductDataMappingProfile());
            c.AddCollectionMappers();

        });

        //cfg.AssertConfigurationIsValid();
        return cfg.CreateMapper();
    }

    private static OrderBusinessDTO BuildOrderDto(int count)
    {
        var order = new OrderBusinessDTO
        {
            CustomerEmail = "customer@test.com",
            products = new List<ProductBusinessDTO>(capacity: count),
        };

        for (int i = 1; i <= count; i++)
        {
            order.products.Add(new ProductBusinessDTO
            {
                Id = i,
                Quantity = 1
                // Price/StockQuantity will be filled from retrieved products via AutoMapper
            });
        }

        return order;
    }

    private static List<ProductDataDto> BuildRetrievedProducts(int count)
    {
        var list = new List<ProductDataDto>(capacity: count);

        for (int i = 1; i <= count; i++)
        {
            list.Add(new ProductDataDto
            {
                Id = i,
                price = 100m + i,          // any deterministic price
                StockQuantity = 10_000     // huge to avoid "not available"
            });
        }

        return list;
    }

    private static OrderBusinessDTO CloneOrder(OrderBusinessDTO source)
    {
        // Manual clone to avoid measuring serialization costs
        var clone = new OrderBusinessDTO
        {
            CustomerEmail = source.CustomerEmail,
            // Reset fields that CreateOrder overwrites anyway:
            TotalPrice = 0,
            Status = default,
            OrderNumber = 0,
            products = new List<ProductBusinessDTO>(source.products.Count)
        };

        foreach (var p in source.products)
        {
            clone.products.Add(new ProductBusinessDTO
            {
                Id = p.Id,
                Quantity = p.Quantity,
                // Keep Price/StockQuantity unset (will be mapped from retrieved)
                Price = 0,
                StockQuantity = 0
            });
        }

        return clone;
    }
}
