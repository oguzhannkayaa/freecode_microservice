using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers
{

    //https://localhost:5001/items
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDto> items = new()
        {

            new ItemDto(Guid.NewGuid(), "Potion", "Restores a small of HP", 5, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Antidote", "Cures Poison", 7, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)

        };


        [HttpGet]
        public IEnumerable<ItemDto> Get()
        {
            return items;
        }


        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetById(Guid id)
        {
            var item = items.Where(item => item.Id == id).SingleOrDefault();

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        [HttpPost]
        public ActionResult<ItemDto> Post(CreateItemDto createItemDto)
        {
            var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
            items.Add(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existinItem = items.Where(item => item.Id == id).SingleOrDefault();

            if (existinItem == null)
            {
                return NotFound();
            }


            var updatedItem = existinItem with
            {
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price
            };

            var index = items.FindIndex(existinItem => existinItem.Id == id);
            items[index] = updatedItem;

            return NoContent();

        }

        // Delete /items/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var index = items.FindIndex(existinItem => existinItem.Id == id);

            if (index < 0)
            {
                return NotFound();
            }

            items.RemoveAt(index);
            return NoContent();
        }
    }

}


