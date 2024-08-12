﻿using AutoMapper;
using Market.Data.Repositories.Interfaces;
using Market.DTOs.Product;
using Market.Models;
using Market.Services.Interfaces;

namespace Market.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ISubcategoryService _subcategoryService;
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, ISubcategoryService subcategoryService, IS3Service s3Service, IMapper mapper)
        {
            _repository = repository;
            _subcategoryService = subcategoryService;
            _s3Service = s3Service;
            _mapper = mapper;
        }

        public async Task<ProductDto> AddAsync(CreateProductDto productDto)
        {
            // Check if the subcategory exists
            var subcategoryExists = await _subcategoryService.ExistsAsync(productDto.SubcategoryId);
            if (!subcategoryExists) throw new ArgumentException("The specified subcategory does not exist.");

            // Upload file to S3
            var imageUrl = await _s3Service.UploadFileAsync(productDto.ImageFile);

            // Map and add product
            var product = _mapper.Map<Product>(productDto);
            product.ImageUrl = imageUrl; // Add image URL to product

            var addedProduct = await _repository.AddAsync(product);
            return _mapper.Map<ProductDto>(addedProduct);
        }

        public async Task<ProductDto> PatchAsync(int id, UpdateProductDto productDto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) throw new ArgumentException("Product not found.");

            // Update properties if provided
            if (productDto.Name != null) product.Name = productDto.Name;
            if (productDto.Description != null) product.Description = productDto.Description;
            if (productDto.Price.HasValue) product.Price = productDto.Price.Value;

            // Check and update SubcategoryId if provided
            if (productDto.SubcategoryId.HasValue)
            {
                var subcategoryExists = await _subcategoryService.ExistsAsync(productDto.SubcategoryId.Value);
                if (!subcategoryExists) throw new ArgumentException("The specified subcategory does not exist.");
                product.SubcategoryId = productDto.SubcategoryId.Value;
            }

            // Check and update if there's a new image file
            if (productDto.ImageFile != null)
            {
                var imageUrl = await _s3Service.UploadFileAsync(productDto.ImageFile);
                product.ImageUrl = imageUrl;  // Update the image URL
            }

            var updatedProduct = await _repository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) throw new ArgumentException("Product not found.");

            product.IsDeleted = true;
            await _repository.UpdateAsync(product);
            return true;
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

        public async Task<IEnumerable<ProductDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var products = await _repository.GetPagedAsync(pageNumber, pageSize);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}
