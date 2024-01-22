using System.Net;
using System.Text.Json;
using _06_Exceptions_Catchers.DomainModel;
using _06_Exceptions_Catchers.DomainModelValidation;
using _06_Exceptions_Catchers.WebsiteMapper;
using _06_Exceptions_Catchers.WebsiteModel;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace _06_Exceptions_Catchers;

public class ProcessCustomerOrderFunction
{
    [Function("ProcessCustomerOrder")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var websiteRawInput = await req.ReadAsStringAsync() ?? string.Empty;
        CustomerOrder customerOrder;
        try
        {
            var websiteCustomerOrder = JsonSerializer.Deserialize<WebsiteCustomerOrder>(websiteRawInput);
            if (websiteCustomerOrder is null)
            {
                return await GetResponseWithJson(req, $$"""{"Error": "Cannot deserialize: {{websiteRawInput}}"}""");
            }

            customerOrder = websiteCustomerOrder.Map();
            var validationResults = customerOrder.Validate();
            if (validationResults.Length != 0)
            {
                var errorMessages = validationResults.Select(x => x.ErrorMessage);
                return await GetResponseWithJson(req, $$"""{"ValidationErrors":[{{string.Join(',', errorMessages)}}]}""");
            }
        }
        catch (JsonException e)
        {
            return await GetResponseWithJson(req, $$"""{"Error": "{{e.Message}}"}""");
        }
        catch (CustomMapperException e)
        {
            return await GetResponseWithJson(req, $$"""{"Error": "{{e.Message}}"}""");
        }
        catch (CustomValidationException e)
        {
            return await GetResponseWithJson(req, $$"""{"Error": "{{e.Message}}"}""");
        }
        catch (Exception e)
        {
            return await GetResponseWithJson(req, $$"""{"Error": "{{e.Message}}"}""");
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(customerOrder);
        return response;
    }

    private static async Task<HttpResponseData> GetResponseWithJson(
        HttpRequestData req, string json, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        var errorResponse = req.CreateResponse(statusCode);
        await errorResponse.WriteAsJsonAsync(json);
        return errorResponse;
    }
}