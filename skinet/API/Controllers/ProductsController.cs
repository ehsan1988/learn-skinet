using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductRepository productRepository) : ControllerBase
{

  [HttpGet]
  public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
    string? brand, string? type, string? sort)
  {
    return Ok(await productRepository.GetProductsAsync(brand, type, sort));
  }
  [HttpGet("{id:int}")]
  public async Task<ActionResult<Product>> GetProduct(int id)
  {
    var product = await productRepository.GetProductByIdAsync(id);
    if (product == null) { return NotFound(); }
    return product;
  }
  [HttpPost]
  public async Task<ActionResult<Product>> CreateProduct(Product product)
  {
    productRepository.CreateProduct(product);
    if (await productRepository.SaveChangesAsync())
    {
      return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }
    return BadRequest("Error in Creating product");
  }

  [HttpPut("{id:int}")]
  public async Task<ActionResult> UpdateProduct(int id, Product product)
  {
    if (product.Id != id || !ProductExists(id))
    {
      return BadRequest("cannot update this prodcut");
    }
    productRepository.UpdateProduct(product);
    if (await productRepository.SaveChangesAsync())
    {
      return NoContent();
    }
    return BadRequest("Error in Updating Product");
  }

  [HttpDelete("{id:int}")]
  public async Task<ActionResult> DeleteProduct(int id)
  {
    var prodcut = await productRepository.GetProductByIdAsync(id);
    if (prodcut == null) { return NotFound(); }
    productRepository.DeleteProduct(prodcut);
    if (await productRepository.SaveChangesAsync())
    {
      return NoContent();
    }
    return BadRequest("Error in Deleting Product");
  }

  private bool ProductExists(int id)
  {
    return productRepository.ProductExists(id);
  }

  [HttpGet("brands")]
  public async Task<ActionResult<IReadOnlyList<Product>>> GetBrands()
  {
    return Ok(await productRepository.GetBrandsAsync());
  }
  [HttpGet("types")]
  public async Task<ActionResult<IReadOnlyList<Product>>> GetTypes()
  {
    return Ok(await productRepository.GetTypesAsync());
  }
}
