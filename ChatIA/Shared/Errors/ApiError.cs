namespace ChatIA.Shared.Errors;

public static class ApiError
{
    public static IResult BadRequest(string message)
    {
        return Results.BadRequest(new
        {
            message
        });
    }

    public static IResult NotFound(string message)
    {
        return Results.NotFound(new
        {
            message
        });
    }

    public static IResult ExternalServiceError(string message)
    {
        return Results.Problem(
            title: "Erro em serviço externo",
            detail: message,
            statusCode: StatusCodes.Status502BadGateway
        );
    }

    public static IResult InternalError(string message)
    {
        return Results.Problem(
            title: "Erro interno",
            detail: message,
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
}