using System;

namespace Play.Catalog.Contracts;

    public record CatalogItemCreated(Guid ItemId, string ItemName, string Description, 
        decimal Price);
    public record CatalogItemUpdated(Guid ItemId, string ItemName, string Description,
        decimal Price);
    public record CatalogItemDeleted(Guid ItemId);