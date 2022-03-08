using UnityEngine;

public class Player : MonoBehaviour
{
    private FiniteStateMachine finiteStateMachine;

    public GameObject cam;
    public bool camFollow = true;
    Vector3 camVelocity = Vector3.zero;
    public float camSmoothSpeed, camSpeed;
    bool mouseCheat;

    public void Awake()
    {
        finiteStateMachine = new FiniteStateMachine(typeof(PlayerTurnState), GetComponents<BaseState>());
    }


    void LateUpdate()
    {
        if (camFollow)
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, transform.position, ref camVelocity, camSmoothSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.Z)) cam.transform.Rotate(0, 1 * camSpeed * Time.deltaTime, 0);
            if (Input.GetKey(KeyCode.X)) cam.transform.Rotate(0, -1 * camSpeed * Time.deltaTime, 0);
            if (mouseCheat)
            {
                cam.transform.Rotate(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
                cam.transform.rotation = Quaternion.Euler(cam.transform.localEulerAngles.x, cam.transform.localEulerAngles.y, 0);
            }
        }
    }


    private void Update()
    {
        MouseCheatCheck();
        finiteStateMachine.OnUpdate();
    }

    public void FixedUpdate()
    {
        finiteStateMachine.OnFixedUpdate();
    }


    void MouseCheatCheck()
    {
        if (Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.Alpha3) && Input.GetKey(KeyCode.T))
        {
            mouseCheat = true;
        }
    }
}
