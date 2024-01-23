using System.Text.Json;
using _09_Out_Hidden_Output.DomainModel;
using _09_Out_Hidden_Output.DomainModelValidation;
using _09_Out_Hidden_Output.Errors;
using _09_Out_Hidden_Output.WebsiteMapper;
using _09_Out_Hidden_Output.WebsiteModel;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace _09_Out_Hidden_Output;

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