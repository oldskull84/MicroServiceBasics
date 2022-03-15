using System;

namespace Play.Inventory.Services
{
    public class Dtos
    {
        public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);

        public record InventoryItemDto(Guid CatalogItemId, int Quantity, DateTimeOffset AcquiredDate);
    }
}