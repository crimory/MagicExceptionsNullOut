using System.Net;
using System.Text.Json;
using _01_MagicAutoMapper.WebsiteMapper;
using _01_MagicAutoMapper.WebsiteModel;
using AutoMapper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace _01_MagicAutoMapper;

public class ProcessCustomerOrderFunction
{
    private readonly IMapper _websiteMapper = CustomerOrderMapperConfig.Config.CreateMapper();
    
    [Function("ProcessCustomerOrder")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var websiteRawInput = await req.ReadAsStringAsync() ?? string.Empty;
        var websiteCustomerOrder = JsonSerializer.Deserialize<WebsiteCustomerOrder>(websiteRawInput);

        var customerOrder = _websiteMapper.Map<DomainModel.CustomerOrder>(websiteCustomerOrder);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(customerOrder);
        return response;
    }
}