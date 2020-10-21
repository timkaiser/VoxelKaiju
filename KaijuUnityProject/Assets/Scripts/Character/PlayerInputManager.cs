using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine;
using System;
using static GameManager;

namespace Character
{
    public class PlayerInputManager : MonoBehaviour
    {
        /// <summary>
        /// The static instance of the GameManager in the scene.
        /// </summary>
        public static PlayerInputManager Instance { get; private set; } = null;

        private CharacterInputActions inputActions;

        [SerializeField]
        bool PCControls = false;

        [SerializeField]
        private float horizontalAxisLeft = 0.0f;
        [SerializeField]
        private float verticalAxisLeft = 0.0f;
        [SerializeField]
        private bool attackLight = false;
        private bool attackLightTap = false;
        float counterLight = 0.0f;
        [SerializeField]
        private bool attackHeavy = false;
        [SerializeField]
        private bool shoulderLeft = false;
        private bool attackLeftTap = false;
        float counterLeft = 0.0f;
        [SerializeField]
        private bool shoulderRight = false;
        float rightShoulderVertical = 0.0f;
        [SerializeField]
        private bool attackForward = false;
        [SerializeField]
        private bool attackLeft = false;

        private Vector2 leftStick = new Vector2(0.0f,0.0f);

        [SerializeField]
        private float verticalAxisRight = 0.0f;
        [SerializeField]
        private float horizontalAxisRight = 0.0f;

        [SerializeField]
        private Vector2 mouseDelta = new Vector2(0.0f,0.0f);
        [SerializeField]
        private float mouseSensitivity = 0.25f;

        private void Awake()
        {
            //Check if instance is assigned
            if (Instance == null) Instance = this;
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            inputActions = new CharacterInputActions();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (inputActions == null)
                Debug.LogError("InputActions are not mapped to PlayerInputManager");
        }

        private void OnEnable()
        {
            inputActions.Kaiju.ControllerLeftStick.performed += HandleLeftStick;
            inputActions.Kaiju.ControllerRightStick.performed += HandleRightStick;
            inputActions.Kaiju.AttackLight.performed += HandleAttack1;

            inputActions.Kaiju.LeftShoulder.started += SetShoulderLeftTrue;
            inputActions.Kaiju.LeftShoulder.canceled += SetShoulderLeftFalse;
            inputActions.Kaiju.RightShoulder.started += SetShoulderRightTrue;
            inputActions.Kaiju.RightShoulder.canceled += SetShoulderRightFalse;

            inputActions.Kaiju.AttackLight.started += SetAttackLightTrue;
            inputActions.Kaiju.AttackLight.canceled += SetAttackLightFalse;
            inputActions.Kaiju.AttackForward.started += SetAttackForwardTrue;
            inputActions.Kaiju.AttackForward.canceled += SetAttackForwardFalse;
            inputActions.Kaiju.AttackHeavy.started += SetAttackHeavyTrue;
            inputActions.Kaiju.AttackHeavy.canceled += SetAttackHeavyFalse;
            inputActions.Kaiju.AttackLeft.started += SetAttackLeftTrue;
            inputActions.Kaiju.AttackLeft.canceled += SetAttackLeftFalse;

            inputActions.Kaiju.Start.started += PauseAndResume;

            inputActions.Kaiju.Enable();
        }

        private void SetAttackLeftFalse(InputAction.CallbackContext obj)
        {
            attackLeft = false;
        }

        private void SetAttackLeftTrue(InputAction.CallbackContext obj)
        {
            counterLeft = 0.1f;
            attackLeft = true;
        }

        private void SetAttackHeavyFalse(InputAction.CallbackContext obj)
        {
            attackHeavy = false;
        }

        private void SetAttackHeavyTrue(InputAction.CallbackContext obj)
        {
            attackHeavy = true;
        }

        private void SetAttackForwardFalse(InputAction.CallbackContext obj)
        {
            attackForward = false; ;
        }

        private void SetAttackForwardTrue(InputAction.CallbackContext obj)
        {
            attackForward = true;
        }

        private void SetAttackLightFalse(InputAction.CallbackContext obj)
        {
            attackLight = false;
        }

        private void SetAttackLightTrue(InputAction.CallbackContext obj)
        {
            counterLight = 0.1f;
            attackLight = true;
        }

        private void HandleAttack2(InputAction.CallbackContext obj)
        {
            Debug.Log("Attack2");
        }

        private void OnDisable()
        {
            inputActions.Kaiju.AttackLight.performed -= HandleAttack1;
            inputActions.Kaiju.ControllerLeftStick.performed -= HandleLeftStick;
            inputActions.Kaiju.ControllerRightStick.performed -= HandleRightStick;

            inputActions.Kaiju.LeftShoulder.started -= SetShoulderLeftTrue;
            inputActions.Kaiju.LeftShoulder.canceled -= SetShoulderLeftFalse;
            inputActions.Kaiju.RightShoulder.started -= SetShoulderRightTrue;
            inputActions.Kaiju.RightShoulder.canceled -= SetShoulderRightFalse;

            inputActions.Kaiju.AttackLight.started -= SetAttackLightTrue;
            inputActions.Kaiju.AttackLight.canceled -= SetAttackLightFalse;
            inputActions.Kaiju.AttackForward.started -= SetAttackForwardTrue;
            inputActions.Kaiju.AttackForward.canceled -= SetAttackForwardFalse;
            inputActions.Kaiju.AttackHeavy.started -= SetAttackHeavyTrue;
            inputActions.Kaiju.AttackHeavy.canceled -= SetAttackHeavyFalse;
            inputActions.Kaiju.AttackLeft.started -= SetAttackLeftTrue;
            inputActions.Kaiju.AttackLeft.canceled -= SetAttackLeftFalse;

            inputActions.Kaiju.Start.started -= PauseAndResume;

            inputActions.Kaiju.Disable();
        }

        private void HandleRightStick(InputAction.CallbackContext obj)
        {
            Vector2 value = obj.ReadValue<Vector2>();
            //Debug.Log("Camera moved +" + value);
        }

        private void HandleLeftStick(InputAction.CallbackContext obj)
        {
            leftStick = obj.ReadValue<Vector2>();
            //Debug.Log("Run in +" + leftStick);
        }

        private void HandleAttack1(InputAction.CallbackContext obj)
        {
            //attack1 = obj.;
            //Debug.Log("Attack");
        }
        private void SetShoulderLeftTrue(InputAction.CallbackContext obj)
        {
            shoulderLeft = true;
        }

        private void SetShoulderLeftFalse(InputAction.CallbackContext obj)
        {
            shoulderLeft = false;
        }

        private void SetShoulderRightTrue(InputAction.CallbackContext obj)
        {
            leftStick = inputActions.Kaiju.ControllerLeftStick.ReadValue<Vector2>();
            rightShoulderVertical = leftStick.y;
            shoulderRight = true;
        }

        private void SetShoulderRightFalse(InputAction.CallbackContext obj)
        {
            rightShoulderVertical = 0.0f;
            shoulderRight = false;
        }

        private void PauseAndResume(InputAction.CallbackContext obj)
        {
            GameManager manager = GameManager.Instance;
            GameStates gameState = manager.GameState;
            if (gameState.Equals(GameStates.Paused))
            {
                GameManager.Instance.ResumeGame();
            }
            else if (gameState.Equals(GameStates.Playing))
            {
                GameManager.Instance.PauseGame();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //ToDo implement easy PC Debug version
            //Left Stick
            leftStick = inputActions.Kaiju.ControllerLeftStick.ReadValue<Vector2>();
            horizontalAxisLeft = leftStick.x;
            verticalAxisLeft = leftStick.y;
            //Right Stick
            Vector2 value = inputActions.Kaiju.ControllerRightStick.ReadValue<Vector2>();
            horizontalAxisRight = value.x;
            verticalAxisRight = value.y;

            //PC Specific controls
            if (PCControls)
            {
                
                if (inputActions.Kaiju.W.ReadValue<float>() > 0.0f) verticalAxisLeft = 1.0f;
                if (inputActions.Kaiju.S.ReadValue<float>() > 0.0f) verticalAxisLeft += -1.0f;
                if (inputActions.Kaiju.A.ReadValue<float>() > 0.0f) horizontalAxisLeft = -1.0f;
                if (inputActions.Kaiju.D.ReadValue<float>() > 0.0f) horizontalAxisLeft += 1.0f;

                mouseDelta = inputActions.Kaiju.Mouse.ReadValue<Vector2>() * mouseSensitivity;
                horizontalAxisRight = mouseDelta.x;
                verticalAxisRight = mouseDelta.y *-1f;
            }
            counterLight -= Time.deltaTime;
            counterLeft -= Time.deltaTime;
        }

        public bool PlayOnPC() { return PCControls; }

        public bool getUIUp()
        {
            return Mathf.Round(verticalAxisLeft) > 0 ;
        }

        public bool getUIDown()
        {
            return Mathf.Round(verticalAxisLeft) < 0;
        }

        public bool getStartDown()
        {
            return inputActions.Kaiju.Start.triggered;
        }

        public bool getAttackLightDown()
        {
            return inputActions.Kaiju.AttackLight.triggered;
        }

        public bool getAttackHeavyDown()
        {
            return inputActions.Kaiju.AttackHeavy.triggered;
        }

        public bool getAttackForwardDown()
        {
            return inputActions.Kaiju.AttackForward.triggered;
        }

        public bool getAttackLeftDown()
        {
            return inputActions.Kaiju.AttackLeft.triggered;
        }

        public bool getUIA()
        {
            return GetAttackButtonLightTab();   
        }

        public float GetHorizontalLeft () { return horizontalAxisLeft; }
        public float GetVerticalLeft() { return verticalAxisLeft; }
        public float GetHorizontalRight() { return horizontalAxisRight; }
        public float GetVerticalRight() { return verticalAxisRight; }
        public bool GetAttackButtonLight () { return attackLight; }
        public bool GetAttackButtonHeavy() { return attackHeavy; }
        public bool GetAttackButtonForward() { return attackForward; }
        public bool GetAttackButtonLeft() { return attackLeft; }
        public bool GetButtonShoulderRight() { return shoulderRight; }
        public float GetVerticalLeftSinceRightShoulder() { return rightShoulderVertical; }
        public bool GetButtonShoulderLeft() { return shoulderLeft; }

        public bool GetAttackButtonLeftTab() { return attackLeft && counterLeft >= 0.0f; }
        public bool GetAttackButtonLightTab() { return attackLight && counterLight >= 0.0f; }

        public Vector3 GetLeftStick() { return new Vector3( leftStick.x, 0.0f, leftStick.y); }

        public Vector2 GetMouseDelta() { return mouseDelta; }
    }
}

