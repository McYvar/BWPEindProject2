using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInAndDestroy : MonoBehaviour
{
    [SerializeField] float secondsTillDestroy;
    [SerializeField] float fadeChangeSpeed;
    float timer = 0;

    private void Update()
    {
        if (timer > secondsTillDestroy) Destroy(gameObject);
        else timer += Time.deltaTime;
    }
}
