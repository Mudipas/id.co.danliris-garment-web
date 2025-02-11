﻿using ExtCore.Data.Abstractions;
using Infrastructure.Domain.Commands;
using Manufactures.Domain.GarmentSewingIns;
using Manufactures.Domain.GarmentSewingIns.Commands;
using Manufactures.Domain.GarmentSewingIns.Repositories;
using Manufactures.Domain.LogHistory;
using Manufactures.Domain.LogHistory.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Manufactures.Application.GarmentSewingIns.CommandHandlers
{
    public class UpdateDatesGarmentSewingInCommandHandler : ICommandHandler<UpdateDatesGarmentSewingInCommand, int>
    {
        private readonly IStorage _storage;
        private readonly IGarmentSewingInRepository _garmentSewingInRepository;
        private readonly ILogHistoryRepository _logHistoryRepository;
        public UpdateDatesGarmentSewingInCommandHandler(IStorage storage)
        {
            _storage = storage;
            _garmentSewingInRepository = storage.GetRepository<IGarmentSewingInRepository>();
            _logHistoryRepository = storage.GetRepository<ILogHistoryRepository>();
        }

        public async Task<int> Handle(UpdateDatesGarmentSewingInCommand request, CancellationToken cancellationToken)
        {
            List<Guid> guids = new List<Guid>();
            foreach (var id in request.Identities)
            {
                guids.Add(Guid.Parse(id));
            }
            var SewIns = _garmentSewingInRepository.Query.Where(a => guids.Contains(a.Identity)).Select(a => new GarmentSewingIn(a)).ToList();

            foreach (var model in SewIns)
            {
                model.setDate(request.Date);
                model.Modify();
                await _garmentSewingInRepository.Update(model);

                //Add Log History
                LogHistory logHistory = new LogHistory(new Guid(), "PRODUKSI", "Update Date Sewing In - " + model.SewingInNo, DateTime.Now);
                await _logHistoryRepository.Update(logHistory);
            }
            _storage.Save();

            return guids.Count();
        }
    }
}
