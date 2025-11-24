using PaymentAPI.Application.Constants;

namespace PaymentAPI.Application.Wrappers;

/// <summary>
/// Represents a standardized API response wrapper.
/// </summary>
/// <typeparam name="T">The type of data being returned in the response.</typeparam>
/// <remarks>
/// Provides a consistent response structure across all API endpoints with status code, data, and message.
/// </remarks>
public class ApiResponse<T> where T : class
{
    /// <summary>
    /// Gets or sets the HTTP status code of the response.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the data payload of the response.
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// Gets or sets the message describing the result of the operation.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets a value indicating whether the response represents a successful operation (status code 2xx).
    /// </summary>
    public bool IsSuccess => Status >= 200 && Status < 300;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponse{T}"/> class.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="data">The response data.</param>
    /// <param name="message">The response message.</param>
    public ApiResponse(int statusCode, T data, string message)
    {
        Status = statusCode;
        Data = data;
        Message = message;
    }

    /// <summary>
    /// Creates a successful response with HTTP 200 OK status.
    /// </summary>
    /// <param name="data">The data to return.</param>
    /// <param name="message">Optional success message. Defaults to "Request processed successfully".</param>
    /// <returns>An <see cref="ApiResponse{T}"/> with status 200.</returns>
    public static ApiResponse<T> Success(T data, string message = "Request processed successfully")
    {
        return new ApiResponse<T>(StatusCode.OK, data, message);
    }

    /// <summary>
    /// Creates a resource created response with HTTP 201 Created status.
    /// </summary>
    /// <param name="data">The created resource data.</param>
    /// <param name="message">Optional message. Defaults to "Resource created successfully".</param>
    /// <returns>An <see cref="ApiResponse{T}"/> with status 201.</returns>
    public static ApiResponse<T> Created(T data, string message = "Resource created successfully")
    {
        return new ApiResponse<T>(StatusCode.Created, data, message);
    }

    /// <summary>
    /// Creates a failed response with HTTP 400 Bad Request status.
    /// </summary>
    /// <param name="message">The error message describing why the request failed.</param>
    /// <returns>An <see cref="ApiResponse{T}"/> with status 400 and no data.</returns>
    public static ApiResponse<T> Fail(string message)
    {
        return new ApiResponse<T>(StatusCode.BadRequest, default, message);
    }

    /// <summary>
    /// Creates a not found response with HTTP 404 Not Found status.
    /// </summary>
    /// <param name="message">The message indicating what resource was not found.</param>
    /// <returns>An <see cref="ApiResponse{T}"/> with status 404 and no data.</returns>
    public static ApiResponse<T> NotFound(string message)
    {
        return new ApiResponse<T>(StatusCode.NotFound, default, message);
    }

    /// <summary>
    /// Creates an internal server error response with HTTP 500 status.
    /// </summary>
    /// <param name="message">Optional error message. Defaults to "An unexpected error occurred".</param>
    /// <returns>An <see cref="ApiResponse{T}"/> with status 500 and no data.</returns>
    public static ApiResponse<T> InternalError(string message = "An unexpected error occurred")
    {
        return new ApiResponse<T>(StatusCode.InternalServerCode, default, message);
    }
}
