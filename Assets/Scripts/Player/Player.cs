using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonobehaviour<Player>
{
    private AnimationOverrides animationOverrides;


    // Movement Parameters
    private float xInput;
    private float yInput;
    public bool isWalking;
    public bool isRunning;
    private bool isIdle;
    private bool isCarrying;
    private ToolEffect toolEffect = ToolEffect.none;
    private bool isUsingToolRight;
    private bool isUsingToolLeft;
    private bool isUsingToolUp;
    private bool isUsingToolDown;
    private bool isLiftingToolRight;
    private bool isLiftingToolLeft;
    private bool isLiftingToolUp;
    private bool isLiftingToolDown;
    private bool isPickingRight;
    private bool isPickingLeft;
    private bool isPickingUp;
    private bool isPickingDown;
    private bool isSwingingToolRight;
    private bool isSwingingToolLeft;
    private bool isSwingingToolUp;
    private bool isSwingingToolDown;
    private bool idleUp;
    private bool idleDown;
    private bool idleLeft;
    private bool idleRight;

    private Camera mainCamera;

    private Rigidbody2D rigidBody2D;

#pragma warning disable 414
    private Direction playerDirection;
#pragma warning restore 414



    private List<CharacterAttribute> characterAttributeCustomisationList;
    private float movementSpeed;


    [Tooltip("Should be populated in the prefab with the equipped item sprite renderer")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    // Player attributes that can be swapped
    private CharacterAttribute armsCharacterAttribute;

    private CharacterAttribute toolCharacterAttribute;


    private bool _playerInputIsDisabled = false;

    public bool PlayerInputIsDisabled { get => _playerInputIsDisabled; set => _playerInputIsDisabled = value; }
    protected override void Awake()
    {
        base.Awake();
        rigidBody2D = GetComponent<Rigidbody2D>();

        animationOverrides = GetComponentInChildren<AnimationOverrides>();

        // Initialise swappable character attributes
        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColour.none, PartVariantType.none);
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.tool, PartVariantColour.none, PartVariantType.hoe);

        // Initialise character attribute list
        characterAttributeCustomisationList = new List<CharacterAttribute>();


        //获取主相机
        mainCamera = Camera.main;
    }
    private void Update()
    {
        #region Player Input
        if (!PlayerInputIsDisabled)
        {
            ResetAnimationTriggers();
            PlayerMovementInput();
            PlayerWalkInput();
            PlayerTestInput();

            // Send event to any listeners for player movement input
            EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect,
                    isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                    isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                    isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                    isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                    false, false, false, false);
        }


        #endregion
    }
    private void FixedUpdate()
    {
        PlayerMovement();
    }
    private void PlayerMovement()
    {
        Vector2 move = new Vector2(xInput * movementSpeed * Time.deltaTime, yInput * movementSpeed * Time.deltaTime);

        rigidBody2D.MovePosition(rigidBody2D.position + move);
    }

    private void ResetAnimationTriggers()
    {
        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;
        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;
        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;
        isSwingingToolRight = false;
        isSwingingToolLeft = false;
        isSwingingToolUp = false;
        isSwingingToolDown = false;
        toolEffect = ToolEffect.none;
    }
    private void PlayerMovementInput()
    {
        yInput = Input.GetAxisRaw("Vertical");
        xInput = Input.GetAxisRaw("Horizontal");

        if (yInput != 0 && xInput != 0)
        {
            xInput = xInput * 0.71f;
            yInput = yInput * 0.71f;
        }

        if (xInput != 0 || yInput != 0)
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;

            // Capture player direction for save game
            if (xInput < 0)
            {
                playerDirection = Direction.left;
            }
            else if (xInput > 0)
            {
                playerDirection = Direction.right;
            }
            else if (yInput < 0)
            {
                playerDirection = Direction.down;
            }
            else
            {
                playerDirection = Direction.up;
            }
        }
        else if (xInput == 0 && yInput == 0)
        {
            isRunning = false;
            isWalking = false;
            isIdle = true;
        }
    }
    private void PlayerWalkInput()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            movementSpeed = Settings.walkingSpeed;
        }
        else
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;
        }
    }

    // TODO: Remove
    /// <summary>
    /// Temp routine for test input
    /// </summary>
    private void PlayerTestInput()
    {
        // Trigger Advance Time
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }

        // Trigger Advance Day
        if (Input.GetKeyDown(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
           SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), transform.position);
        }

    }


    private void ResetMovement()
    {
        // Reset movement
        xInput = 0f;
        yInput = 0f;
        isRunning = false;
        isWalking = false;
        isIdle = true;
    }

    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();
        ResetMovement();

        // Send event to any listeners for player movement input
        EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying, toolEffect,
            isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
            isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
            isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
             isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
             false, false, false, false);
    }

    public void DisablePlayerInput()
    {
        PlayerInputIsDisabled = true;
    }

    public void EnablePlayerInput()
    {
        PlayerInputIsDisabled = false;
    }
    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);

        // Apply base character arms customisation
        armsCharacterAttribute.partVariantType = PartVariantType.none;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(armsCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        isCarrying = false;
    }
    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        if (itemDetails != null)
        {
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            // Apply 'carry' character arms customisation
            armsCharacterAttribute.partVariantType = PartVariantType.carry;
            characterAttributeCustomisationList.Clear();
            characterAttributeCustomisationList.Add(armsCharacterAttribute);
            animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

            isCarrying = true;
        }
    }

    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }
        else if (cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }
    public Vector3 GetPlayerViewportPosition()
    {
        return mainCamera.WorldToViewportPoint(transform.position);
    }





}


