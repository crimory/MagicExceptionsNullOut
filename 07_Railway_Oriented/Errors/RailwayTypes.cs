using System.Net;
using _07_Railway_Oriented.DomainModel;
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

    internal ErrorOrOutput<TK> PropagateError<TK>(Func<T, ErrorOrOutput<TK>> abc)
    {
        return this switch
        {
            Error error => new ErrorOrOutput<TK>.Error(error.DomainErrors),
            ActualValue actualValue => abc(actualValue.Value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public static class RailwayUtility
{
    public static ErrorOrOutput<T> WrapValue<T>(T value) =>
        new ErrorOrOutput<T>.ActualValue(value);
    
    public static ErrorOrOutput<TO> RailwayPropagate<TI, TO>(
        this ErrorOrOutput<TI> input,
        Func<TI, ErrorOrOutput<TO>> internalProcess)
    {
        return input.PropagateError(internalProcess);
    }

    public static async Task<HttpResponseData> GetOkOrBadRequestResponse<T>(HttpRequestData req, ErrorOrOutput<T> errorOrOutput)
    {
        return errorOrOutput switch
        {
            ErrorOrOutput<T>.Error error => await GetErrorResponse(error.DomainErrors),
            ErrorOrOutput<T>.ActualValue actualValue => await GetOkResponse(actualValue.Value),
            _ => throw new ArgumentOutOfRangeException()
        };
        
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