﻿using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    [Route("v1")]
    public class PostController : ControllerBase
    {
        [HttpGet("posts")]
        public async Task<IActionResult> GetAsync([FromServices]BlogDataContext context,
                                                  [FromQuery]int page = 0,
                                                  [FromQuery]int pageSize = 25)
        {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();
                var posts = await context.Posts.AsNoTracking()
                                               .Include(x => x.Category)
                                               .Include(x => x.Author)
                                               .Select(x => new ListPostsViewModel
                                               {
                                                   Id = x.Id,
                                                   Title = x.Title,
                                                   Slug = x.Slug,
                                                   LastUpdateDate = x.LastUpdateDate,
                                                   Category = x.Category.Name,
                                                   Author = $"{x.Author.Name} ({x.Author.Email})"
                                               })
                                               .Skip(page * pageSize)
                                               .Take(pageSize)
                                               .OrderByDescending(x => x.LastUpdateDate)  
                                               .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
            }
            catch (Exception)
            {  
                return StatusCode(500, new ResultViewModel<Post>("Falha interna no servidor."));   
            }
        }
        [HttpGet("posts/{id:int}")]
        public async Task<IActionResult> DetailAsync([FromServices] BlogDataContext context,
                                                     [FromRoute] int id)
        {
            try
            {
                var posts = await context.Posts.AsNoTracking()
                                               .Include(x => x.Author)
                                               .ThenInclude(x => x.Roles)
                                               .Include(x => x.Category)
                                               .FirstOrDefaultAsync(x => x.Id == id);
                if (posts == null)
                    return NotFound(new ResultViewModel<Post>("Conteúdo não encontrado."));

                return Ok(new ResultViewModel<Post>(posts));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Post>("Falha interna no servidor."));
            }
        }
        [HttpGet("posts/category/{category}")]
        public async Task<IActionResult> GetByCategoryAsync([FromServices] BlogDataContext context,
                                                            [FromRoute] string category,
                                                            [FromQuery] int page = 0,
                                                            [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();
                var posts = await context.Posts.AsNoTracking()
                                               .Include(x => x.Category)
                                               .Include(x => x.Author)
                                               .Where(x => x.Category.Slug == category)
                                               .Select(x => new ListPostsViewModel
                                               {
                                                   Id = x.Id,
                                                   Title = x.Title,
                                                   Slug = x.Slug,
                                                   LastUpdateDate = x.LastUpdateDate,
                                                   Category = x.Category.Name,
                                                   Author = $"{x.Author.Name} ({x.Author.Email})"
                                               })
                                               .Skip(page * pageSize)
                                               .Take(pageSize)
                                               .OrderByDescending(x => x.LastUpdateDate)
                                               .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Post>("Falha interna no servidor."));
            }
        }

    }
}
