using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Dungeon
{
    public abstract class DungeonGeneratorStrategy : NetworkBehaviour
    {
        public abstract void RunProceduralGeneration();
    }

    public abstract class DungeonGeneratorStrategyExperiment : MonoBehaviour
    {
        public abstract void RunProceduralGeneration();
    }

}
