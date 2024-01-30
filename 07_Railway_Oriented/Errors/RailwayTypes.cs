using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

namespace _07_Railway_Oriented.Errors;

internal record DomainError(string Message);

public abstract record ErrorOrOutput<T>
{
    private ErrorOrOutput()
    {
    }

    internal record Error(DomainError[] DomainErrors) : ErrorOrOutput<T>;
    internal record ActualValue(T Value) : ErrorOrOutput<T>;

    internal TOutput Match<TOutput>(Func<T, TOutput> processActualValue, Func<DomainError[], TOutput> processErrors)
    {
        return this switch
        {
            Error error => processErrors(error.DomainErrors),
            ActualValue actualValue => processActualValue(actualValue.Value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public static class RailwayUtility
{
    public static ErrorOrOutput<TO> RailwayBind<TI, TO>(
        this ErrorOrOutput<TI> input,
        Func<TI, ErrorOrOutput<TO>> internalProcess)
    {
        return input.Match(
            internalProcess,
            errors => new ErrorOrOutput<TO>.Error(errors));
    }

    public static async Task<HttpResponseData> GetOkOrBadRequestResponse<T>(
        HttpRequestData req, ErrorOrOutput<T> errorOrOutput)
    {
        return await errorOrOutput.Match(
            GetOkResponse,
            GetErrorResponse);
        
        async Task<HttpResponseData> GetErrorResponse(DomainError[] errors)
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            var errorMessages = errors.Select(x => x.Message);
            await errorResponse.WriteStringAsync($"Errors:[{string.Join(',', errorMessages)}]");
            return errorResponse;
        }
        
        async Task<HttpResponseData> GetOkResponse(T value)
        {
            var okResponse = req.CreateResponse(HttpStatusCode.OK);
            await okResponse.WriteAsJsonAsync(value);
            return okResponse;
        }
    }
}