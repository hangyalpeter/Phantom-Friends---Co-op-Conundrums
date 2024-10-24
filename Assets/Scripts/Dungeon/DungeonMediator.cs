using UnityEngine;

public class DungeonMediator : MonoBehaviour, IDungeonMediator
{
    [SerializeField]
    private PlayerManager playerManager;

    [SerializeField]
    private RoomsManager roomManager;

    [SerializeField]
    private EnemyManager enemyManager;

    [SerializeField]
    private DungeonManager dungeonManager;


    private void Awake()
    {
        playerManager.SetMediator(this);
        roomManager.SetMediator(this);
        enemyManager.SetMediator(this);
        dungeonManager.SetMediator(this);
    }
  
    public void Notify(Component sender, DungeonEvents eventCode)
    {
        switch (eventCode)
        {
            case DungeonEvents.PlayerDied:
                dungeonManager.HandlePlayerDeath();
                break;
            case DungeonEvents.EnemyDied:
                dungeonManager.IncrementKilledEnemies();
                break;
            case DungeonEvents.DungeonWin:
                dungeonManager.HandleDungeonWin();
                playerManager.ResetHealth();
                playerManager.IncreaseDamage(1.1f);
                break;
        }
    }

    public T GetManager<T>() where T : class
    {
        if (typeof(T) == typeof(RoomsManager)) return roomManager as T;
        if (typeof(T) == typeof(EnemyManager)) return enemyManager as T;
        if (typeof(T) == typeof(PlayerManager)) return playerManager as T;
        if (typeof(T) == typeof(DungeonManager)) return dungeonManager as T;
        return null;
    }
}
public enum DungeonEvents
{
    PlayerDied,
    EnemyDied,
    PlayerEnteredRoom,
    PlayerEnteredBossRoom,
    DungeonWin,
    RoomCleared
}

