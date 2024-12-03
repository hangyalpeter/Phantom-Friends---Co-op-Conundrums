using Assets.Scripts.Dungeon;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{

    [SerializeField]
    private DungeonGeneratorStrategyExperiment dungeonGenerator;

    public void RunProceduralGeneration()
    {
        dungeonGenerator.RunProceduralGeneration();
    }
}
