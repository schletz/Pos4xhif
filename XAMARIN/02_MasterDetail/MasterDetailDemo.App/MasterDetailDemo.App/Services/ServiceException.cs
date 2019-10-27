using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MasterDetailDemo.App.Services
{
    /// <summary>
    /// Exceptionklasse für alle Exceptions, die im Servicelayer auftreten.
    /// </summary>
    class ServiceException : Exception
    {
        /// <summary>
        /// Der Name der Methode, bei der die Exception aufgetreten ist. Wird durch
        /// System.Runtime.CompilerServices.CallerMemberName automatisch gesetzt.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Falls die Exception beim Request eines Webservices auftritt, wird die URL angegeben.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Falls die Exception beim Request eines Webservices auftritt, wird der HTTP Statuscode angegeben.
        /// </summary>
        public int? HttpStatusCode { get; set; }

        public ServiceException() : base() { }
        public ServiceException(string message, [CallerMemberName] string methodName = "") : base(message)
        {
            MethodName = methodName;
        }
        public ServiceException(string message, Exception innerException, [CallerMemberName] string methodName = "") : base(message, innerException)
        {
            MethodName = methodName;
        }
        public override string ToString()
        {
            int level = 0;
            string message = $"{MethodName}(): {Message} URL {Url ?? "N/A"} HTTP {HttpStatusCode?.ToString() ?? "N/A"}";
            Exception innerException = InnerException;

            while (innerException != null)
            {
                level++;
                message += Environment.NewLine + new String('-', 3 * level) + innerException.Message?.Replace(Environment.NewLine, " ");
                innerException = innerException.InnerException;
            }
            return message;
        }
    }
}
