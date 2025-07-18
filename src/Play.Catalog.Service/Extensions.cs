using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service
{
    public static class Extensions
    {
        // item entity into item dto 
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(
                item.Id, 
                item.Name, 
                item.Description, 
                item.Price, 
                item.CreatedDate); 
        }
    
    }
}