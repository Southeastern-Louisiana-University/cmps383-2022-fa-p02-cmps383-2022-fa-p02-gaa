using System.Net;
using FA22.P01.Tests.Web.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FA22.P02.Tests.Web;

[TestClass]
public class Phase2Tests
{
    private WebTestContext context = new();

    [TestInitialize]
    public void Init()
    {
        context = new WebTestContext();
    }

    [TestCleanup]
    public void Cleanup()
    {
        context.Dispose();
    }

    [TestMethod]
    public async Task ListAllProducts_Returns200AndData()
    {
        //arrange
        var webClient = context.GetStandardWebClient();

        //act
        var httpResponse = await webClient.GetAsync("/api/products");

        //assert
        await AssertListAllFunctions(httpResponse);
    }

    [TestMethod]
    public async Task GetProductById_Returns200AndDatawebClient()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var target = await GetItem(webClient);
        if (target == null)
        {
            Assert.Fail("Make List All products work first");
            return;
        }

        //act
        var httpResponse = await webClient.GetAsync($"/api/products/{target.Id}");

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we expect an HTTP 200 when calling GET /api/products/{id} ");
        var resultDto = await httpResponse.Content.ReadAsJsonAsync<Dto>();
        resultDto.Should().BeEquivalentTo(target, "we expect get product by id to return the same data as the list all product endpoint");
    }

    [TestMethod]
    public async Task GetProductById_NoSuchId_Returns404webClient()
    {
        //arrange
        var webClient = context.GetStandardWebClient();

        //act
        var httpResponse = await webClient.GetAsync("/api/products/999999");

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, "we expect an HTTP 404 when calling GET /api/products/{id} with an invalid id");
    }

    [TestMethod]
    public async Task CreateProduct_NoName_Returns400()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var request = new Dto
        {
            Description = "asd",
            Price = 1
        };

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/products", request);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when getting POST /api/products with no name");
    }

    [TestMethod]
    public async Task CreateProduct_NameTooLong_Returns400()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var request = new Dto
        {
            Name = "a".PadLeft(121, '0'),
            Description = "asd",
            Price = 1
        };

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/products", request);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when getting POST /api/products with no name");
    }

    [TestMethod]
    public async Task CreateProduct_NoDescription_ReturnsError()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var request = new Dto
        {
            Name = "asd",
            Price = 1
        };

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/products", request);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling POST /api/products with no description");
    }

    [TestMethod]
    public async Task CreateProduct_NoPrice_ReturnsError()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var request = new Dto
        {
            Description = "asd",
            Name = "asd"
        };

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/products", request);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling POST /api/products with no price");
    }

    [TestMethod]
    public async Task CreateProduct_NegativePrice_ReturnsError()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var request = new Dto
        {
            Description = "asd",
            Name = "asd"
        };

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/products", request);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling POST /api/products with negative price");
    }

    [TestMethod]
    public async Task CreateProduct_Returns201AndData()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var request = new Dto
        {
            Name = "a",
            Description = "asd",
            Price = 1
        };

        //act
        var httpResponse = await webClient.PostAsJsonAsync("/api/products", request);

        //assert
        await AssertCreateFunctions(httpResponse, request, webClient);
    }

    [TestMethod]
    public async Task UpdateProduct_InvalidId_Returns404()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var target = await GetItem(webClient);
        if (target == null)
        {
            Assert.Fail("You are not ready for this test");
            return;
        }

        //act
        var httpResponse = await webClient.PutAsJsonAsync($"/api/products/{9999999}", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, "we expect an HTTP 404 when getting PUT /api/products/{id} with an invalid id");
    }

    [TestMethod]
    public async Task UpdateProduct_NoName_Returns400()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var target = await GetItem(webClient);
        if (target == null)
        {
            Assert.Fail("You are not ready for this test");
            return;
        }

        target.Name = null;

        //act
        var httpResponse = await webClient.PutAsJsonAsync($"/api/products/{target.Id}", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when getting PUT /api/products/{id} with no name");
    }

    [TestMethod]
    public async Task UpdateProduct_NameTooLong_Returns400()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var target = await GetItem(webClient);
        if (target == null)
        {
            Assert.Fail("You are not ready for this test");
            return;
        }
        target.Name = "0".PadRight(121,'0');

        //act
        var httpResponse = await webClient.PutAsJsonAsync($"/api/products/{target.Id}", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when getting PUT /api/products/{id} with no name");
    }

    [TestMethod]
    public async Task UpdateProduct_NoDescription_ReturnsError()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var target = await GetItem(webClient);
        if (target == null)
        {
            Assert.Fail("You are not ready for this test");
            return;
        }
        target.Description = null;

        //act
        var httpResponse = await webClient.PutAsJsonAsync($"/api/products/{target.Id}", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling PUT /api/products/{id} with no description");
    }

    [TestMethod]
    public async Task UpdateProduct_NoPrice_ReturnsError()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var target = await GetItem(webClient);
        if (target == null)
        {
            Assert.Fail("You are not ready for this test");
            return;
        }
        target.Price = 0;

        //act
        var httpResponse = await webClient.PutAsJsonAsync($"/api/products/{target.Id}", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling PUT /api/products/{id} with no price");
    }

    [TestMethod]
    public async Task UpdateProduct_NegativePrice_ReturnsError()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var target = await GetItem(webClient);
        if (target == null)
        {
            Assert.Fail("You are not ready for this test");
            return;
        }
        target.Price = -1m;

        //act
        var httpResponse = await webClient.PutAsJsonAsync($"/api/products/{target.Id}", target);

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling PUT /api/products/{id} with negative price");
    }

    [TestMethod]
    public async Task UpdateProduct_Returns200AndData()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var target = await GetItem(webClient);
        if (target == null)
        {
            Assert.Fail("You are not ready for this test");
            return;
        }
        target.Name = Guid.NewGuid().ToString("N");

        //act
        var httpResponse = await webClient.PutAsJsonAsync($"/api/products/{target.Id}", target);

        //assert
        await AssertUpdateFunctions(httpResponse, target, webClient);
    }

    [TestMethod]
    public async Task DeleteProduct_NoSuchItem_ReturnsNotFound()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var request = new Dto
        {
            Description = "asd",
            Name = "asd",
            Price = 1
        };
        using var itemHandle = await CreateProduct(webClient, request);
        if (itemHandle == null)
        {
            Assert.Fail("You are not ready for this test");
            return;
        }
        //act
        var httpResponse = await webClient.DeleteAsync($"/api/products/{request.Id + 21}");

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, "we expect an HTTP 404 when calling DELETE /api/products/{id} with an invalid Id");
    }

    [TestMethod]
    public async Task DeleteProduct_ValidItem_ReturnsOk()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var request = new Dto
        {
            Description = "asd",
            Name = "asd",
            Price = 1
        };
        using var itemHandle = await CreateProduct(webClient, request);
        if (itemHandle == null)
        {
            Assert.Fail("You are not ready for this test");
            return;
        }
        //act
        var httpResponse = await webClient.DeleteAsync($"/api/products/{request.Id}");

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we expect an HTTP 200 when calling DELETE /api/products/{id} with a valid id");
    }

    [TestMethod]
    public async Task DeleteProduct_SameItemTwice_ReturnsNotFound()
    {
        //arrange
        var webClient = context.GetStandardWebClient();
        var request = new Dto
        {
            Description = "asd",
            Name = "asd",
            Price = 1
        };
        using var itemHandle = await CreateProduct(webClient, request);
        if (itemHandle == null)
        {
            Assert.Fail("You are not ready for this test");
            return;
        }
        //act
        await webClient.DeleteAsync($"/api/products/{request.Id}");
        var httpResponse = await webClient.DeleteAsync($"/api/products/{request.Id}");

        //assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, "we expect an HTTP 404 when calling DELETE /api/products/{id} on the same item twice");
    }

    private async Task<IDisposable?> CreateProduct(HttpClient webClient, Dto request)
    {
        try
        {
            var httpResponse = await webClient.PostAsJsonAsync("/api/products", request);
            var resultDto = await AssertCreateFunctions(httpResponse, request, webClient);
            request.Id = resultDto.Id;
            return new DeleteItem(resultDto, webClient);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static async Task<Dto?> GetItem(HttpClient webClient)
    {
        try
        {
            var getAllRequest = await webClient.GetAsync("/api/products");
            var getAllResult = await AssertListAllFunctions(getAllRequest);
            return getAllResult.OrderByDescending(x => x.Id).First();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static async Task<List<Dto>> AssertListAllFunctions(HttpResponseMessage httpResponse)
    {
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we expect an HTTP 200 when calling GET /api/products");
        var resultDto = await httpResponse.Content.ReadAsJsonAsync<List<Dto>>();
        Assert.IsNotNull(resultDto, "We expect json data when calling GET /api/products");
        resultDto.Should().HaveCountGreaterThan(2, "we expect at least 3 records");
        resultDto.All(x => !string.IsNullOrWhiteSpace(x.Name)).Should().BeTrue("we expect all products to have names");
        resultDto.All(x => !string.IsNullOrWhiteSpace(x.Description)).Should().BeTrue("we expect all products to have descriptions");
        resultDto.All(x => x.Price > 0).Should().BeTrue("we expect all products to have non zero/non negative prices");
        resultDto.All(x => x.Id > 0).Should().BeTrue("we expect all products to have an id");
        var ids = resultDto.Select(x => x.Id).ToArray();
        ids.Should().HaveSameCount(ids.Distinct(), "we expect Id values to be unique for every product");
        return resultDto;
    }

    private static async Task<Dto> AssertCreateFunctions(HttpResponseMessage httpResponse, Dto request, HttpClient webClient)
    {
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Created, "we expect an HTTP 201 when calling POST /api/products with valid data to create a new product");

        var resultDto = await httpResponse.Content.ReadAsJsonAsync<Dto>();
        Assert.IsNotNull(resultDto, "We expect json data when calling POST /api/products");

        resultDto.Id.Should().BeGreaterOrEqualTo(1, "we expect a newly created product to return with a positive Id");
        resultDto.Should().BeEquivalentTo(request, x => x.Excluding(y => y.Id), "We expect the create product endpoint to return the result");

        httpResponse.Headers.Location.Should().NotBeNull("we expect the 'location' header to be set as part of a HTTP 201");
        httpResponse.Headers.Location.Should().Be($"http://localhost/api/products/{resultDto.Id}", "we expect the location header to point to the get product by id endpoint");

        var getByIdResult = await webClient.GetAsync($"/api/products/{resultDto.Id}");
        getByIdResult.StatusCode.Should().Be(HttpStatusCode.OK, "we should be able to get the newly created product by id");
        var dtoById = await getByIdResult.Content.ReadAsJsonAsync<Dto>();
        dtoById.Should().BeEquivalentTo(resultDto, "we expect the same result to be returned by a create product as what you'd get from get product by id");

        var getAllRequest = await webClient.GetAsync("/api/products");
        await AssertListAllFunctions(getAllRequest);

        var listAllData = await getAllRequest.Content.ReadAsJsonAsync<List<Dto>>();
        Assert.IsNotNull(listAllData, "We expect json data when calling GET /api/products");
        listAllData.Should().NotBeEmpty("list all should have something if we just created a product");
        var matchingItem = listAllData.Where(x => x.Id == resultDto.Id).ToArray();
        matchingItem.Should().HaveCount(1, "we should be a be able to find the newly created product by id in the list all endpoint");
        matchingItem[0].Should().BeEquivalentTo(resultDto, "we expect the same result to be returned by a created product as what you'd get from get getting all products");

        return resultDto;
    }

    private async Task AssertUpdateFunctions(HttpResponseMessage httpResponse, Dto request, HttpClient webClient)
    {
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we expect an HTTP 200 when calling PUT /api/products/{id} with valid data to update a product");
        var resultDto = await httpResponse.Content.ReadAsJsonAsync<Dto>();
        resultDto.Should().BeEquivalentTo(request, "We expect the update product endpoint to return the result");

        var getByIdResult = await webClient.GetAsync($"/api/products/{request.Id}");
        getByIdResult.StatusCode.Should().Be(HttpStatusCode.OK, "we should be able to get the updated product by id");
        var dtoById = await getByIdResult.Content.ReadAsJsonAsync<Dto>();
        dtoById.Should().BeEquivalentTo(request, "we expect the same result to be returned by a update product as what you'd get from get product by id");

        var getAllRequest = await webClient.GetAsync("/api/products");
        await AssertListAllFunctions(getAllRequest);

        var listAllData = await getAllRequest.Content.ReadAsJsonAsync<List<Dto>>();
        listAllData.Should().NotBeEmpty("list all should have something if we just updated a product");
        Assert.IsNotNull(listAllData, "We expect json data when calling GET /api/products");
        var matchingItem = listAllData.Where(x => x.Id == request.Id).ToArray();
        matchingItem.Should().HaveCount(1, "we should be a be able to find the newly created product by id in the list all endpoint");
        matchingItem[0].Should().BeEquivalentTo(request, "we expect the same result to be returned by a updated product as what you'd get from get getting all products");
    }

    internal class Dto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }

    internal sealed class DeleteItem : IDisposable
    {
        private readonly Dto request;
        private readonly HttpClient webClient;

        public DeleteItem(Dto request, HttpClient webClient)
        {
            this.request = request;
            this.webClient = webClient;
        }

        public void Dispose()
        {
            try
            {
                webClient.DeleteAsync($"/api/products/{request.Id}").Wait();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}