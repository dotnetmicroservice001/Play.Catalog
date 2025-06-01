using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common; 
using Play.Catalog.Contracts;


namespace Play.Catalog.Service.Controllers;


[ApiController]
[Route("items")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> _itemsRepository ;
    private readonly IPublishEndpoint _publishEndpoint;
    

    public ItemsController( IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint )
    {
        _itemsRepository = itemsRepository;
       _publishEndpoint = publishEndpoint;
        
    }
    
    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetAsync()
    {
        // get the collection from db and change Item object to itemDto 
        var items = (await _itemsRepository.GetAllAsync()).Select(item => item.AsDto());
        return items;
    }

    // Get items/{id }
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
    {
        var item = await _itemsRepository.GetAsync(id); 
        if (item == null)
        {
            return NotFound();
        }
        return item.AsDto();
    }

    // POST /items 
    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
    {
        var item = new Item
        { 
            Name = createItemDto.Name,
            Description = createItemDto.Description,
            Price = createItemDto.Price, 
            CreatedDate = DateTimeOffset.UtcNow
        };
        await _itemsRepository.CreateAsync(item); 
        await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));
        return CreatedAtAction( nameof(GetByIdAsync), new { id = item.Id }, item);
    }
    
    // PUT items/{id} 
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
    {
        var existingItem = await _itemsRepository.GetAsync(id);
        if (existingItem == null)
        {
            return NotFound();
        }
        
        existingItem.Name = updateItemDto.Name;
        existingItem.Description = updateItemDto.Description;
        existingItem.Price = updateItemDto.Price;
        await _itemsRepository.UpdateAsync(existingItem);
        await _publishEndpoint.Publish(new CatalogItemUpdated(
            existingItem.Id, existingItem.Name, existingItem.Description));
        return NoContent();
    }
    
    // DELETE items/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var item = await _itemsRepository.GetAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        await _itemsRepository.DeleteAsync(item.Id);
        await _publishEndpoint.Publish(new CatalogItemDeleted(item.Id));
        return NoContent();
    }
    
}