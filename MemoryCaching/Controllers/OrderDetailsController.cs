using MemoryCaching.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Text;

namespace MemoryCaching.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderDetailsController : ControllerBase
	{
            private readonly IMemoryCache memoryCache;
            private readonly NorthwindContext context;
            public OrderDetailsController(IMemoryCache memoryCache, NorthwindContext context)
            {
                this.memoryCache = memoryCache;
                this.context = context;
            }
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var cacheKey = "orderdList";
                if (!memoryCache.TryGetValue(cacheKey, out List<ProductsInOrder> orderdList))
                {
                    orderdList = await context.ProductsInOrders.ToListAsync();
                    var cacheExpiryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                        Priority = CacheItemPriority.High,
                        SlidingExpiration = TimeSpan.FromMinutes(2)
                    };
                    memoryCache.Set(cacheKey, orderdList, cacheExpiryOptions);
                }
                return Ok(orderdList);
            }
        }
    }

