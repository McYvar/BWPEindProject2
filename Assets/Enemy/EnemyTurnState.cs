using UnityEngine;

public class EnemyTurnState : BaseState
{
    Enemy enemy;
    [SerializeField] GameObject cam;

    float timer;

    public override void OnAwake()
    {
        enemy = GetComponent<Enemy>();
        cam = FindObjectOfType<CameraBehaviour>().gameObject;
    }


    public override void OnEnter()
    {
        timer = 0;
    }


    public override void OnExit()
    {
        GameStates.ableToDequeue = true;
    }


    public override void OnUpdate()
    {
        for (int i = enemy.GetTurns(); i >= 0; i--)
        {
            if (timer > 3 && cam.transform.position == transform.position)
            {
                Move();
                timer = 0;
            }
            else timer += Time.deltaTime;
            if (i == 0)
            {
                enemy.isTurn = false;
                stateManager.SwitchState(typeof(EnemyIdleState));
            }
        }

        cam.GetComponent<CameraBehaviour>().objectToFollow = gameObject;

    }


    public void Move()
    {

    }
}
