using System.Threading.Tasks;
using Facturi.Models.TokenAuth;
using Facturi.Web.Controllers;
using Shouldly;
using Xunit;

namespace Facturi.Web.Tests.Controllers
{
    public class HomeController_Tests: FacturiWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}