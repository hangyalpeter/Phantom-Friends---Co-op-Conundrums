using System;
using System.Collections;
using UnityEngine;

// Copied over from the official UQuiz Unity tutorial project
// https://assetstore.unity.com/packages/essentials/tutorial-projects/quizu-a-ui-toolkit-sample-268492
// downloaded from the Unity Asset Store on 2024-02-19
public static class Coroutines
{
    private static MonoBehaviour s_CoroutineRunner;

    public static bool IsInitialized => s_CoroutineRunner != null;

    public static void Initialize(MonoBehaviour runner)
    {
        s_CoroutineRunner = runner;
    }

    public static Coroutine StartCoroutine(IEnumerator coroutine)
    {
        if (s_CoroutineRunner == null)
        {
            throw new InvalidOperationException("CoroutineRunner is not initialized.");
        }

        return s_CoroutineRunner.StartCoroutine(coroutine);
    }

    public static void StopCoroutine(Coroutine coroutine)
    {
        if (s_CoroutineRunner != null)
        {
            s_CoroutineRunner.StopCoroutine(coroutine);
        }
    }

    public static void StopCoroutine(ref Coroutine coroutine)
    {
        if (s_CoroutineRunner != null && coroutine != null)
        {
            s_CoroutineRunner.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}
