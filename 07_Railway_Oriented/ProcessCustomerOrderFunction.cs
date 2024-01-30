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
        
        var outputOrError = new ErrorOrOutput<string>.ActualValue(websiteRawInput)
            .RailwayBind(DeserializeWebsiteCustomerOrder)
            .RailwayBind(WrappedMapper)
            .RailwayBind(CustomerOrderSimpleValidator.Validate);

        return await RailwayUtility.GetOkOrBadRequestResponse(req, outputOrError);
    }

    private static ErrorOrOutput<WebsiteCustomerOrder> DeserializeWebsiteCustomerOrder(string rawInput)
    {
        WebsiteCustomerOrder? websiteCustomerOrder;
        try
        {
            websiteCustomerOrder = JsonSerializer.Deserialize<WebsiteCustomerOrder>(rawInput);
        }
        catch (Exception e)
        {
            return new ErrorOrOutput<WebsiteCustomerOrder>.Error(
                [new DomainError($"Exception during deserialization: {e.Message}")]);
        }

        return websiteCustomerOrder is not null
            ? new ErrorOrOutput<WebsiteCustomerOrder>.ActualValue(websiteCustomerOrder)
            : new ErrorOrOutput<WebsiteCustomerOrder>.Error(
                [new DomainError($"Cannot deserialize {rawInput}")]);
    }

    private static ErrorOrOutput<CustomerOrder> WrappedMapper(WebsiteCustomerOrder websiteOrder)
    {
        return new ErrorOrOutput<CustomerOrder>.ActualValue(websiteOrder.Map());
    }
}