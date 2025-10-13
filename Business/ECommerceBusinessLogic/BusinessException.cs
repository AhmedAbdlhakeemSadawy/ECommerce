using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceBusinessLogic
{
    public class BusinessException : Exception
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }

        public BusinessException(string message, string errorCode = "BusinessError", int statusCode = 400)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }
}
