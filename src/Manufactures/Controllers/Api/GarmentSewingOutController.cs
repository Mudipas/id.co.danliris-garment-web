﻿using Barebone.Controllers;
using Infrastructure.Data.EntityFrameworkCore.Utilities;
using Manufactures.Domain.GarmentSewingIns.Repositories;
using Manufactures.Domain.GarmentSewingOuts.Commands;
using Manufactures.Domain.GarmentSewingOuts.Repositories;
using Manufactures.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manufactures.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("sewing-outs")]
    public class GarmentSewingOutController : ControllerApiBase
    {
        private readonly IGarmentSewingOutRepository _garmentSewingOutRepository;
        private readonly IGarmentSewingOutItemRepository _garmentSewingOutItemRepository;
        private readonly IGarmentSewingOutDetailRepository _garmentSewingOutDetailRepository;
        private readonly IGarmentSewingInItemRepository _garmentSewingInItemRepository;

        public GarmentSewingOutController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _garmentSewingOutRepository = Storage.GetRepository<IGarmentSewingOutRepository>();
            _garmentSewingOutItemRepository = Storage.GetRepository<IGarmentSewingOutItemRepository>();
            _garmentSewingOutDetailRepository = Storage.GetRepository<IGarmentSewingOutDetailRepository>();
            _garmentSewingInItemRepository = Storage.GetRepository<IGarmentSewingInItemRepository>();
        }
        [HttpGet]
        public async Task<IActionResult> Get(int page = 1, int size = 25, string order = "{}", [Bind(Prefix = "Select[]")]List<string> select = null, string keyword = null, string filter = "{}")
        {
            VerifyUser();

            var query = _garmentSewingOutRepository.Read(page, size, order, keyword, filter);
            var count = query.Count();

            List<GarmentSewingOutListDto> garmentSewingOutListDtos = _garmentSewingOutRepository
                .Find(query)
                .Select(SewOut => new GarmentSewingOutListDto(SewOut))
                .ToList();

            var dtoIds = garmentSewingOutListDtos.Select(s => s.Id).ToList();
            var items = _garmentSewingOutItemRepository.Query
                .Where(o => dtoIds.Contains(o.SewingOutId))
                .Select(s => new { s.Identity, s.SewingOutId, s.ProductCode, s.Color , s.Quantity, s.RemainingQuantity})
                .ToList();

            var itemIds = items.Select(s => s.Identity).ToList();
            var details = _garmentSewingOutDetailRepository.Query
                .Where(o => itemIds.Contains(o.SewingOutItemId))
                .Select(s => new { s.Identity, s.SewingOutItemId })
                .ToList();

            Parallel.ForEach(garmentSewingOutListDtos, dto =>
            {
                var currentItems = items.Where(w => w.SewingOutId == dto.Id);
                dto.Colors = currentItems.Select(i => i.Color).Distinct().ToList();
                dto.Products = currentItems.Select(i =>  i.ProductCode).Distinct().ToList();
                dto.TotalQuantity = currentItems.Sum(i => i.Quantity);
                dto.TotalRemainingQuantity = currentItems.Sum(i => i.RemainingQuantity);
            });

            await Task.Yield();
            return Ok(garmentSewingOutListDtos, info: new
            {
                page,
                size,
                count
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            Guid guid = Guid.Parse(id);

            VerifyUser();

            GarmentSewingOutDto garmentSewingOutDto = _garmentSewingOutRepository.Find(o => o.Identity == guid).Select(sewOut => new GarmentSewingOutDto(sewOut)
            {
                Items = _garmentSewingOutItemRepository.Find(o => o.SewingOutId == sewOut.Identity).Select(sewOutItem => new GarmentSewingOutItemDto(sewOutItem)
                {
                    Details = _garmentSewingOutDetailRepository.Find(o => o.SewingOutItemId == sewOutItem.Identity).Select(sewOutDetail => new GarmentSewingOutDetailDto(sewOutDetail)
                    {
                    }).ToList()
                    
                }).ToList()
            }
            ).FirstOrDefault();

            await Task.Yield();
            return Ok(garmentSewingOutDto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PlaceGarmentSewingOutCommand command)
        {
            try
            {
                VerifyUser();

                var order = await Mediator.Send(command);

                return Ok(order.Identity);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateGarmentSewingOutCommand command)
        {
            Guid guid = Guid.Parse(id);

            command.SetIdentity(guid);

            VerifyUser();

            var order = await Mediator.Send(command);

            return Ok(order.Identity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            Guid guid = Guid.Parse(id);

            VerifyUser();

            RemoveGarmentSewingOutCommand command = new RemoveGarmentSewingOutCommand(guid);
            var order = await Mediator.Send(command);

            return Ok(order.Identity);

        }

        [HttpGet("complete")]
        public async Task<IActionResult> GetComplete(int page = 1, int size = 25, string order = "{}", [Bind(Prefix = "Select[]")]List<string> select = null, string keyword = null, string filter = "{}")
        {
            VerifyUser();

            var query = _garmentSewingOutRepository.Read(page, size, order, keyword, filter);
            var count = query.Count();

            var garmentSewingOutDto = _garmentSewingOutRepository.Find(query).Select(o => new GarmentSewingOutDto(o)).ToArray();
            var garmentSewingOutItemDto = _garmentSewingOutItemRepository.Find(_garmentSewingOutItemRepository.Query).Select(o => new GarmentSewingOutItemDto(o)).ToList();
            var garmentSewingOutDetailDto = _garmentSewingOutDetailRepository.Find(_garmentSewingOutDetailRepository.Query).Select(o => new GarmentSewingOutDetailDto(o)).ToList();

            Parallel.ForEach(garmentSewingOutDto, itemDto =>
            {
                var garmentSewingOutItems = garmentSewingOutItemDto.Where(x => x.SewingOutId == itemDto.Id).OrderBy(x => x.Id).ToList();

                itemDto.Items = garmentSewingOutItems;

                Parallel.ForEach(itemDto.Items, detailDto =>
                {
                    var garmentSewingOutDetails = garmentSewingOutDetailDto.Where(x => x.SewingOutItemId == detailDto.Id).OrderBy(x => x.Id).ToList();
                    detailDto.Details = garmentSewingOutDetails;
                });
            });

            if (order != "{}")
            {
                Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
                garmentSewingOutDto = QueryHelper<GarmentSewingOutDto>.Order(garmentSewingOutDto.AsQueryable(), OrderDictionary).ToArray();
            }

            await Task.Yield();
            return Ok(garmentSewingOutDto, info: new
            {
                page,
                size,
                count
            });
        }
    }
}