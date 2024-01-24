using System.Text.Json;
using _10_Impurity.DomainModel;
using _10_Impurity.DomainModelValidation;
using _10_Impurity.Errors;
using _10_Impurity.WebsiteMapper;
using _10_Impurity.WebsiteModel;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace _10_Impurity;

public static class ProcessCustomerOrderFunction
{
    [Function("ProcessCustomerOrder")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var websiteRawInput = await req.ReadAsStringAsync() ?? string.Empty;
        
        var outputOrError = RailwayUtility.RailwayBind(websiteRawInput)
            .RailwayBind(DeserializeWebsiteCustomerOrder)
            .RailwayBind(WrappedMapper)
            .RailwayBind(CustomerOrderSimpleValidator.Validate);
        LogInfo(outputOrError);

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

    private static void LogInfo(ErrorOrOutput<CustomerOrder> orderOrError)
    {
        _ = orderOrError.Match(
            order =>
            {
                order.Customer.Name = "Captain Pollution was here!";
                order.Positions = order.Positions
                    .Select(x =>
                    {
                        x.ItemName = "Captain Pollution was here!";
                        return x;
                    })
                    .ToArray();
                return true;
            },
            _ => true);
    }
}