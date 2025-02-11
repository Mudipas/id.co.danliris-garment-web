﻿using ExtCore.Data.Abstractions;
using Infrastructure.Domain.Commands;
using Manufactures.Domain.GarmentLoadings;
using Manufactures.Domain.GarmentLoadings.Commands;
using Manufactures.Domain.GarmentLoadings.Repositories;
using Manufactures.Domain.LogHistory;
using Manufactures.Domain.LogHistory.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Manufactures.Application.GarmentLoadings.CommandHandlers
{
    public class UpdateDatesGarmentLoadingCommandHandler : ICommandHandler<UpdateDatesGarmentLoadingCommand, int>
    {
        private readonly IGarmentLoadingRepository _garmentLoadingRepository;
        private readonly IStorage _storage;
        private readonly ILogHistoryRepository _logHistoryRepository;
        public UpdateDatesGarmentLoadingCommandHandler(IStorage storage)
        {
            _garmentLoadingRepository = storage.GetRepository<IGarmentLoadingRepository>();
            _storage = storage;
            _logHistoryRepository = storage.GetRepository<ILogHistoryRepository>();
        }

        public async Task<int> Handle(UpdateDatesGarmentLoadingCommand request, CancellationToken cancellationToken)
        {
            List<Guid> guids = new List<Guid>();
            foreach (var id in request.Identities)
            {
                guids.Add(Guid.Parse(id));
            }
            var Preparings = _garmentLoadingRepository.Query.Where(a => guids.Contains(a.Identity)).Select(a => new GarmentLoading(a)).ToList();

            foreach (var model in Preparings)
            {
                model.setDate(request.Date);
                model.Modify();
                await _garmentLoadingRepository.Update(model);

                //Add Log History
                LogHistory logHistory = new LogHistory(new Guid(), "PRODUKSI", "Update Date Loading - " + model.LoadingNo, DateTime.Now);
                await _logHistoryRepository.Update(logHistory);
            }
            _storage.Save();

            return guids.Count();
        }
    }
}
