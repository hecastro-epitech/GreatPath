using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Rigidbody2D))]
[RequireComponent(typeof (PlayerControler))]
public class PlayerControler : EntityController
{
    public Animator playerAnimator;
    public FixedJoystick fJoystick;
 
    float input_x = 0;

    float input_y = 0;

    bool isWalking = false;
    
    bool isAttacking = false;
    
    private GameObject auroraButton;

    private GameObject callButton;

    [Header("Player UI")]
    public Slider health;

    [Header("Player Regeneration")]
    public bool regenerateHp = true;

    public int regenerateHPValue = 5;

    public int regenerateHPTime = 2;
    
    [Header("Game Manager")]
    public GameManager manager;

    // public bool allowMoviment = true;
    Rigidbody2D rb2D;
    GameObject attackObj;
    SpriteRenderer sprite;
    Vector2 moviment = Vector2.zero;

    bool CheckCloseToTag(string tag, float minimumDistance)
    {
        GameObject[] goWithTag = GameObject.FindGameObjectsWithTag(tag);

        for (int i = 0; i < goWithTag.Length; ++i)
        {
            if (
                Vector3
                    .Distance(transform.position,
                    goWithTag[i].transform.position) <=
                minimumDistance
            ) return true;
        }

        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        isWalking = false;
        rb2D = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        attackObj = gameObject.transform.Find("AttackArea").gameObject;
        auroraButton = GameObject.Find("AuroraButton");
        callButton = GameObject.Find("CallButton");

        if (manager == null)
        {
            Debug.LogError("instancie o gameManager para essa entidade");
            return;
        }

        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);
        entity.maxStamina = manager.CalculateStamina(entity);

        int dmg = (int) manager.CalculateDamage(entity, 7);
        int def = (int) manager.CalculateDefence(entity, 4);

        entity.currentHealth = entity.maxHealth;

        entity.currentStamina = entity.maxStamina;

        health.value = (float) entity.currentHealth;

        StartCoroutine(RegenerateHealth());
    }

    // Update is called once per frame
    void Update()
    {
        if (entity.dead) return;

        if (entity.currentHealth <= 0)
        {
            entity.currentHealth = 0;
            Dead();
        }

        auroraButton
            .SetActive(CheckCloseToTag("aurora", 30) &&
            !GameObject.Find("PauseMenu"));

        if (auroraButton.activeSelf)
            callButton.SetActive(false);
        else
            callButton.SetActive(true);

        health.value = (float) entity.currentHealth / (float) entity.maxHealth;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            entity.currentHealth -= 1;
        }

        if (!allowMoviment) {
            input_x = 0;
            input_y = 0;
        } else {
            input_x = Input.GetAxisRaw("Horizontal");
            input_y = Input.GetAxisRaw("Vertical");
            
            if (fJoystick != null) {
                input_x += fJoystick.Horizontal;
                input_y += fJoystick.Vertical;
            } 
        }
        
        isWalking = (input_x != 0 || input_y != 0);
        moviment = new Vector2(input_x, input_y);

        if (isWalking)
        {
            playerAnimator.SetFloat("input_x", input_x);
            playerAnimator.SetFloat("input_y", input_y);
        }
        
        if (input_x > 0)
        {
            attackObj.transform.localPosition = new Vector3(1, 0, 0);
            attackObj.transform.localScale = new Vector3(5, 5, 0);
        }
        else if (input_x < 0)
        {
            attackObj.transform.localPosition = new Vector3(-1, 0, 0);
            attackObj.transform.localScale = new Vector3(-5, 5, 0);
        }

        playerAnimator.SetBool("isWalking", isWalking);

        if (Input.GetButtonDown("Fire1") && !isAttacking && allowMoviment)
            StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        playerAnimator.SetTrigger("attack");
        isAttacking = true;
        allowMoviment = false;
        yield return new WaitForSeconds(entity.attackDelay);
        attackObj.SetActive(true);
        yield return new WaitForSeconds(entity.attackTimer);
        attackObj.SetActive(false);
        allowMoviment = true;
        yield return new WaitForSeconds(entity.attackRecharge);
        isAttacking = false;
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

    IEnumerator RegenerateHealth()
    {
        while (!entity.dead)
        {
            if (regenerateHp)
            {
                if (entity.currentHealth < entity.maxHealth)
                {
                    entity.currentHealth += regenerateHPValue;
                    yield return new WaitForSeconds(regenerateHPTime);
                }
                else
                {
                    yield return null;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private void FixedUpdate()
    {
        rb2D
            .MovePosition(rb2D.position +
            moviment * entity.speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("buffvida"))
        {
            Destroy(collision.gameObject);
            entity.maxHealth += 60;
            entity.currentHealth += 60;
            print("MaxLife was buffed.");
        }
        else if (collision.gameObject.CompareTag("buffdefesa"))
        {
            Destroy(collision.gameObject);
            entity.defense += 1;
            print("Defense was buffed.");
        }
        else if (collision.gameObject.CompareTag("buffataque"))
        {
            Destroy(collision.gameObject);
            entity.damage += 2;
            print("Attack was buffed.");
        }
        else if (collision.gameObject.CompareTag("EnemyHitbox"))
        {
            TakeDamage(collision.gameObject.transform.parent.gameObject);
        }
    }

    void Dead()
    {
        entity.dead = true;
        entity.inCombat = false;
        attackObj.SetActive(false);
        entity.target = null;
        playerAnimator.SetBool("isWalking", false);
        PauseMenu.ActivateYouDied();
        
        //manager.GainExp(rewardExperience);
        
        playerAnimator.SetBool("isDead", true);
        StopAllCoroutines();
    
    }




}
