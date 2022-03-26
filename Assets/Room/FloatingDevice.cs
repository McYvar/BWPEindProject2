using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FloatingDevice : MonoBehaviour
{
    [SerializeField] List<Transform> points;
    [SerializeField] GameObject floater;

    [Range(0.1f, 10)] [SerializeField] float speed;
    [Range(0.1f, 10)] [SerializeField] float pause;

    Vector3 destination;
    float timer = 0;
    int size;
    int i = 0;

    public void SetValues(List<Transform> points, float speed, float pause)
    {
        this.points = new List<Transform>();
        this.speed = speed;
        this.pause = pause;

        foreach (Transform point in points)
        {
            this.points.Add(point);
        }
    }

    void Start()
    {
        destination = floater.transform.position;
    }

    private void Update()
    {
        size = points.Count;
        if (size == 0) return;
        if (i >= size) i = 0;
        if (size >= 0)
        {
            if (timer < pause && floater.transform.position == destination)
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

        floater.transform.position = Vector3.MoveTowards(floater.transform.position, destination, speed * Time.deltaTime);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawSphere(points[i].position, 1);
            if (i + 1 < points.Count) Gizmos.DrawLine(points[i].position, points[i + 1].position);
        }
        Gizmos.DrawLine(points[points.Count - 1].position, points[0].position);
    }

    void AddPoint(Transform point)
    {

    }
}
