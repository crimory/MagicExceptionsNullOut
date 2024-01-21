using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using _06_Exceptions_Catchers.DomainModel;
using _06_Exceptions_Catchers.DomainModelValidation;
using _06_Exceptions_Catchers.WebsiteMapper;
using _06_Exceptions_Catchers.WebsiteModel;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace _06_Exceptions_Catchers;

public class ProcessCustomerOrderFunction
{
    [Function("ProcessCustomerOrder")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var websiteRawInput = await req.ReadAsStringAsync() ?? string.Empty;
        WebsiteCustomerOrder? websiteCustomerOrder;
        try
        {
            websiteCustomerOrder = JsonSerializer.Deserialize<WebsiteCustomerOrder>(websiteRawInput);
        }
        catch (Exception)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        if (websiteCustomerOrder is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        CustomerOrder customerOrder;
        try
        {
            customerOrder = websiteCustomerOrder.Map();
        }
        catch (Exception)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        List<ValidationResult> validationResults;
        try
        {
            validationResults = customerOrder.Validate();
        }
        catch (Exception)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        if (validationResults.Count != 0)
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