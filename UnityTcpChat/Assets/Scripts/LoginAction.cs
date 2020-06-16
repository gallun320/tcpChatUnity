using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LoginAction : MonoBehaviour
    {
        [SerializeField]
        private Text _loginComp;

        public void OnClick()
        {
            var login = _loginComp.text;
            _loginComp.text = string.Empty;
            EventManager.Instance.RaiseOnLogin(login);
        }
    }
}
