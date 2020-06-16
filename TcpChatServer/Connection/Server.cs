using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpChatServer.Configs;
using TcpChatServer.Requests;

namespace TcpChatServer.Connection
{
    public class Server : IDisposable
    {
        private TcpListener _listener;
        private readonly ConcurrentDictionary<string, Client> _clients;

        public Server()
        {
            _clients = new ConcurrentDictionary<string, Client>();
            InitServer();
            Task.Run(async () => await RunAccpetingAsync());
        }

        private void InitServer()
        {
            var config = ConfigLoader.GetConfig();
            var host = IPAddress.Parse(config.Host);
            _listener = new TcpListener(host, config.Port);
            Console.WriteLine("Server init");
        }

        private async Task RunAccpetingAsync()
        {
            Console.WriteLine("Server run");
            _listener.Start();
            while(true)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
                Task.Run(async () => 
                { 
                    while (true) 
                    {
                        await ClientProcessAsync(tcpClient);
                        await Task.Delay(100);
                    }
                });
            }
        }

        private async Task ClientProcessAsync(TcpClient client)
        {
            var c = new Client(client);
            var request = await c.ProcessAsync();
            if (_clients.ContainsKey(request.Login) && request is AuthClientRequest)
            {
                await c.WriteError()
                    .ContinueWith(task =>
                    {
                        c.Dispose();
                        c = null;
                        client.Close();
                    });
                return;
            }

            if(request is AuthClientRequest)
            {
                var checkAdd = _clients.TryAdd(request.Login, c);
                if(checkAdd)
                {
                    await c.WriteAuthComplete();
                }
            }

            if(!_clients.ContainsKey(request.Login))
            {
                await c.WriteError();
                return;
            }

            if (request is ChatClientRequest)
            {
                foreach(var cl in _clients)
                {
                    await cl.Value.WriteChatMessage($"{request.Login};{request.Message}");
                }
            }

            if(request is ErrorClientRequest)
            {
                await c.WriteError();
            }

            if(request is LogoutClientRequest)
            {
                await c.WriteLogoutComplete()
                    .ContinueWith(task =>
                    {
                        if (_clients.TryRemove(request.Login, out var cl))
                        {
                            cl.Dispose();
                            //client.Close();
                        }
                    });
            }
        }

        public void Dispose()
        {
            foreach (var client in _clients)
            {
                client.Value.WriteShutdown();
            }

            foreach (var client in _clients)
            {
                client.Value.Dispose();
            }
        }
    }
}
