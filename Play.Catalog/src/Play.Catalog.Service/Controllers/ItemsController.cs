using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{
    // https://localhost:5001/items
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;
        // private static int requestCounter;
        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
        }

        // private static readonly List<ItemDto> items = new()
        // {
        //     new ItemDto(System.Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, System.DateTimeOffset.UtcNow),
        //     new ItemDto(System.Guid.NewGuid(), "Antidote", "Cures Poison", 7, System.DateTimeOffset.UtcNow),
        //     new ItemDto(System.Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, System.DateTimeOffset.UtcNow),
        // };

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            // requestCounter++;
            // Console.WriteLine($"Request {requestCounter}: Starting...");
            //
            // if (requestCounter <= 2)
            // {
            //     Console.WriteLine($"Request {requestCounter}: Delaying...");
            //     await Task.Delay(TimeSpan.FromSeconds(10));
            // }
            //
            // if (requestCounter <= 4)
            // {
            //     Console.WriteLine($"Request {requestCounter}: 500 (Internal Server Error).");
            //     return StatusCode(500);
            // }

            var items = (await itemsRepository.GetAllAsync())
            .Select(item => item.AsDto());
            Console.WriteLine($"Request {requestCounter}: 200 (OK).");
            return Ok(items);
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        // POST /items/{id}
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

            await itemsRepository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(PostAsync), new { id = item.Id }, item);
        }

        // PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await itemsRepository.GetAsync(id);

            if (existingItem == null) return NotFound();

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(existingItem);
            
            await publishEndpoint.Publish(new CatalogItemCreated(existingItem.Id, existingItem.Name, existingItem.Description));

            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null) return NotFound();

            await itemsRepository.RemoveAsync(item.Id);

            await publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }
    }
}