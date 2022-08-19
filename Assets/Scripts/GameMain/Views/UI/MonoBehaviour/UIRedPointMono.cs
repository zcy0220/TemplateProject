/**
 * 红点Mono
 */

using UnityEngine;
using UnityEngine.UI;
using GameMain.Modules;
using GameBaseFramework.Base;

namespace GameMain.Views.UI
{
    public class UIRedPointMono : MonoBehaviour
    {
        /// <summary>
        /// 该红点关注的模块
        /// </summary>
        public ERedPointModule Module;
        /// <summary>
        /// 红点类型
        /// </summary>
        public ERedPointType RedPointType;
        /// <summary>
        /// 显示数量的文本
        /// </summary>
        private Text _textNum;
        /// <summary>
        /// 红点数据
        /// </summary>
        private DataValue<int> _redPoint;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Awake()
        {
            _redPoint = G.DataModuleManager.GetRedPointNum(Module);
            _redPoint += OnRedPointNumChange;
            _textNum = transform.GetChild(0).GetComponent<Text>();
            OnRedPointNumChange(_redPoint.Get());
        }

        /// <summary>
        /// 红点数量监听
        /// </summary>
        /// <param name="pointNum"></param>
        private void OnRedPointNumChange(int pointNum)
        {
            if (pointNum == 0)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                _textNum.gameObject.SetActive(RedPointType == ERedPointType.NumRedPoint);
                _textNum.text = pointNum.ToString();
            }
        }

        /// <summary>
        /// 销毁删除监听
        /// </summary>
        private void OnDestroy()
        {
            if (_redPoint != null)
            {
                _redPoint -= OnRedPointNumChange;
            }
        }
    }
}