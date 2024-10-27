using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static DontDestroyOnLoad Instance;

     private void Awake()
    {
        /*if (Instance != null && Instance != this)
        {
            Instance = this;
            Destroy(Instance.gameObject);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }*/
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
    }
}
