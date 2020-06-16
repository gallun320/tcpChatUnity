using System;
using System.Collections.Generic;
using System.Text;

namespace TcpChatServer.Requests
{
    public class ErrorClientRequest : ClientRequest
    {
        public ErrorClientRequest()
        {
            HasError = true;
        }
    }
}
