using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static DontDestroyOnLoad Instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
    }
}
