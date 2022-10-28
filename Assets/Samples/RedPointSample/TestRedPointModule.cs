
using UnityEngine;
using UnityEngine.UI;
using GameMain;
using GameMain.Modules;

namespace GameTest
{
    public class TestRedPointModule : MonoBehaviour
    {
        /// <summary>
        /// 测试红点的主界面prefab
        /// </summary>
        public GameObject UITestRedPontMainPrefab;
        /// <summary>
        /// 测试红点的邮件界面prefab
        /// </summary>
        public GameObject UITestRedPointMailPrefab;
        /// <summary>
        /// UICanvas
        /// </summary>
        public Transform UICanvas;
        //=========================================
        private GameObject _testRedPontMain;
        private GameObject _testRedPointMail;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Awake()
        {
            OpenUITestRedPontMain();
        }

        /// <summary>
        /// 打开UITestRedPontMain
        /// </summary>
        public void OpenUITestRedPontMain()
        {
            if (_testRedPointMail != null)
            {
                GameObject.Destroy(_testRedPointMail);
            }
            _testRedPontMain = GameObject.Instantiate(UITestRedPontMainPrefab, UICanvas);
            _testRedPontMain.transform.Find("<Button>Mail").GetComponent<Button>().onClick.AddListener(OpenUITestRedPontMail);
        }

        /// <summary>
        /// 打开邮件界面测试
        /// </summary>
        public void OpenUITestRedPontMail()
        {
            if (_testRedPontMain != null)
            {
                GameObject.Destroy(_testRedPontMain);
            }
            _testRedPointMail = GameObject.Instantiate(UITestRedPointMailPrefab, UICanvas);
            _testRedPointMail.transform.Find("<Button>Close").GetComponent<Button>().onClick.AddListener(OpenUITestRedPontMain);
            _testRedPointMail.transform.Find("<Button>MailSystem").GetComponent<Button>().onClick.AddListener(OnButtonMailSystem);
            _testRedPointMail.transform.Find("<Button>MailFriend").GetComponent<Button>().onClick.AddListener(OnButtonMailFriend);
        }

        /// <summary>
        /// 系统邮件按钮回调
        /// </summary>
        public void OnButtonMailSystem()
        {
            //G.DataModuleManager.TestAddRedPointNum(ERedPointModule.MailSystem);
        }

        /// <summary>
        /// 好友邮件回调
        /// </summary>
        public void OnButtonMailFriend()
        {
            //G.DataModuleManager.TestAddRedPointNum(ERedPointModule.MailFriend);
        }
    }
}