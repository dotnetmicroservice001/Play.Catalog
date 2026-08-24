using System;

namespace Play.Catalog.Contracts;

    public record CatalogItemCreated(Guid ItemId, string ItemName, string Description,
        decimal Price, string Category, string ImageUrl, string Rarity);
    public record CatalogItemUpdated(Guid ItemId, string ItemName, string Description,
        decimal Price, string Category, string ImageUrl, string Rarity);
    public record CatalogItemDeleted(Guid ItemId);