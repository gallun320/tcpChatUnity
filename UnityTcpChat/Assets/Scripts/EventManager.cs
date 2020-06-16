using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this);
        }

        public void RaiseOnLogin(string login)
        {
            OnLogin?.Invoke(login);
        }

        public event Action<string> OnLogin;

        public void RaiseOnLogout()
        {
            OnLogout?.Invoke();
        }

        public event Action OnLogout;

        public void RaiseOnMesssage(string message)
        {
            OnMessage?.Invoke(message);
        }

        public event Action<string> OnMessage;
    }
}
