using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpChatServer.Requests;

namespace TcpChatServer.Connection
{
    public class Client : IDisposable
    {
        private readonly NetworkStream _stream;
        private readonly int _bufferSize = 4096;

        public Client(TcpClient c)
        {
            _stream = c.GetStream();
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public async Task<ClientRequest> ProcessAsync()
        {
            var res = await ReadFromStreamAsync();
            Console.WriteLine(res);
            var request = ParseMessage(res);
            return request;
        }

        public async Task WriteError()
        {
            var error = RawRequestFactory.GetErrorRequest();
            await WriteToStreamAsync(error);
        }

        public async Task WriteAuthComplete()
        {
            var req = RawRequestFactory.GetAuthCompleteRequest();
            await WriteToStreamAsync(req);
        }

        public async Task WriteLogoutComplete()
        {
            var req = RawRequestFactory.GetLogoutRequest();
            await WriteToStreamAsync(req);
        }

        public async Task WriteShutdown()
        {
            var req = RawRequestFactory.GetShutdownRequest();
            await WriteToStreamAsync(req);
        }

        public async Task WriteChatMessage(string message)
        {
            
            var req = RawRequestFactory.GetMessageRequest(message);
            await WriteToStreamAsync(req);
        }

        public async Task WriteAllMembers(string message)
        {
            var req = RawRequestFactory.GetMembersRequest(message);
            await WriteToStreamAsync(req);
        }

        private async Task<string> ReadFromStreamAsync()
        {
            var buffer = new byte[_bufferSize];
            var byteCount = await _stream.ReadAsync(buffer, 0, _bufferSize);
            var result = Encoding.UTF8.GetString(buffer, 0, byteCount);
            return result;
        }

        private async Task WriteToStreamAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(buffer, 0, buffer.Length);
        }

        private ClientRequest ParseMessage(string message)
        {
            if(string.IsNullOrEmpty(message))
            {
                return new ErrorClientRequest();
            }

            var rawMessage = message.Split(":");
            var type = rawMessage[0];
            var login = rawMessage[1];
            var content = rawMessage[2];

            if(type.Contains("auth"))
            {
                return new AuthClientRequest
                {
                    Login = login,
                    Message = content
                };
            }

            if(type.Contains("message"))
            {
                return new ChatClientRequest
                {
                    Login = login,
                    Message = content
                };
            }

            if(type.Contains("error"))
            {
                return new ErrorClientRequest
                {
                    Login = login,
                    Message = content
                };
            }

            if (type.Contains("logout"))
            {
                return new LogoutClientRequest
                {
                    Login = login,
                    Message = content
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }


    }
}
