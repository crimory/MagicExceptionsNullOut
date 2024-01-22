using System.Net;
using System.Text.Json;
using _07_Railway_Oriented.DomainModel;
using _07_Railway_Oriented.DomainModelValidation;
using _07_Railway_Oriented.Errors;
using _07_Railway_Oriented.WebsiteMapper;
using _07_Railway_Oriented.WebsiteModel;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace _07_Railway_Oriented;

public static class ProcessCustomerOrderFunction
{
    [Function("ProcessCustomerOrder")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var websiteRawInput = await req.ReadAsStringAsync() ?? string.Empty;
        
        var outputOrError = RailwayUtility.WrapValue(websiteRawInput)
            .RailwayPropagate(DeserializeWebsiteCustomerOrder)
            .RailwayPropagate(WrappedMapper)
            .RailwayPropagate(CustomerOrderSimpleValidator.Validate);

        return await RailwayUtility.GetOkOrBadRequestResponse(req, outputOrError);
    }

    private static ErrorOrOutput<WebsiteCustomerOrder> DeserializeWebsiteCustomerOrder(string rawInput)
    {
        var websiteCustomerOrder = JsonSerializer.Deserialize<WebsiteCustomerOrder>(rawInput);
        return websiteCustomerOrder is not null
            ? RailwayUtility.WrapValue(websiteCustomerOrder)
            : new ErrorOrOutput<WebsiteCustomerOrder>.Error(
                [new DomainError($"Cannot deserialize {rawInput}")]);
    }

    private static ErrorOrOutput<CustomerOrder> WrappedMapper(WebsiteCustomerOrder websiteOrder)
    {
        return RailwayUtility.WrapValue(websiteOrder.Map());
    }
}