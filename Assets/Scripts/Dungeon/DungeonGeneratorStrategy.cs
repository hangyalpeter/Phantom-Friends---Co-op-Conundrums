using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Dungeon
{
    public abstract class DungeonGeneratorStrategy : NetworkBehaviour
    {
        public abstract void RunProceduralGeneration();
    }
}
