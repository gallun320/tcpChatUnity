using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ChatFlow : MonoBehaviour
    {

        [SerializeField]
        private GameObject _prefab;
        [SerializeField]
        private ScrollRect _scrollRectComponent;
        [SerializeField]
        private Text _textComponent;
        private List<GameObject> _intantiatedList;
        private string _login;

        private void Awake()
        {
            //_login = PlayerPrefs.GetString("ChatLogin");
            _intantiatedList = new List<GameObject>();
            EventManager.Instance.OnLogin += SetLogin;
            EventManager.Instance.OnLogout += SetLogout;
            EventManager.Instance.OnMessage += SetMessage;
            Client.Instance.OnLogin += Login;
            Client.Instance.OnLogout += Logout;
            Client.Instance.OnLoginError += LoginError;
            Client.Instance.OnMessage += Message;
            Client.Instance.OnMembers += Members;
        }

        private void Update()
        {
            if(_intantiatedList.Count > 3)
            {
                Canvas.ForceUpdateCanvases();
                _scrollRectComponent.verticalNormalizedPosition = 0f;
                Canvas.ForceUpdateCanvases();
            }
        }

        private void SetMessage(string obj)
        {
            Client.Instance.WriteToServer($"message:{_login}:{obj}");
        }

        private void SetLogout()
        {
            if (!string.IsNullOrEmpty(_login))
            {
                //PlayerPrefs.SetString("ChatLogin", "");
                Client.Instance.WriteToServer($"logout:{_login}:0");
                _login = "";
            }
        }

        private void SetLogin(string obj)
        {
            if (string.IsNullOrEmpty(_login))
            {
                _login = obj;
                //PlayerPrefs.SetString("ChatLogin", _login);
                //PlayerPrefs.Save();
                Client.Instance.WriteToServer($"auth:{_login}:0");
            }
        }

        private void Message(string obj)
        {
            var rawMessage = obj.Split(';');
            var name = rawMessage[0];
            var message = rawMessage[1];

            UnityMainThreadDispatcher.Instance().Enqueue(InitChatMessage(name, message));
        }

        private IEnumerator InitChatMessage(string name, string message)
        {
            var item = Instantiate(_prefab, transform);
            var texts = item.GetComponentsInChildren<Text>();
            texts[0].text = name;
            texts[1].text = message;
            _intantiatedList.Add(item);

            yield return null;
        }


        private void LoginError(string error)
        {
            Debug.LogError(error);
            UnityMainThreadDispatcher.Instance().Enqueue(() => _textComponent.text = "LoginError");
        }

        private void Logout()
        {
            Debug.Log("Logout");
            UnityMainThreadDispatcher.Instance().Enqueue(() => _textComponent.text = "LogoutComplete");
        }

        private void Login()
        {
            Debug.Log("Login");
            UnityMainThreadDispatcher.Instance().Enqueue(() => _textComponent.text = "LoginComplete");
        }

        private void Members(string obj)
        {
            Debug.Log("Members");
            UnityMainThreadDispatcher.Instance().Enqueue(() => _textComponent.text = obj);
        }

        private void OnDestroy()
        {
            SetLogout();
        }
    }
}
