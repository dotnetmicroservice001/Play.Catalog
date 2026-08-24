using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common; 
using Play.Catalog.Contracts;
using Play.Common.Settings;


namespace Play.Catalog.Service.Controllers;


[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> _itemsRepository ;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly Counter<int> _itemsCreatedCounter;
    private readonly Counter<int> _itemsUpdatedCounter;
    private readonly Counter<int> _itemsDeletedCounter;

    private const string AdminRole = "Admin";

    public ItemsController(
        IRepository<Item> itemsRepository,
        IPublishEndpoint publishEndpoint,
        IConfiguration configuration)
    {
        _itemsRepository = itemsRepository;
       _publishEndpoint = publishEndpoint;

       var settings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
       Meter meter = new(settings.ServiceName);
       _itemsCreatedCounter = meter.CreateCounter<int>("ItemsCreated");
       _itemsUpdatedCounter = meter.CreateCounter<int>("ItemsUpdated");
       _itemsDeletedCounter = meter.CreateCounter<int>("ItemsDeleted");

    }
    
    [HttpGet]
    [Authorize(Policies.Read)]
    public async Task<IEnumerable<ItemDto>> GetAsync()
    {
        // get the collection from db and change Item object to itemDto 
        var items = (await _itemsRepository.GetAllAsync()).Select(item => item.AsDto());
        return items;
    }

    // Get items/{id }
    [HttpGet("{id}")]
    [Authorize(Policies.Read)]
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
    [Authorize(Policies.Write)]
    public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
    {
        var item = new Item
        {
            Name = createItemDto.Name,
            Description = createItemDto.Description,
            Price = createItemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow,
            Category = createItemDto.Category,
            ImageUrl = createItemDto.ImageUrl,
            Rarity = createItemDto.Rarity
        };
        await _itemsRepository.CreateAsync(item);
        _itemsCreatedCounter.Add(1, KeyValuePair.Create<string, object>("ItemId", item.Id));

        await _publishEndpoint.Publish(new CatalogItemCreated(
            item.Id,
            item.Name,
            item.Description,
            item.Price,
            item.Category,
            item.ImageUrl,
            item.Rarity));
        return CreatedAtAction( nameof(GetByIdAsync), new { id = item.Id }, item);
    }
    
    // PUT items/{id} 
    [HttpPut("{id}")]
    [Authorize(Policies.Write)]
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
        existingItem.Category = updateItemDto.Category;
        existingItem.ImageUrl = updateItemDto.ImageUrl;
        existingItem.Rarity = updateItemDto.Rarity;
        await _itemsRepository.UpdateAsync(existingItem);
        _itemsUpdatedCounter.Add(1, KeyValuePair.Create<string, object>("ItemId", existingItem.Id));

        await _publishEndpoint.Publish(new CatalogItemUpdated(
            existingItem.Id, existingItem.Name, existingItem.Description, existingItem.Price,
            existingItem.Category, existingItem.ImageUrl, existingItem.Rarity));
        return NoContent();
    }
    
    // DELETE items/{id}
    [HttpDelete("{id}")]
    [Authorize(Policies.Write)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var item = await _itemsRepository.GetAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        await _itemsRepository.DeleteAsync(item.Id);
        _itemsDeletedCounter.Add(1, KeyValuePair.Create<string, object>("ItemId", item.Id));
        await _publishEndpoint.Publish(new CatalogItemDeleted(item.Id));
        return NoContent();
    }
    
}