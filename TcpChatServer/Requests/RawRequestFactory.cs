using System;
using System.Collections.Generic;
using System.Text;
using TcpChatServer.Utils;

namespace TcpChatServer.Requests
{
    public static class RawRequestFactory
    {
        public static string GetErrorRequest() => $"error:{RequestType.ErrorAlreadyLogin}:user already login";
        public static string GetAuthCompleteRequest() => $"login:{RequestType.LoginComplete}:login complete";
        public static string GetLogoutRequest() => $"logout:{RequestType.LogoutComplete}:logout complete";
        public static string GetShutdownRequest() => $"shutdown::";
        public static string GetMessageRequest(string message) => $"message:{RequestType.Message}:{message}";
        public static string GetMembersRequest(string message) => $"members:{RequestType.Members}:{message}";
    }
}
