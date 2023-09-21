using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    [Route("v1")]
    public class CategoryController : ControllerBase
    {
        [HttpGet("categories")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
        {
            var categories = await context.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("categories/{id:int}")]
        public async Task<IActionResult> GetAsync([FromRoute] int id,
                                                  [FromServices] BlogDataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
        [HttpPost("categories")]
        public async Task<IActionResult> PostAsync([FromBody] Category model,
                                                   [FromServices] BlogDataContext context)
        {
            await context.Categories.AddAsync(model);
            await context.SaveChangesAsync();

            return Created($"v1/categories/{model.Id}", model);
        }
        [HttpPut("categories")]
        public async Task<IActionResult> PutAsync([FromRoute] int id,
                                                  [FromBody] Category model,
                                                  [FromServices] BlogDataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            category.Name = model.Name;
            category.Slug = model.Slug;
            
            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return Ok(model);
        }
        [HttpDelete("categories")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id,
                                                     [FromServices] BlogDataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return Ok(category);
        }
    }
}
