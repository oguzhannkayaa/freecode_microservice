using System.ComponentModel;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{

    //https://localhost:5001/items
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {

        private readonly IRepository<Item> itemsRepository;

        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(IRepository<Item> _itemsRepository, IPublishEndpoint _publishEndpoint)
        {
            itemsRepository = _itemsRepository;
            publishEndpoint = _publishEndpoint;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {


            var items = (await itemsRepository.GetAllAsync())
            .Select(item => item.AsDto());


            return Ok(items);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {


            var item = (await itemsRepository.GetAsync(id)).AsDto();


            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

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

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            var updatedItem = new Item
            {
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price
            };

            await itemsRepository.UpdateAsync(updatedItem);

            await publishEndpoint.Publish(new CatalogItemUpdated(updatedItem.Id, updatedItem.Name, updatedItem.Description));

            return NoContent();

        }

        // Delete /items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await itemsRepository.RemoveAsync(id);
            await publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }
    }

}


