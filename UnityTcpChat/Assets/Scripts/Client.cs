using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Client : MonoBehaviour
    {
        public string host;
        public int port;
        public static Client Instance { get; private set; }

        public event Action<string> OnMessage;
        public event Action<string> OnMembers;
        public event Action<string> OnLoginError;
        public event Action OnLogin;
        public event Action OnLogout;

        private TcpClient _client;
        private NetworkStream _clientStream;
        private CancellationTokenSource _cancellationSource;
        private Timer _timer;
        private Task _readTask;
        private bool _isSending;
        private bool _isDisconnect;

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this);
            _cancellationSource = new CancellationTokenSource();
            Connect();
        }

        private void Connect()
        {
            _readTask = Task.Run(async () =>
            {
                await ConnectAsync();

                while (_client.Connected)
                {
                    await ReadFromServerAsync();
                }

                while (!_client.Connected && !_isDisconnect)
                {
                    await ConnectAsync();
                    await Task.Delay(200);
                }
            });
        }

        private async Task ConnectAsync()
        {
            _client = new TcpClient();


            var connect = _client.ConnectAsync(IPAddress.Parse(host), port).Wait(100);

            await Task.Delay(100);

            if (!_client.Connected)
            {
                connect = _client.ConnectAsync(IPAddress.Parse(host), port).Wait(300);
                await Task.Delay(300);
                if (!_client.Connected)
                {
                    _isDisconnect = true;
                    RaiseOnResponse("error:0:server not active");
                    return;
                }
            }

            _clientStream = _client.GetStream();
        }

        public void WriteToServer(string message)
        {
            if(!_isSending)
            {
                _isSending = !_isSending;
                Task.Run(async () => await WriteToServerAsync(message));
            }
            
        }


       private async Task WriteToServerAsync(string message)
       {
            if(!_client.Connected)
            {
                await ConnectAsync();
                _readTask = Task.Run(async () =>
                {
                    while (_client.Connected)
                    {
                        await ReadFromServerAsync();
                    }

                    while (!_client.Connected && !_isDisconnect)
                    {
                        await ConnectAsync();
                        await Task.Delay(200);
                    }
                });
            }

            if (_client.Connected)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await _clientStream.WriteAsync(buffer, 0, buffer.Length)
                    .ContinueWith(t => _isSending = !_isSending);
            }
        }

        private async Task ReadFromServerAsync()
        {
            var buffer = new byte[4096];
            var task = _clientStream.ReadAsync(buffer, 0, buffer.Length);
            task.Wait(100);
            await Task.Delay(100);
            if(task.IsCompleted)
            {
                var response = Encoding.UTF8.GetString(buffer, 0, task.Result);

                if (!string.IsNullOrEmpty(response))
                {
                    RaiseOnResponse(response);
                }
            }
            else
            {
                task.GetAwaiter().OnCompleted(() =>
                {
                    var response = Encoding.UTF8.GetString(buffer, 0, task.Result);

                    if (!string.IsNullOrEmpty(response))
                    {
                        RaiseOnResponse(response);
                    }
                });
            }
        }

        private void RaiseOnResponse(string message)
        {
            var rawMessage = message.Split(':');
            var type = rawMessage[0];

            if(type.Contains("error"))
            {
                OnLoginError?.Invoke(rawMessage[2]);
                return;
            }

            if (type.Contains("shutdown"))
            {
                OnLoginError?.Invoke(rawMessage[2]);
                return;
            }

            if (type.Contains("login"))
            {
                OnLogin?.Invoke();
                return;
            }

            if (type.Contains("logout"))
            {
                DestroyRunTask();
                OnLogout?.Invoke();
                return;
            }

            if (type.Contains("message"))
            {
                OnMessage?.Invoke(rawMessage[2]);
                return;
            }

            if (type.Contains("members"))
            {
                OnMembers?.Invoke(rawMessage[2]);
                return;
            }
        }


        private void DestroyRunTask()
        {
            _client.Close();
            _clientStream.Close();
            _cancellationSource.Cancel();
            if(_readTask.IsCompleted || _readTask.IsCanceled)
            {
                _readTask.Dispose();
                _readTask = null;
            }
        }
    }
}
