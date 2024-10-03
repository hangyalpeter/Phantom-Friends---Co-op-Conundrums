using UnityEngine;

public interface IDungeonMediator
{
    void Notify(Component sender, DungeonEvents eventCode);
    T GetManager<T>() where T : class;

}
