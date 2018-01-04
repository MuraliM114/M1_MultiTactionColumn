using UnityEngine;
using System.Collections;

public class DestroySelf : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(iDestroy());
    }

    IEnumerator iDestroy()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        Destroy(gameObject);
    }
}
