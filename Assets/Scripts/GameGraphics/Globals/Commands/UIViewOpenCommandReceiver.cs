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
        /// UI优先级根节点
        /// </summary>
        private Dictionary<int, Transform> _2DUIPriorityNodeDict = new Dictionary<int, Transform>();
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
        /// 根据界面名，查找对应界面指定的优先级节点
        /// </summary>
        /// <param name="uiViewName"></param>
        /// <returns></returns>
        private Transform Get2DUIPriorityNode(int priority)
        {
            if (_2DUIPriorityNodeDict.TryGetValue(priority, out var node))
            {
                return node;
            }
            var root = Get2DUIGraphicRoot();
            var nodeName = "PriorityNode" + priority;
            node = root.Find(nodeName);
            if (node == null)
            {
                var rectTr = new GameObject().AddComponent<RectTransform>();
                rectTr.gameObject.name = nodeName;
                rectTr.SetParent(root);
                rectTr.anchorMin = Vector2.zero;
                rectTr.anchorMax = Vector2.one;
                rectTr.offsetMin = Vector2.zero;
                rectTr.offsetMax = Vector2.zero;
                node = rectTr;
            }
            return node;
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="command"></param>
        private void OpenView(GameLogics.UIViewOpenCommand command)
        {
            if (UIViewConfig.NameToTypeDict.TryGetValue(command.Name, out var type))
            {
                switch (command.ViewOpenType)
                {
                    case GameLogics.EUIViewOpenType.Replace:
                        //关闭除永久存在之外的所有UIView，在打开当前界面
                        foreach (var item in _curUIViews)
                        {
                            if (item.Value.OpenType != GameLogics.EUIViewOpenType.Forever)
                            {
                                item.Value.Close();
                            }
                        }
                        break;
                    case GameLogics.EUIViewOpenType.Overlying:
                        if (_curUIViews.ContainsKey(command.Name))
                        {
                            _curUIViews[command.Name].Close();
                        }
                        break;
                    case GameLogics.EUIViewOpenType.Forever:
                        if (_curUIViews.ContainsKey(command.Name))
                        {
                            return;
                        }
                        break;
                }
                var parent = Get2DUIPriorityNode((int)command.ViewPriority);
                var view = Activator.CreateInstance(type) as UIViewBase;
                _curUIViews.Add(command.Name, view);
                view.Open(parent, command.Data);
            }
            else
            {
                Debuger.LogError($"open view ==> not find {command.Name}");
            }
        }
    }
}
