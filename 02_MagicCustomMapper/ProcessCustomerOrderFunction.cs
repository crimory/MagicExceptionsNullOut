using System.Net;
using System.Text.Json;
using _02_MagicCustomMapper.WebsiteMapper;
using _02_MagicCustomMapper.WebsiteModel;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace _02_MagicCustomMapper;

public static class ProcessCustomerOrderFunction
{
    [Function("ProcessCustomerOrder")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var websiteRawInput = await req.ReadAsStringAsync() ?? string.Empty;
        var websiteCustomerOrder = JsonSerializer.Deserialize<WebsiteCustomerOrder>(websiteRawInput);

        if (websiteCustomerOrder is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        
        var customerOrder = websiteCustomerOrder.Map();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(customerOrder);
        return response;
    }
}