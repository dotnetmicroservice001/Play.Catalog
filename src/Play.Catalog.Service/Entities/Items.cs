using System;
using Play.Common; 

namespace Play.Catalog.Service.Entities;

public class Item : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public string Category { get; set; }
    public string ImageUrl { get; set; }
    public string Rarity { get; set; }

    public Item()
    {
        Id = Guid.NewGuid();
        Name = string.Empty;
        Description = string.Empty;
        Price = 0;
    }
}
