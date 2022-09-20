using System;
using Telegram.Bot.Types;

namespace Telegram.Bot.Exceptions
{
    /// <summary>
    /// Represents an API error
    /// </summary>
    public class ApiRequestException : Exception
    {
        /// <summary>
        /// Gets the error code.
        /// </summary>
        public virtual int ErrorCode { get; }

        /// <summary>
        /// Contains information about why a request was unsuccessful.
        /// </summary>
        public ResponseParameters Parameters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequestException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ApiRequestException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequestException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        public ApiRequestException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequestException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApiRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequestException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="innerException">The inner exception.</param>
        public ApiRequestException(string message, int errorCode, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequestException"/> class
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="errorCode">The error code</param>
        /// <param name="parameters">Response parameters</param>
        public ApiRequestException(string message, int errorCode, ResponseParameters parameters)
            : base(message)
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRequestException"/> class
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="errorCode">The error code</param>
        /// <param name="parameters">Response parameters</param>
        /// <param name="innerException">The inner exception</param>
        public ApiRequestException(string message, int errorCode, ResponseParameters parameters,
                                   Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }

        /// <summary>
        /// Returns a new instance of the <see cref="ApiRequestException"/> class.
        /// </summary>
        /// <typeparam name="T">Expected type of operation result</typeparam>
        /// <param name="apiResponse">The API response.</param>
        /// <returns><see cref="ApiRequestException"/></returns>
        public static ApiRequestException FromApiResponse<T>(ApiResponse<T> apiResponse)
        {
            return ApiExceptionParser.Parse(apiResponse);
        }
    }
}
