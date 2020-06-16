using System;
using System.Collections.Generic;
using System.Text;

namespace TcpChatServer.Utils
{
    public enum RequestType
    {
        ErrorAlreadyLogin,
        LoginComplete,
        LogoutComplete,
        Message,
        Members
    }
}
