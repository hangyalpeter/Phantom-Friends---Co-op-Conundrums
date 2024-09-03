using Assets.Scripts.Dungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{

    [SerializeField]
    private DungeonGeneratorStrategy dungeonGenerator;

    public void RunProceduralGeneration()
    {
        dungeonGenerator.RunProceduralGeneration();
    }
}
