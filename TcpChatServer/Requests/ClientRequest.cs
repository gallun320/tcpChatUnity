using System;
using System.Collections.Generic;
using System.Text;

namespace TcpChatServer.Requests
{
    public abstract class ClientRequest
    {
        public bool HasError { get; set; }
        public string Message { get; set; }
        public string Login { get; set; }
    }
}
