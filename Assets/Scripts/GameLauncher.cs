/**
 * 游戏启动项
 */

using UnityEngine;
using BattleSystem;
using GameFramework.Utils;

public class GameLauncher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var playerData = new PlayerData();
        playerData.Id = 1;
        playerData.TeamId = 1;
        playerData.UserId = 1001;
        playerData.Name = "测试名字A";
        var param = new BattleParam();
        param.Players.Add(playerData);

        var battle = new Battle();
        battle.Start(param);
        MonoUpdaterManager.Instance.AddFixedUpdateListener(battle.UpdateFrame);
    }
}
