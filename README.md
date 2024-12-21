# ProductManager

ProductManager/
├── src/
│   ├── ProductManager.API/
│   │   ├── Controllers/
│   │   │   └── ProductController.cs
│   │   ├── Middleware/
│   │   │   └── ExceptionMiddleware.cs
│   │   ├── Models/
│   │   │   ├── ApiResponse.cs
│   │   │   ├── ErrorApiResponse.cs
│   │   │   └── ValidationApiResponse.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── appsettings.Development.json
│   │
│   ├── ProductManager.Core/
│   │   ├── Entities/
│   │   │   └── Product.cs
│   │   ├── Interfaces/
│   │   │   ├── IRepository.cs
│   │   │   └── IProductRepository.cs
│   │   └── DTOs/
│   │       ├── ProductDto.cs
│   │       └── CreateProductDto.cs
│   │
│   ├── ProductManager.Infrastructure/
│   │   ├── Data/
│   │   │   ├── ProductManagerContext.cs
│   │   │   └── Configurations/
│   │   │       └── ProductConfiguration.cs
│   │   ├── Repositories/
│   │   │   └── ProductRepository.cs
│   │   └── Migrations/
│   │       ├── YYYYMMDDHHMMSS_InitialCreate.cs
│   │       └── ProductManagerContextModelSnapshot.cs
│   │
│   └── ProductManager.Application/
│       ├── Services/
│       │   └── ProductService.cs
│       ├── Interfaces/
│       │   └── IProductService.cs
│       ├── Mappings/
│       │   └── MappingProfile.cs
│       ├── Validators/
│       │   └── CreateProductDtoValidator.cs
│       └── Exceptions/
│           └── CustomValidationException.cs
│
└── tests/
    └── ProductManager.Tests/
        ├── UnitTests/
        │   ├── Services/
        │   │   └── ProductServiceTests.cs
        │   └── Controllers/
        │       └── ProductControllerTests.cs
        └── IntegrationTests/
            └── ProductApiTests.cs

dotnet run --project ProductManager.API

Bu komutu çalıştırarak API'yi başlatabiliriz.