﻿using FluentValidation.TestHelper;
using Manufactures.Domain.GarmentCuttingOuts.Commands;
using Manufactures.Domain.GarmentCuttingOuts.ValueObjects;
using Manufactures.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Manufactures.Tests.Validations.GarmentCuttingOuts
{
    public class PlaceGarmentCuttingOutCommandValidatorTest
    {
        private PlaceGarmentCuttingOutCommandValidator GetValidationRules()
        {
            return new PlaceGarmentCuttingOutCommandValidator();
        }

        [Fact]
        public void Place_HaveError()
        {
            // Arrange
            var validator = GetValidationRules();
            var unitUnderTest = new PlaceGarmentCuttingOutCommand();

            // Action
            var result = validator.TestValidate(unitUnderTest);

            // Assert
            result.ShouldHaveError();
        }

        [Fact]
        public void Place_NotHaveError()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var unitUnderTest = new PlaceGarmentCuttingOutCommand()
            {
                Article = "Article",
                RONo = "RONo",
                Unit = new Domain.Shared.ValueObjects.UnitDepartment()
                {
                    Id = 1,
                    Code = "Code",
                    Name = "Name"
                },
                Comodity = new GarmentComodity()
                {
                    Id = 1,
                    Code = "Code",
                    Name = "Name"
                },
                CutOutNo = "CutOutNo",
                CuttingOutDate = DateTimeOffset.Now,
                CuttingOutType = "CuttingOutType",
                Price = 1,
                PriceSewing = 1,
                UnitFrom = new UnitDepartment()
                {
                    Id = 1,
                    Code = "Code",
                    Name = "Name"
                },
                Items = new List<GarmentCuttingOutItemValueObject>()
                {
                    new GarmentCuttingOutItemValueObject()
                    {

                        Id =id,
                        Details=new List<GarmentCuttingOutDetailValueObject>()
                        {
                            new GarmentCuttingOutDetailValueObject()
                            {

                               BasicPrice =1,
                               Color ="Color",
                               CutOutItemId =id,
                               CuttingOutQuantity =1,
                                RemainingQuantity =2,
                                CuttingOutUom =new Uom()
                                {
                                    Id =1,
                                    Unit ="Unit"
                                },
                                Id =id,
                                Price =0,
                                Size =new SizeValueObject()
                                {
                                    Id =1,
                                    Size ="Size"
                                },

                            }
                        },
                        CutOutId =id,
                        CuttingInDetailId =id,
                        CuttingInId =id,
                        DesignColor ="DesignColor",
                        IsSave =true,
                        Product =new Product()
                        {
                             Id = 1,
                             Code = "Code",
                             Name = "Name"
                        },

                        TotalCuttingOut =1,
                        TotalCuttingOutQuantity =1,
                        TotalRemainingQuantityCuttingInItem=2
                    }
                }
            };

            var validator = GetValidationRules();

            // Action
            var result = validator.TestValidate(unitUnderTest);

            // Assert
            result.ShouldNotHaveError();
        }
    }
}
