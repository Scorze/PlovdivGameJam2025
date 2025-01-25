using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.InputSystem.InputAction;
using static UnityEngine.Rendering.DebugUI;
using Random = System.Random;

public class Player : MonoBehaviour

{
    [SerializeField]
    private float MoveSpeed = 4.5f;

    [SerializeField]
    private float lightPunchPushback = 1f;

    [SerializeField]
    private float heavyPunchPushback = 2f;

    public AudioClip lightPunchClip;
    public AudioClip heavyPunchClip;
    public List<AudioClip> hitClips;

    private CharacterController controller;
    private AudioSource audioSource;
    private Vector2 inputVector = Vector2.zero;
    private PlayerControls controls;
    private PlayerConfiguration playerConfig;
    private Animator animator;
    private bool isLightPunch = false;
    private bool isHeavyPunch = false;
    private bool hasHit = false;
    private bool isAttacking = false;
    private bool isBlocking = false;
    private float currentFunBar = 0f;
    private SimpleHPBarScript hpBar;
    private bool frozen = false;
    private float targetPosition = 0f;
    private bool isPushedBack = false;
    private static Random rnd = new Random();
    private bool isPlayerTwo = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void InitializePlayer(PlayerConfiguration config, GameObject playerFunBar, bool isPlayerTwo)
    {
        playerConfig = config;
        this.hpBar = playerFunBar.GetComponent<SimpleHPBarScript>();
        config.Input.onActionTriggered += Input_onActionTriggered;
        this.isPlayerTwo = isPlayerTwo;
        if (isPlayerTwo)
        {
            animator.SetBool("isPlayerTwo", true);
        }
    }

    private void Input_onActionTriggered(CallbackContext obj)
    {
        OnMove(obj);
    }

    // Update is called once per frame
    void Update()
    {
        if (frozen)
        {
            return;
        }

        if (isPushedBack)
        {
            if (transform.rotation.y > 0)
            {
                if (targetPosition > transform.position.x)
                {
                    Vector3 pushbackVector = new Vector3(10f, 0f, 0f);
                    pushbackVector += Physics.gravity;
                    controller.Move(pushbackVector * Time.deltaTime);
                    return;
                }
                else 
                {
                    isPushedBack = false;
                }
            }
            else
            {
                if (targetPosition < transform.position.x)
                {
                    Vector3 pushbackVector = new Vector3(-10f, 0f, 0f);
                    pushbackVector += Physics.gravity;
                    controller.Move(pushbackVector * Time.deltaTime);
                    return;
                }
                else
                {
                    isPushedBack = false;
                }
            }
        }
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, 0f).normalized;
        if (moveDirection.magnitude >= 0.1f && !isAttacking && !isBlocking)
        {
            moveDirection *= MoveSpeed;
            moveDirection += Physics.gravity;
            animator.SetBool("isWalking", true);
            controller.Move(moveDirection * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isWalking", false);
            controller.Move(Physics.gravity * Time.deltaTime);
        }
    }

    public void OnMove(CallbackContext context)
    {
        if (frozen)
        {
            return;
        }
        if (context.action.name == controls.Player.Movement.name)
        {
            inputVector = context.ReadValue<Vector2>();
        }
        else if (context.action.name == controls.Player.LightPunch.name && context.started && !isAttacking && !isBlocking)
        {
            LightPunch();
        }
        else if (context.action.name == controls.Player.HeavyPunch.name && context.started && !isAttacking && !isBlocking)
        {
            HeavyPunch();
        }
        else if (context.action.name == controls.Player.Block.name && context.started && !isAttacking)
        {
            Block();
        } 
        else if (context.action.name == controls.Player.Block.name && context.canceled)
        {
            StopBlock();
        }
    }

    private void LightPunch()
    {
        animator.SetTrigger("punch");
        setIsLightPunch(true);
        setIsAttacking(true);
        audioSource.clip = lightPunchClip;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }

    private void HeavyPunch()
    {
        animator.SetTrigger("punch2");
        setIsHeavyPunch(true);
        setIsAttacking(true);
        audioSource.clip = heavyPunchClip;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }

    private void Block()
    { 
        this.isBlocking = true;
        animator.SetBool("isBlocking", true);
    }

    private void StopBlock()
    {
        this.isBlocking = false;
        animator.SetBool("isBlocking", false);
    }

    public void setIsLightPunch(bool isLightPunch)
    {
        this.isLightPunch = isLightPunch;
    }

    public void setIsHeavyPunch(bool isHeavyPunch)
    {
        this.isHeavyPunch = isHeavyPunch;
    }

    public bool getIsLightPunch()
    {
        return this.isLightPunch;
    }

    public bool getIsHeavyPunch()
    {
        return this.isHeavyPunch;
    }

    public bool getHasHit()
    {
        return this.hasHit;
    }

    public void setHasHit(bool hasHit)
    {
        this.hasHit = hasHit;
    }

    public void increaseFunBar(float increase)
    {
        currentFunBar += increase;
        hpBar.IncrementBar(increase);
    }

    public void pushBackHeavyPunch() 
    {
        Quaternion rotation;
        if (isPlayerTwo)
        {
            rotation = Quaternion.identity;
        }
        else
        {
            rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        Instantiate(GameManager.Instance.heavyPunchParticle, transform.position, rotation).Play();
        float positionChange = transform.rotation.y > 0 ? heavyPunchPushback : -heavyPunchPushback;
        if (isBlocking)
        {
            positionChange /= 2f;
        }
        targetPosition = transform.position.x + positionChange;
        print(targetPosition);
        print(transform.position.x);
        isPushedBack = true;
        if (!isBlocking)
        {
            animator.SetTrigger("hit");
            int r = rnd.Next(hitClips.Count);
            audioSource.clip = hitClips[r];
            audioSource.volume = 1.0f;
            audioSource.Play();
        }
    }

    public void pushBackLightPunch()
    {
        Instantiate(GameManager.Instance.lightPunchParticle, transform.position, transform.rotation).Play();
        float positionChange = transform.rotation.y > 0 ? lightPunchPushback : -lightPunchPushback;
        if (isBlocking)
        {
            positionChange /= 2f;
        }
        targetPosition = transform.position.x + positionChange;

        print(targetPosition);
        print(transform.position.x);
        isPushedBack = true;
        if (!isBlocking) 
        {
            animator.SetTrigger("hit");
            int r = rnd.Next(hitClips.Count);
            audioSource.clip = hitClips[r];
            audioSource.volume = 1.0f;
            audioSource.Play();
        }
    }

    public float getCurrentFunBar()
    {
        return this.currentFunBar;
    }

    public bool getIsAttacking()
    {
        return this.isAttacking;
    }

    public void setIsAttacking(bool isAttacking)
    {
        this.isAttacking = isAttacking;
    }

    public bool getIsBlocking()
    {
        return this.isBlocking;
    }

    public void setFrozen(bool frozen) 
    {
        this.frozen = frozen;
    }

    public void setIsPushedBack(bool isPushedBack)
    {
        this.isPushedBack = isPushedBack;
    }

    void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.gameObject.tag == "BoundaryWall")
        {
            isPushedBack = false;
        }
    }

    public void triggerAnimation(String name)
    {
        animator.SetTrigger(name);
    }
}