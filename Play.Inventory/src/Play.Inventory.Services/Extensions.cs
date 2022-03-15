using Play.Inventory.Services.Entities;
using static Play.Inventory.Services.Dtos;

namespace Play.Inventory.Services
{
    public static class Extensions
    {
        public static InventoryItemDto AsDto(this InventoryItem item)
        {
            return new InventoryItemDto(item.CatalogItemId, item.Quantity, item.AcquiredDate);
        }
    }
}