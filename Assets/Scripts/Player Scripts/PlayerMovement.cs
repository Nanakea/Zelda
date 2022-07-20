using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    walk,
    attack,
    interact,
    stagger,
    idle
}

public class PlayerMovement : MonoBehaviour
{
    public PlayerState currentState;
    //Publicは外（Unityで見えるのコマンド）Floatは番号の意味です。Speedは名前
    public float speed;
    //Privateは外側で見えないようにするのコマンド。RigidBody２Dは先入れたもを使用することができるようにする。　myRigidBodyを配置する理由は、スクリプト上の他のものがそれを呼び出せるようにするためです。
    private Rigidbody2D myRigidBody;
    private Vector3 change;
    //Animatorのパラメーターを呼んぶのコマンド。
    private Animator animator;
    //Health
    public FloatValue currentHealth;
    public GameSignal playerHealthSignal;
    public VectorValue startingPosition;
    public Inventory playerInventory;
    public SpriteRenderer receivedItemSprite;
    public GameSignal playerHit;

    // Start is called before the first frame update
    void Start()
    {
        //()は付いているの意味はUnityで入れれるのスペースです。
        currentState = PlayerState.walk;
        animator = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>();
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
        transform.position = startingPosition.initialValue;
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is interacting with something
        if(currentState == PlayerState.interact)
        {
            return;
        }
        //Change = Vector3 zeroは、フレームごとにプレーヤーの動きをリセットすることを意味します。
        change = Vector3.zero;
        //x = 左、右の動く。
        change.x = Input.GetAxisRaw("Horizontal");
        //ｙ = 上下の動く。
        change.y = Input.GetAxisRaw("Vertical");
        if(Input.GetButtonDown("attack") && currentState != PlayerState.attack
            && currentState != PlayerState.stagger)
        {
            StartCoroutine(AttackCo());    
        }
        else if (currentState == PlayerState.walk || currentState == PlayerState.idle)
        {
            UpdateAnimationAndMove();
        }
    }

    public void RaiseItem()
    {
        if (playerInventory.currentItem != null)
        {
            if (currentState != PlayerState.interact)
            {
                animator.SetBool("receive item", true);
                currentState = PlayerState.interact;
                receivedItemSprite.sprite = playerInventory.currentItem.itemSprite;
            }
            else
            {
                animator.SetBool("receive item", false);
                currentState = PlayerState.idle;
                receivedItemSprite.sprite = null;
                playerInventory.currentItem = null;
            }
        }
    }

    private IEnumerator AttackCo()
    {
        animator.SetBool("attacking", true);
        currentState = PlayerState.attack;
        yield return null;
        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.3f);
        if(currentState != PlayerState.interact)
        {
            currentState = PlayerState.walk;
        }
    }

    void UpdateAnimationAndMove()
    {
        //もしVectorが０じゃなければ、MoveCharacterを呼ぶ。
        if (change != Vector3.zero)
        {
            MoveCharacter();
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    public void Knock(float knockTime, float damage)
    {
        currentHealth.RuntimeValue -= damage;
        playerHealthSignal.Raise();
        if (currentHealth.RuntimeValue > 0)
        {

            StartCoroutine(KnockCoroutine(knockTime));
        }else{
            this.gameObject.SetActive(false);
        }
    }

    //Characterを動く方法
    void MoveCharacter()
    {
        change.Normalize();
        //myRigidBodyを動くスピードは
        myRigidBody.MovePosition(
            //position + ライン２３番（change)　*　スピード　*　１FPS
            transform.position + change * speed * Time.deltaTime
        );
    }

    private IEnumerator KnockCoroutine(float knockTime)
    {
        playerHit.Raise();
        if (myRigidBody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidBody.velocity = Vector2.zero;
            currentState = PlayerState.idle;
            myRigidBody.velocity = Vector2.zero;
        }
    }
}