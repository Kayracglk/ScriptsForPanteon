using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    private LoadingManager loadingManager;
    private PlayerBase playerBase;
    private PlayerCurrentType playerCurrentType;
    public PlayerHealth health;

    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float turnSpeed = 540f;

    Rigidbody rb;

    private Vector2 movementInput;
    private Vector3 currentMovement;

    private bool isMovementPressed;
    private bool canMove = true;
    private bool isStunned = false;
    private bool isBlocking = false;

    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public bool IsStunned { get { return isStunned; } set { isStunned = value; } }
    public bool IsBlocking { get { return isBlocking; } set { isBlocking = value; } }

    [SerializeField] private Transform raycastPoint;

    private RaycastHit hit;
    private Ray ray;

    private void Awake()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            this.enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        playerBase = GetComponent<PlayerBase>();
        playerCurrentType = GetComponent<PlayerCurrentType>();
        health = GetComponent<PlayerHealth>();

        loadingManager = LoadingManager.instance;

        StartCoroutine(Movement());
    }
    private void FixedUpdate()
    {
        if (Physics.Raycast(raycastPoint.position, raycastPoint.forward, out hit, 2))
        {
            if (hit.transform.CompareTag("Obstacles"))
            {
                isBlocking = true;
            }
        }
        else isBlocking = false;

        Debug.DrawRay(raycastPoint.position, raycastPoint.forward * 5);
    }
    private IEnumerator Movement()
    {
        while (!loadingManager.canStartGame)
        {
            yield return null;
        }

        while (true)
        {
            PlayerMoveWithTranslate();
            yield return null;
        }

    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;

        if (context.phase == InputActionPhase.Canceled)
        {
            movementInput = Vector2.zero; currentMovement = Vector3.zero; isMovementPressed = false; return;
        }

        if (context.phase != InputActionPhase.Performed) return;

        RPCManager.instance.SendRPC(GameManager.instance.view, "SetBoolCharacterAnimation", (int)GetComponent<PlayerCurrentType>().currentType, "Run", 1);

        isMovementPressed = true;

        movementInput = context.ReadValue<Vector2>();

        currentMovement = new Vector3(movementInput.x, 0, movementInput.y);
    }
    public void UpgradeMenu(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;

        if (!health.isPlayerAlive) return;

        if (context.phase == InputActionPhase.Performed)
        {
            PlayerSpawner.instance.UpgradePanel[(int)playerCurrentType.currentType].gameObject.SetActive(true);
            IsStunned = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            PlayerSpawner.instance.UpgradePanel[(int)playerCurrentType.currentType].gameObject.SetActive(false);
            IsStunned = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void RotationProcess()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentMovement, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
    private void PlayerMoveWithTranslate()
    {

        if (!CanMove) return;

        if (IsStunned) return;

        if (isBlocking) return;

        if (currentMovement == Vector3.zero)
            RPCManager.instance.SendRPC(GameManager.instance.view, "SetBoolCharacterAnimation", (int)GetComponent<PlayerCurrentType>().currentType, "Run", 0);
        else
        {
            transform.Translate(Time.deltaTime * walkSpeed * currentMovement.normalized, Space.World);
            RotationProcess();
        }

    }
    private void PlayerMoveWithRigidbody()
    {
        if (isMovementPressed)
        {
            rb.MovePosition(transform.position + currentMovement.normalized * walkSpeed * Time.deltaTime);
            RotationProcess();
        }
    }

}
