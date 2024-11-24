using Assets.Scripts.Dungeon;
using System.Collections;
using System.Collections.Generic;
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
