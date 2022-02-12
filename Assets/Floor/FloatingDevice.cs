using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingDevice : MonoBehaviour
{
    public List<GameObject> points;
    public float speed;
    public float pause;

    Vector3 destination;
    float timer = 0;
    int size;
    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        size = points.Count;
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (i >= size) i = 0;
        if (size >= 0)
        {
            if (timer < pause && transform.position == destination)
            {
                destination = points[i].transform.position;
                timer += Time.deltaTime;
            }
            else if (timer >= pause)
            {
                timer = 0;
                i++;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }
}
