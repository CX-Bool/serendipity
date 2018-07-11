using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace view
{
    public class FloatingText : MonoBehaviour
    {

        /// <summary>
        /// 自身文本
        /// </summary>
        private Text _txt;
        static bool isActive = false;
        void Awake()
        {
            _txt = GetComponent<Text>();
        }
        private void FixedUpdate()
        {
            if (isActive)
            {
                _txt.transform.position += new Vector3(0.2f, 0.2f);

            }
        }
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="msg">文本显示内容</param>
        /// <param name="color">文本颜色</param>
        public void SetText(string msg, Color color)
        {
            _txt.color = color;
            _txt.text = msg;
            _txt.CrossFadeAlpha(0.1f, 2, true);

            SetActive();
        }
        public void SetActive()
        {
            isActive = true;

            Destroy(gameObject,3);
        }
        /// <summary>
        /// 设置文本位置
        /// </summary>
        /// <param name="position">文本所在位置</param>
        public void SetPos(Vector3 position)
        {
            //世界坐标转为屏幕坐标
            _txt.transform.position = Camera.main.WorldToScreenPoint(position);
        }

   
    }
}
