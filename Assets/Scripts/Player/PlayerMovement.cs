using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;
    private CharacterController characterController;
    private Animator playerAnimator;

    public float moveSpeed;
    public float gravity = 9.81f;

    public bool enableMovement = true;
    public bool playerMoving = false;

    public float burnMovementVariation; 
    public float burnSpeedIncrease;

    void Start()
    {
        playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (enableMovement)
        {
            HandleMovement();
            HandleAnimations();
            ApplyGravity();
        }
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 movement = forward * verticalInput + right * horizontalInput;

        if (GetComponent<PlayerBurn>().isBurning)
        {
            movement.x += Random.Range(-burnMovementVariation, burnMovementVariation);
            movement.z += Random.Range(-burnMovementVariation, burnMovementVariation);
            movement *= (1f + burnSpeedIncrease);
        }

        if (movement.magnitude > 0f)
        {
            characterController.Move(movement * moveSpeed * Time.deltaTime);
            playerMoving = true;
        }
        else
        {
            playerMoving = false;
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Vector3.Distance(mainCamera.transform.position, transform.position);
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3 directionToMouse = mouseWorldPosition - transform.position;
        directionToMouse.y = 0f;

        if (directionToMouse.magnitude > 0f)
        {
            Quaternion rotationToMouse = Quaternion.LookRotation(directionToMouse);

            transform.rotation = Quaternion.Euler(-90, rotationToMouse.eulerAngles.y, rotationToMouse.eulerAngles.z + -90);
        }
    }

    void HandleAnimations()
    {
        var weaponStatus = GetComponent<WeaponEquipedStatus>();

        SetAnimatorBools("isRunning", playerMoving);
        SetAnimatorBools("pistolRunning", weaponStatus.pistolEquiped && playerMoving);
        SetAnimatorBools("shotgunRunning", weaponStatus.shotgunEquiped && playerMoving);
        SetAnimatorBools("rocketlauncherRunning", weaponStatus.rocketLauncherEquiped && playerMoving);
        SetAnimatorBools("deathwadRunning", weaponStatus.deathwadEquiped && playerMoving);
        SetAnimatorBools("flamethrowerRunning", weaponStatus.flamethrowerEquiped && playerMoving);

        SetAnimatorBools("pistolEquiped", weaponStatus.pistolEquiped);
        SetAnimatorBools("shotgunEquiped", weaponStatus.shotgunEquiped);
        SetAnimatorBools("rocketLauncherEquiped", weaponStatus.rocketLauncherEquiped);
        SetAnimatorBools("deathwadEquiped", weaponStatus.deathwadEquiped);
        SetAnimatorBools("flamethrowerEquiped", weaponStatus.flamethrowerEquiped);
        SetAnimatorBools("isBurning", GetComponent<PlayerBurn>().isBurning);
    }

    void SetAnimatorBools(string parameter, bool value)
    {
        playerAnimator.SetBool(parameter, value);
    }

    void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            Vector3 gravityVector = Vector3.down * gravity;
            characterController.Move(gravityVector * Time.deltaTime);
        }
    }
}