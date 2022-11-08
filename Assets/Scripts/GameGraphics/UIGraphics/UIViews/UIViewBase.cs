/**
 * UI视图基类 
 */

using UnityEngine;

namespace GameGraphics
{
    public enum EUIViewState
    {
        Open,
        Close
    }

    public class UIViewBase
    {
        /// <summary>
        /// UIView的状态
        /// </summary>
        public EUIViewState State { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public GameLogics.EUIViewOpenType OpenType { get; set; }
        /// <summary>
        /// 预制体实例引用
        /// </summary>
        protected GameObject _root;
        /// <summary>
        /// 父节点
        /// </summary>
        protected Transform _parent;

        /// <summary>
        /// 打开界面
        /// </summary>
        public void Open(Transform parent, object data = null)
        {
            _parent = parent;
            _root = MainGraphic.UnityObjectManager.SyncGameObjectInstantiate(GetPrefabPath(), _parent);
            State = EUIViewState.Open;
            BindWidgets();
            OnEnter(data);
        }

        /// <summary>
        /// UI直接相互直接打开接口
        /// </summary>
        public void Open(string name, GameLogics.EUIViewOpenType openType = GameLogics.EUIViewOpenType.Replace)
        {
            var command = new GameLogics.UIViewOpenCommand();
            command.Name = name;
            command.ViewOpenType = openType;
            MainGraphic.Input(command);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        public void Close()
        {
            State = EUIViewState.Close;
            OnExit();
            if (_root != null)
            {
                GameObject.Destroy(_root);
            }
        }

        /// <summary>
        /// 绑定所有控件
        /// </summary>
        protected virtual void BindWidgets() { }

        /// <summary>
        /// 预制体路径
        /// </summary>
        /// <returns></returns>
        protected virtual string GetPrefabPath() { return ""; }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// 进入界面
        /// </summary>
        protected virtual void OnEnter(object data = null) { }
        
        /// <summary>
        /// 退出界面
        /// </summary>
        protected virtual void OnExit() { }
    }
}