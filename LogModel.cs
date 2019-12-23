using System;
using System.Collections.Generic;
using System.Text;

namespace LogLogging
{
    public class LogModel
    {
        public string Message { get; set; }
        public string User { get; set; }
        public string Host { get; set; }
        public string Source { get; set; }
        public string CorellationId { get; set; }

    }

    public class ExceptionModel
    {
        public string Message { get; set; }
        public string User { get; set; }
        public string Host { get; set; }
        public string Source { get; set; }
        public string Statuscode { get; set; }
        public string CorellationId { get; set; }
        public Exception ExceptionDetail { get; set; }
        public string PropertyValues { get; set; }


    }
}
