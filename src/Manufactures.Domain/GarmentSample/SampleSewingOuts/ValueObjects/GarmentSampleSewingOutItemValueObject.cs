﻿using Manufactures.Domain.Shared.ValueObjects;
using Moonlay.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manufactures.Domain.GarmentSample.SampleSewingOuts.ValueObjects
{
    public class GarmentSampleSewingOutItemValueObject : ValueObject
    {
        public Guid Id { get; set; }
        public Guid SewingOutId { get; set; }
        public Guid SewingInId { get; set; }
        public Guid SewingInItemId { get; set; }
        public Product Product { get; set; }
        public string DesignColor { get; set; }
        public SizeValueObject Size { get; set; }
        public double Quantity { get; set; }
        public Uom Uom { get; set; }
        public string Color { get; set; }
        public List<GarmentSampleSewingOutDetailValueObject> Details { get; set; }
        public bool IsSave { get; set; }
        public bool IsDifferentSize { get; set; }
        public double SewingInQuantity { get; set; }
        public double TotalQuantity { get; set; }
        public double RemainingQuantity { get; set; }
        public double BasicPrice { get; set; }
        public double Price { get; set; }
        public GarmentSampleSewingOutItemValueObject()
        {
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            throw new NotImplementedException();
        }
    }
}