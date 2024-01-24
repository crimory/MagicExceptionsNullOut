using System.Net;
using System.Text.Json;
using _03_MagicValidation.DataAnnotationsValidation;
using _03_MagicValidation.WebsiteMapper;
using _03_MagicValidation.WebsiteModel;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace _03_MagicValidation;

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
            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await errorResponse.WriteStringAsync($"Error: Cannot deserialize: {websiteRawInput}");
            return errorResponse;
        }
        
        var customerOrder = websiteCustomerOrder.Map();
        var validationResults = customerOrder.Validate();
        if (validationResults.Count != 0)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            var errorMessages = validationResults.Select(x => x.ErrorMessage);
            await errorResponse.WriteStringAsync($"ValidationErrors: {string.Join(',', errorMessages)}");
            return errorResponse;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(customerOrder);
        return response;
    }
}