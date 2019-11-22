using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebStore.Controllers;
using WebStore.Interfaces.Api;

using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class WebApiTestControllerTests
    {
        private WebApiTestController _Controller;
        private Mock<IValuesService> _ValueServiceMock = new Mock<IValuesService>();
        private readonly string[] _ExpectedValues = { "1", "2", "3" };

        [TestInitialize]
        public void Initialize()
        {
            _ValueServiceMock
               .Setup(service => service.GetAsync())
               .ReturnsAsync(_ExpectedValues);

            _Controller = new WebApiTestController(_ValueServiceMock.Object);
        }

        [TestMethod]
        public async Task Index_Method_Returns_View_With_Values()
        {
            var result = await _Controller.Index();

            var view_result = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<string>>(view_result.Model);

            Assert.Equal(_ExpectedValues.Length, model.Count());
            _ValueServiceMock.Verify(service => service.GetAsync());
            _ValueServiceMock.VerifyNoOtherCalls();
        }
    }
}
