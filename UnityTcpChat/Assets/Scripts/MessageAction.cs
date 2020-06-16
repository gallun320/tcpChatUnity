using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MessageAction : MonoBehaviour
    {
        [SerializeField]
        private Text _inputText;
        private bool _isSending;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return) && !_isSending)
            {
                _isSending = true;
            }
        }

        private void FixedUpdate()
        {
            if(_isSending)
            {
                _isSending = false;
                var text = _inputText.text;
                _inputText.text = "";
                EventManager.Instance.RaiseOnMesssage(text);
            }
        }
    }
}
