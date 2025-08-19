using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(IUnitOfWork unit) : BaseApiController
    {
        [HttpGet("orders")]
        public async Task<ActionResult<Order>> GetOrders([FromQuery] OrderSpecParams specParams)
        {
            var spec = new OrderSpecification(specParams);
            return await CreatePageResult(unit.Repository<Order>(), spec, specParams.PageIndex, specParams.PageSize);
        }

    }
}