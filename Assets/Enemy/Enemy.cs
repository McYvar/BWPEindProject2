using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] EnemyInfo info;
    [SerializeField] GameObject hpText;
    [SerializeField] int itemSpawnChance;
    [SerializeField] int spellSpawnChance;
    TMP_Text hp;

    public string enemyName;
    private int turns;
    public bool isTurn;
    public bool isJumperEnemy;
    public float attackRange;
    public float detectRange;
    public int dealsDamage;

    public int healt { get; set; }

    public FiniteStateMachine fsm;

    CameraBehaviour cam;

    private void Start()
    {
        cam = FindObjectOfType<CameraBehaviour>();
        fsm = new FiniteStateMachine(typeof(EnemyIdleState), GetComponents<BaseState>());
        turns = info.turns;
        setHealth(info.health);
        enemyName = info.enemyName;
        attackRange = info.attackRange;
        isJumperEnemy = info.doesEnemyJump;
        detectRange = info.detectRange;
        dealsDamage = info.dealsDamage;
        hp = hpText.GetComponent<TMP_Text>();
    }


    private void Update()
    {
        hp.text = "HP: " + healt;
        hpText.transform.LookAt(cam.actualCamera.transform.position);
        if (MenuSystem.invIsOpen || MenuSystem.gameIsPaused) return;
        fsm.OnUpdate();

        if (healt <= 0)
        {
            float r = Random.value * 100;
            if (r > spellSpawnChance)
            {
                EventManager<Vector3>.InvokeEvent(EventType.ON_ENEMY_DEATH_SPAWN_SPELL, transform.position - transform.up);
            }
            else if (r > itemSpawnChance)
            {
                EventManager<Vector3>.InvokeEvent(EventType.ON_ENEMY_DEATH_SPAWN_ITEM, transform.position - transform.up);
            }
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        turns = 0;
    }


    public int GetTurns()
    {
        return turns;
    }
    

    public void setHealth(int amount)
    {
        healt = amount;
    }

    public void takeDamage(int amount)
    {
        healt -= amount;
        StartCoroutine(FindObjectOfType<MenuSystem>().DisplayThenRemoveChat(enemyName + " took: " + amount + " damage!"));
    }

}
