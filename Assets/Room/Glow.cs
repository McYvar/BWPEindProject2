using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour
{
    public float timer;
    Material mat;
    // Start is called before the first frame update
    void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 0)
        {
            mat.DisableKeyword("_EMISSION");
        }
        else
        {
            mat.EnableKeyword("_EMISSION");
            timer -= Time.deltaTime;
        }
    }
}
