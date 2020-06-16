using System;
using System.Collections.Generic;
using System.Text;

namespace TcpChatServer.Requests
{
    public class AuthClientRequest : ClientRequest
    {
        public string Password { get; set; }
    }
}
