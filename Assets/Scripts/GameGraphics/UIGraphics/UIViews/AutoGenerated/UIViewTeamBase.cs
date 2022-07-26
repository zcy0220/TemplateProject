/**
 * AutoGenerated Code By UIPrefabCodeGenerate ==> CodeGenerate
 */

using UnityEngine.UI;

namespace GameGraphics
{
	public class UIViewTeamBase : UIViewBase
	{
		protected Button _buttonCreateRoom;
		protected virtual void OnButtonCreateRoomClick(){ }
		protected Button _buttonJoinGame;
		protected virtual void OnButtonJoinGameClick(){ }
		protected Button _buttonClose;
		protected virtual void OnButtonCloseClick(){ }
		protected override void BindWidgets()
		{
			_buttonCreateRoom = _root.transform.Find("BackgroundPanel/<Button>CreateRoom").GetComponent<Button>();
			_buttonCreateRoom.onClick.AddListener(OnButtonCreateRoomClick);
			_buttonJoinGame = _root.transform.Find("BackgroundPanel/<Button>JoinGame").GetComponent<Button>();
			_buttonJoinGame.onClick.AddListener(OnButtonJoinGameClick);
			_buttonClose = _root.transform.Find("BackgroundPanel/<Button>Close").GetComponent<Button>();
			_buttonClose.onClick.AddListener(OnButtonCloseClick);
		}
		protected override string GetPrefabPath()
		{
			return "Prefabs/UIGraphics/UIViews/UIViewTeam.prefab";
		}
	}
}
