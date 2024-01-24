using System.Text.Json;
using _08_Null_Everywhere.DomainModel;
using _08_Null_Everywhere.DomainModelValidation;
using _08_Null_Everywhere.Errors;
using _08_Null_Everywhere.WebsiteMapper;
using _08_Null_Everywhere.WebsiteModel;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace _08_Null_Everywhere;

public static class ProcessCustomerOrderFunction
{
    [Function("ProcessCustomerOrder")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var websiteRawInput = await req.ReadAsStringAsync() ?? string.Empty;
        
        // WebsiteModel was adjusted, present 07 first
        var outputOrError = RailwayUtility.RailwayBind(websiteRawInput)
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
            ? RailwayUtility.RailwayBind(websiteCustomerOrder)
            : new ErrorOrOutput<WebsiteCustomerOrder>.Error(
                [new DomainError($"Cannot deserialize {rawInput}")]);
    }

    private static ErrorOrOutput<CustomerOrder> WrappedMapper(WebsiteCustomerOrder websiteOrder)
    {
        return RailwayUtility.RailwayBind(websiteOrder.Map());
    }
}