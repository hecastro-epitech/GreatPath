using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody2D))]
[RequireComponent(typeof (Animator))]
public class BossController : EntityController
{
    [Header("Controller")]
    public GameManager manager;
    public PlayerControler player;

    [Header("Patrol")]
    public Transform[] waypointList;

    public float arrivalDistance = 0.5f;

    public float waitTime = 0;

    public float distanceFollowPlayer = 100f;

    //Privates
    Transform targetWapoint;

    int currenntWaypoint = 0;

    float lastDistanceToTarget = 0f;

    float currentWaitTime = 0f;

    [Header("Experience Reword")]
    public int rewardExperience = 10;

    public int lootGoldMin = 3;

    public int lootGoldMax = 10;

    [Header("Respawn")]
    public GameObject prefab;

    public bool respawn = true;

    public float respawnTime = 5f;

    Rigidbody2D rb2D;

    Animator animator;
    GameObject attackObj;
    SpriteRenderer sprite;

    // public bool allowMoviment = true;
    // Vector2 vector2 = Vector2.zero;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        attackObj = gameObject.transform.Find("AttackArea").gameObject;

        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;
        entity.currentStamina = entity.maxStamina;

        currentWaitTime = waitTime;

        if (waypointList.Length > 0)
        {
            targetWapoint = waypointList[currenntWaypoint];
            lastDistanceToTarget =
                Vector2.Distance(transform.position, targetWapoint.position);
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (entity.dead) return;

        if (entity.currentHealth <= 0)
        {
            entity.currentHealth = 0;
            Dead();
        }

        float input_x = animator.GetFloat("input_x");
        if (input_x < 0)
        {
            attackObj.transform.localPosition = new Vector3(-0.6f, 0, 0);
            attackObj.transform.localScale = new Vector3(-2.2f, 2.2f, 0);
        }
        else if (input_x > 0)
        {
            attackObj.transform.localPosition = new Vector3(0.6f, 0, 0);
            attackObj.transform.localScale = new Vector3(2.2f, 2.2f, 0);
        }

        if (!entity.inCombat)
        {
            float distanceToTarget = Vector2.Distance(transform.position, player.transform.position);

            if(distanceToTarget <= distanceFollowPlayer){
                if(distanceToTarget <= 20f){

                    if (entity.dead) return;
    
                    entity.inCombat = true;
                    entity.target = player.gameObject;
                    
                }
                followPlayer();
            }
            else{
                if (waypointList.Length > 0)
                {
                    Patrulhar();
                }
                else
                {
                    animator.SetBool("isWalking", false);
                }
            }
        }
        else
        {
            if (!entity.combatCoroutine)
            {
                entity.combatCoroutine = true;
                StartCoroutine(Attack());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D colider)
    {
        if (entity.dead) return;

        if (colider.tag == "Player")
        {
            entity.inCombat = true;
            entity.target = colider.gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D colider)
    {
        
        if (colider.tag == "PlayerHitbox")
        {
            TakeDamage(colider.transform.parent.gameObject);
        }
    }
    
    private void TakeDamage(GameObject damageDealer)
    {
        StartCoroutine(FlashDamage());
        Entity damager = damageDealer.GetComponent<EntityController>().entity;
        
        int dmg = manager.CalculateDamage(damager, damager.damage);
        int def = manager.CalculateDefence(entity, entity.defense);
        
        int resultDmg = dmg - def;
        
        if (resultDmg < 0)
        {
            resultDmg = 0;
        }
        
        entity.currentHealth -= resultDmg;
        
        if (entity.currentHealth < 0) entity.currentHealth = 0;
    }
    
    private IEnumerator FlashDamage() {
        sprite.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.1f);
        sprite.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(0.2f);
        sprite.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.1f);
        // sprite.color = new Color(1, 0, 0, 1);
        // yield return new WaitForSeconds(0.1f);
        // sprite.color = new Color(1, 1, 1, 1);
        // yield return new WaitForSeconds(0.1f);
    }
    
    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerExit2D(Collider2D colider)
    {
        if (colider.tag == "Player")
        {
            entity.inCombat = false;
            entity.target = null;
        }
    }

    void followPlayer(){

        if (entity.dead || !allowMoviment)
        {
            return;
        }
        
        float distanceToTarget = Vector2.Distance(transform.position, player.transform.position);

        if(distanceToTarget <= distanceFollowPlayer){
            
            Vector2 direction =
                (player.transform.position - transform.position).normalized;
            animator.SetFloat("input_x", direction.x);
            animator.SetFloat("input_y", direction.y);

            rb2D
                .MovePosition(rb2D.position +
                direction * (entity.speed * Time.fixedDeltaTime));
        }

    }

    void Patrulhar()
    {
        if (entity.dead || !allowMoviment)
        {
            return;
        }

        float distanceToTarget =
            Vector2.Distance(transform.position, targetWapoint.position);

        if (
            distanceToTarget <= arrivalDistance ||
            distanceToTarget >= lastDistanceToTarget
        )
        {
            animator.SetBool("isWalking", false);

            if (currentWaitTime <= 0)
            {
                currenntWaypoint++;

                if (currenntWaypoint >= waypointList.Length)
                {
                    currenntWaypoint = 0;
                }

                targetWapoint = waypointList[currenntWaypoint];

                lastDistanceToTarget =
                    Vector2
                        .Distance(transform.position, targetWapoint.position);

                currentWaitTime = waitTime;
            }
            else
            {
                currentWaitTime -= Time.deltaTime;
            }
        }
        else
        {
            animator.SetBool("isWalking", true);
            lastDistanceToTarget = distanceToTarget;
        }

        Vector2 direction =
            (targetWapoint.position - transform.position).normalized;
        animator.SetFloat("input_x", direction.x);
        animator.SetFloat("input_y", direction.y);

        rb2D
            .MovePosition(rb2D.position +
            direction * (entity.speed * Time.fixedDeltaTime));
    }

    IEnumerator Attack()
    {
        animator.SetTrigger("attack");
        entity.inCombat = true;
        allowMoviment = false; 
        yield return new WaitForSeconds(entity.attackDelay);
        attackObj.SetActive(true);
        yield return new WaitForSeconds(entity.attackTimer);
        attackObj.SetActive(false);
        yield return new WaitForSeconds(entity.attackRecharge);
        entity.inCombat = false;
        allowMoviment = true;
        entity.combatCoroutine = false;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        GameObject newMonster =
            Instantiate(prefab, transform.position, transform.rotation, null);
        newMonster.name = prefab.name;
        newMonster.GetComponent<BossController>().entity.dead = false;

        Destroy(this.gameObject);
    }

    void Dead()
    {
        entity.dead = true;
        entity.inCombat = false;
        attackObj.SetActive(false);
        entity.target = null;
        animator.SetBool("isWalking", false);

        //manager.GainExp(rewardExperience);
        Debug.Log("Inimigo morreu" + gameObject.tag);
        

        animator.SetBool("isDead", true);
        StopAllCoroutines();
        if (entity.respawn)
            StartCoroutine(Respawn());
        
        if (gameObject.tag == "boss") {
            PauseMenu.ActivateYouWon();
        }
    }
}
