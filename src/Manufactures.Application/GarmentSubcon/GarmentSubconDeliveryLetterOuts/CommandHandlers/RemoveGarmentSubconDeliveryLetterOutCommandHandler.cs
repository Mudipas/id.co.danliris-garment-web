﻿using ExtCore.Data.Abstractions;
using Infrastructure.Domain.Commands;
using Manufactures.Domain.GarmentSubcon.ServiceSubconCuttings;
using Manufactures.Domain.GarmentSubcon.ServiceSubconCuttings.Repositories;
using Manufactures.Domain.GarmentSubcon.ServiceSubconSewings;
using Manufactures.Domain.GarmentSubcon.ServiceSubconSewings.Repositories;
using Manufactures.Domain.GarmentSubcon.SubconDeliveryLetterOuts;
using Manufactures.Domain.GarmentSubcon.SubconDeliveryLetterOuts.Commands;
using Manufactures.Domain.GarmentSubcon.SubconDeliveryLetterOuts.Repositories;
using Manufactures.Domain.GarmentSubconCuttingOuts;
using Manufactures.Domain.GarmentSubconCuttingOuts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Manufactures.Application.GarmentSubcon.GarmentSubconDeliveryLetterOuts.CommandHandlers
{
    public class RemoveGarmentSubconDeliveryLetterOutCommandHandler : ICommandHandler<RemoveGarmentSubconDeliveryLetterOutCommand, GarmentSubconDeliveryLetterOut>
    {
        private readonly IStorage _storage;
        private readonly IGarmentSubconDeliveryLetterOutRepository _garmentSubconDeliveryLetterOutRepository;
        private readonly IGarmentSubconDeliveryLetterOutItemRepository _garmentSubconDeliveryLetterOutItemRepository;
        private readonly IGarmentSubconCuttingOutRepository _garmentCuttingOutRepository;
        private readonly IGarmentServiceSubconCuttingRepository _garmentSubconCuttingRepository;
        private readonly IGarmentServiceSubconSewingRepository _garmentSubconSewingRepository;

        public RemoveGarmentSubconDeliveryLetterOutCommandHandler(IStorage storage)
        {
            _storage = storage;
            _garmentSubconDeliveryLetterOutRepository = storage.GetRepository<IGarmentSubconDeliveryLetterOutRepository>();
            _garmentSubconDeliveryLetterOutItemRepository = storage.GetRepository<IGarmentSubconDeliveryLetterOutItemRepository>();
            _garmentCuttingOutRepository = storage.GetRepository<IGarmentSubconCuttingOutRepository>();
            _garmentSubconCuttingRepository = storage.GetRepository<IGarmentServiceSubconCuttingRepository>();
            _garmentSubconSewingRepository = storage.GetRepository<IGarmentServiceSubconSewingRepository>();
        }


        public async Task<GarmentSubconDeliveryLetterOut> Handle(RemoveGarmentSubconDeliveryLetterOutCommand request, CancellationToken cancellationToken)
        {
            var subconDeliveryLetterOut = _garmentSubconDeliveryLetterOutRepository.Query.Where(o => o.Identity == request.Identity).Select(o => new GarmentSubconDeliveryLetterOut(o)).Single();

            _garmentSubconDeliveryLetterOutItemRepository.Find(o => o.SubconDeliveryLetterOutId == subconDeliveryLetterOut.Identity).ForEach(async subconDeliveryLetterOutItem =>
            {
                subconDeliveryLetterOutItem.Remove();
                if (subconDeliveryLetterOut.ContractType == "SUBCON CUTTING")
                {
                    var subconCuttingOut = _garmentCuttingOutRepository.Query.Where(x => x.Identity == subconDeliveryLetterOutItem.SubconId).Select(s => new GarmentSubconCuttingOut(s)).Single();
                    subconCuttingOut.SetIsUsed(false);
                    subconCuttingOut.Modify();

                    await _garmentCuttingOutRepository.Update(subconCuttingOut);
                }
                if (subconDeliveryLetterOut.ContractType == "SUBCON JASA")
                {
                    if (subconDeliveryLetterOut.ServiceType == "SUBCON JASA KOMPONEN")
                    {
                        var subconCutting = _garmentSubconCuttingRepository.Query.Where(x => x.Identity == subconDeliveryLetterOutItem.SubconId).Select(s => new GarmentServiceSubconCutting(s)).Single();
                        subconCutting.SetIsUsed(false);
                        subconCutting.Modify();

                        await _garmentSubconCuttingRepository.Update(subconCutting);
                    }
                    if (subconDeliveryLetterOut.ServiceType == "SUBCON JASA GARMENT WASH")
                    {
                        var subconSewing = _garmentSubconSewingRepository.Query.Where(x => x.Identity == subconDeliveryLetterOutItem.SubconId).Select(s => new GarmentServiceSubconSewing(s)).Single();
                        subconSewing.SetIsUsed(false);
                        subconSewing.Modify();

                        await _garmentSubconSewingRepository.Update(subconSewing);
                    }

                }
                await _garmentSubconDeliveryLetterOutItemRepository.Update(subconDeliveryLetterOutItem);
            });

            subconDeliveryLetterOut.Remove();
            await _garmentSubconDeliveryLetterOutRepository.Update(subconDeliveryLetterOut);

            _storage.Save();

            return subconDeliveryLetterOut;
        }
    }
}
