using System.Net;
using System.Text.Json;
using _04_MagicCustomValidation.DomainModelValidation;
using _04_MagicCustomValidation.WebsiteMapper;
using _04_MagicCustomValidation.WebsiteModel;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace _04_MagicCustomValidation;

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
        var validationResults = customerOrder.Validate();
        if (validationResults.Length != 0)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            foreach (var validationResult in validationResults)
            {
                errorResponse.Headers.Add("Validation-Error", validationResult.ErrorMessage);
            }
            return errorResponse;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(customerOrder);
        return response;
    }
}