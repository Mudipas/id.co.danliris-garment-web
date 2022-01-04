﻿using Moonlay.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manufactures.Domain.GarmentSample.SampleAvalProducts.ValueObjects
{
    [JsonConverter(typeof(SingleValueObjectConverter))]
    public class GarmentSamplePreparingItemId : SingleValueObject<string>
    {
        public GarmentSamplePreparingItemId(string value) : base(value)
        {
        }
    }
}
