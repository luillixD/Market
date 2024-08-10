using AutoMapper;
using Market.Data.Repositories.Interfaces;
using Market.DTOs.Product;
using Market.Models;
using Market.Services.Interfaces;

namespace Market.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductDto> AddAsync(CreateProductDto productDto, string imageUrl)
        {
            var product = _mapper.Map<Product>(productDto);
            product.ImageUrl = imageUrl; // Add image URL to product

            var addedProduct = await _repository.AddAsync(product);
            return _mapper.Map<ProductDto>(addedProduct);
        }

        public async Task<ProductDto> UpdateAsync(UpdateProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            var updatedProduct = await _repository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}
