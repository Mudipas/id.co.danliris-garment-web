﻿using Barebone.Tests;
using Manufactures.Controllers.Api.GarmentSubcon;
using Manufactures.Domain.GarmentSubcon.ServiceSubconShrinkagePanels;
using Manufactures.Domain.GarmentSubcon.ServiceSubconShrinkagePanels.Commands;
using Manufactures.Domain.GarmentSubcon.ServiceSubconShrinkagePanels.ReadModels;
using Manufactures.Domain.GarmentSubcon.ServiceSubconShrinkagePanels.Repositories;
using Manufactures.Domain.Shared.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Manufactures.Tests.Controllers.Api.GarmentSubcon
{
    public class GarmentServiceSubconShrinkpagePanelControllerTests : BaseControllerUnitTest
    {
        private Mock<IGarmentServiceSubconShrinkagePanelRepository> _mockGarmentServiceSubconShrinkagePanelRepository;
        private Mock<IGarmentServiceSubconShrinkagePanelItemRepository> _mockGarmentServiceSubconShrinkagePanelItemRepository;
        private Mock<IGarmentServiceSubconShrinkagePanelDetailRepository> _mockServiceSubconShrinkagePanelDetailRepository;

        public GarmentServiceSubconShrinkpagePanelControllerTests() : base()
        {
            _mockGarmentServiceSubconShrinkagePanelRepository = CreateMock<IGarmentServiceSubconShrinkagePanelRepository>();
            _mockGarmentServiceSubconShrinkagePanelItemRepository = CreateMock<IGarmentServiceSubconShrinkagePanelItemRepository>();
            _mockServiceSubconShrinkagePanelDetailRepository = CreateMock<IGarmentServiceSubconShrinkagePanelDetailRepository>();

            _MockStorage.SetupStorage(_mockGarmentServiceSubconShrinkagePanelRepository);
            _MockStorage.SetupStorage(_mockGarmentServiceSubconShrinkagePanelItemRepository);
            _MockStorage.SetupStorage(_mockServiceSubconShrinkagePanelDetailRepository);
        }

        private GarmentServiceSubconShrinkagePanelController CreateGarmentServiceSubconShrinkagePanelController()
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            GarmentServiceSubconShrinkagePanelController controller = (GarmentServiceSubconShrinkagePanelController)Activator.CreateInstance(typeof(GarmentServiceSubconShrinkagePanelController), _MockServiceProvider.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "7";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }

        [Fact]
        public async Task Get_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = CreateGarmentServiceSubconShrinkagePanelController();

            _mockGarmentServiceSubconShrinkagePanelRepository
                .Setup(s => s.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<GarmentServiceSubconShrinkagePanelReadModel>().AsQueryable());

            Guid serviceSubconShrinkagePanelGuid = Guid.NewGuid();
            _mockGarmentServiceSubconShrinkagePanelRepository
                .Setup(s => s.Find(It.IsAny<IQueryable<GarmentServiceSubconShrinkagePanelReadModel>>()))
                .Returns(new List<GarmentServiceSubconShrinkagePanel>()
                {
                    new GarmentServiceSubconShrinkagePanel(serviceSubconShrinkagePanelGuid,null,  DateTimeOffset.Now, false)
                });

            Guid serviceSubconShrinkagePanelItemGuid = Guid.NewGuid();
            GarmentServiceSubconShrinkagePanelItem garmentServiceSubconShrinkagePanelItem = new GarmentServiceSubconShrinkagePanelItem(serviceSubconShrinkagePanelItemGuid, serviceSubconShrinkagePanelGuid, null, DateTimeOffset.Now, new UnitSenderId(1), null, null, new UnitRequestId(1), null, null);

            _mockGarmentServiceSubconShrinkagePanelItemRepository
                .Setup(s => s.Query)
                .Returns(new List<GarmentServiceSubconShrinkagePanelItemReadModel>()
                {
                    garmentServiceSubconShrinkagePanelItem.GetReadModel()
                }.AsQueryable());

            // Act
            var result = await unitUnderTest.Get();

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result));
        }

        [Fact]
        public async Task GetSingle_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = CreateGarmentServiceSubconShrinkagePanelController();
            Guid serviceSubconShrinkagePanelGuid = Guid.NewGuid();
            _mockGarmentServiceSubconShrinkagePanelRepository
                .Setup(s => s.Find(It.IsAny<Expression<Func<GarmentServiceSubconShrinkagePanelReadModel, bool>>>()))
                .Returns(new List<GarmentServiceSubconShrinkagePanel>()
                {
                    new GarmentServiceSubconShrinkagePanel(serviceSubconShrinkagePanelGuid,null, DateTimeOffset.Now, false)
                });

            Guid serviceSubconShrinkagePanelItemGuid = Guid.NewGuid();
            _mockGarmentServiceSubconShrinkagePanelItemRepository
                .Setup(s => s.Find(It.IsAny<Expression<Func<GarmentServiceSubconShrinkagePanelItemReadModel, bool>>>()))
                .Returns(new List<GarmentServiceSubconShrinkagePanelItem>()
                {
                    new GarmentServiceSubconShrinkagePanelItem(serviceSubconShrinkagePanelItemGuid, serviceSubconShrinkagePanelGuid, null, DateTimeOffset.Now, new UnitSenderId(1), null, null, new UnitRequestId(1), null, null)
                });
            _mockServiceSubconShrinkagePanelDetailRepository
                .Setup(s => s.Find(It.IsAny<Expression<Func<GarmentServiceSubconShrinkagePanelDetailReadModel, bool>>>()))
                .Returns(new List<GarmentServiceSubconShrinkagePanelDetail>()
                {
                    new GarmentServiceSubconShrinkagePanelDetail(new Guid(), serviceSubconShrinkagePanelItemGuid, new ProductId(1), null, null, null, null, 1, new UomId(1), null)
                });
            // Act
            var result = await unitUnderTest.Get(Guid.NewGuid().ToString());

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result));
        }

        [Fact]
        public async Task Post_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = CreateGarmentServiceSubconShrinkagePanelController();
            Guid serviceSubconShrinkagePanelGuid = Guid.NewGuid();
            _MockMediator
                .Setup(s => s.Send(It.IsAny<PlaceGarmentServiceSubconShrinkagePanelCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GarmentServiceSubconShrinkagePanel(serviceSubconShrinkagePanelGuid, null, DateTimeOffset.Now, false));

            // Act
            var result = await unitUnderTest.Post(It.IsAny<PlaceGarmentServiceSubconShrinkagePanelCommand>());

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result));
        }

        [Fact]
        public async Task Post_Throw_Exception()
        {
            // Arrange
            var unitUnderTest = CreateGarmentServiceSubconShrinkagePanelController();
            Guid serviceSubconShrinkagePanelGuid = Guid.NewGuid();
            _MockMediator
                .Setup(s => s.Send(It.IsAny<PlaceGarmentServiceSubconShrinkagePanelCommand>(), It.IsAny<CancellationToken>()))
                .Throws(new Exception());

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() => unitUnderTest.Post(It.IsAny<PlaceGarmentServiceSubconShrinkagePanelCommand>()));
        }

        [Fact]
        public async Task Put_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = CreateGarmentServiceSubconShrinkagePanelController();
            Guid serviceSubconShrinkagePanelGuid = Guid.NewGuid();
            _MockMediator
                .Setup(s => s.Send(It.IsAny<UpdateGarmentServiceSubconShrinkagePanelCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GarmentServiceSubconShrinkagePanel(serviceSubconShrinkagePanelGuid, null, DateTimeOffset.Now, false));
            // Act
            var result = await unitUnderTest.Put(Guid.NewGuid().ToString(), new UpdateGarmentServiceSubconShrinkagePanelCommand());

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result));
        }

        [Fact]
        public async Task Delete_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var unitUnderTest = CreateGarmentServiceSubconShrinkagePanelController();
            Guid serviceSubconShrinkagePanelGuid = Guid.NewGuid();
            _MockMediator
                .Setup(s => s.Send(It.IsAny<RemoveGarmentServiceSubconShrinkagePanelCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GarmentServiceSubconShrinkagePanel(serviceSubconShrinkagePanelGuid, null, DateTimeOffset.Now, false));

            // Act
            var result = await unitUnderTest.Delete(Guid.NewGuid().ToString());

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(result));
        }
    }
}
