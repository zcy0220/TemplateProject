/**
 * 打开UI指令接收器
 */

using System;
using UnityEngine;
using GameBaseFramework.Base;
using GameBaseFramework.Patterns;
using System.Collections.Generic;

namespace GameGraphics
{
    internal class UIViewOpenCommandReceiver : CommandReceiver
    {
        /// <summary>
        /// 2DUI根节点
        /// </summary>
        private Transform _2DUIGraphicRoot;
        /// <summary>
        /// 3DUI根节点
        /// </summary>
        private Transform _3DUIGraphicRoot;
        /// <summary>
        /// 需要删除的UIView列表
        /// </summary>
        private List<string> _removeUIViews = new List<string>();
        /// <summary>
        /// 当前已经存在的所有UIView
        /// </summary>
        private Dictionary<string, UIViewBase> _curUIViews = new Dictionary<string, UIViewBase>();

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command"></param>
        public override void Execute(Command command)
        {
            OpenView(command as GameLogics.UIViewOpenCommand);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            foreach (var item in _curUIViews)
            {
                if (item.Value.State == EUIViewState.Open)
                {
                    item.Value.Update();
                }
                else
                {
                    _removeUIViews.Add(item.Key);
                }
            }
            if (_removeUIViews.Count > 0)
            {
                for (var i = 0; i < _removeUIViews.Count; i++)
                {
                    _curUIViews.Remove(_removeUIViews[i]);
                }
            }
        }

        /// <summary>
        /// 获取2DUIGraphicRoot
        /// </summary>
        /// <returns></returns>
        private Transform Get2DUIGraphicRoot()
        {
            if (_2DUIGraphicRoot == null)
            {
                var root = GameObject.Find("UIGraphicRoot");
                if (root == null)
                {
                    root = MainGraphic.UnityObjectManager.SyncGameObjectInstantiate("Prefabs/UIGraphics/UIRoots/UIGraphicRoot.prefab");
                    GameObject.DontDestroyOnLoad(root);
                }
                _2DUIGraphicRoot = root.transform.Find("2DUIGraphicRoot");
            }

            return _2DUIGraphicRoot;
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="command"></param>
        private void OpenView(GameLogics.UIViewOpenCommand command)
        {
            if (UIViewConfig.NameToTypeDict.TryGetValue(command.Name, out var type))
            {
                if (_curUIViews.TryGetValue(command.Name, out var view))
                {
                    view.Close();
                }
                else
                {
                    view = Activator.CreateInstance(type) as UIViewBase;
                    _curUIViews.Add(command.Name, view);
                }
                view.Open(Get2DUIGraphicRoot(), command.Data);
            }
            else
            {
                Debuger.LogError($"open view ==> not find {command.Name}");
            }
        }
    }
}
