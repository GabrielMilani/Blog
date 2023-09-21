using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
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
            try
            {
                var categories = await context.Categories.ToListAsync();
                return Ok(categories);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Não foi possivel buscar as categorias. ERRO:{ex.Message}");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Falha interna no servidor. ERRO:{e.Message}");
            }
        }

        [HttpGet("categories/{id:int}")]
        public async Task<IActionResult> GetAsync([FromRoute] int id,
                                                  [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                {
                    return NotFound();
                }
                return Ok(category);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Falha interna no servidor. ERRO:{e.Message}");
            }
        }
        [HttpPost("categories")]
        public async Task<IActionResult> PostAsync([FromBody] EditorCategoryViewModel model,
                                                   [FromServices] BlogDataContext context)
        {
            try
            {
                var category = new Category
                {
                    Id = 0,
                    Posts = null,
                    Name = model.Name,
                    Slug = model.Slug.ToLower()
                };
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", category);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Falha interna no servidor. ERRO:{e.Message}");
            }
        }
        [HttpPut("categories")]
        public async Task<IActionResult> PutAsync([FromRoute] int id,
                                                  [FromBody] EditorCategoryViewModel model,
                                                  [FromServices] BlogDataContext context)
        {
            try
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
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Não foi possivel alterar a categoria. ERRO:{ex.Message}");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Falha interna no servidor. ERRO:{e.Message}");
            }
        }
        [HttpDelete("categories")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id,
                                                     [FromServices] BlogDataContext context)
        {
            try
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
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Não foi possivel deletar a categoria. ERRO:{ex.Message}");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Falha interna no servidor. ERRO:{e.Message}");
            }
        }
    }
}
