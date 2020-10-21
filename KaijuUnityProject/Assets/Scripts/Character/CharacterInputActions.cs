// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Character/CharacterInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @CharacterInputActions : IInputActionCollection, IDisposable
{
    private InputActionAsset asset;
    public @CharacterInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CharacterInputActions"",
    ""maps"": [
        {
            ""name"": ""Kaiju"",
            ""id"": ""a3bfc490-c535-49e3-b1bd-376369f8ddd7"",
            ""actions"": [
                {
                    ""name"": ""AttackLight"",
                    ""type"": ""Button"",
                    ""id"": ""3cf4c7b3-4e43-4e7d-9463-3d0e2597bc06"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ControllerLeftStick"",
                    ""type"": ""Value"",
                    ""id"": ""a652cfaf-b809-4bc8-805c-19aada6a8b3e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ControllerRightStick"",
                    ""type"": ""Button"",
                    ""id"": ""b02d537c-6980-4973-9d69-e6599c69b525"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AttackHeavy"",
                    ""type"": ""Button"",
                    ""id"": ""4e6ffb23-d69e-4416-ae60-9c2abf98ea33"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""W"",
                    ""type"": ""Button"",
                    ""id"": ""6df37db9-2430-46d1-81da-a1f7089adcf4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""A"",
                    ""type"": ""Button"",
                    ""id"": ""d6812264-f3b1-4427-8aa4-98fb8776e1b4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""S"",
                    ""type"": ""Button"",
                    ""id"": ""9b35b817-9195-4089-8d39-bd1505b1573b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""D"",
                    ""type"": ""Button"",
                    ""id"": ""19a0ef32-ed7b-4737-9a55-0066cfb74a8e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Mouse"",
                    ""type"": ""Button"",
                    ""id"": ""8a6f0a7e-387f-4911-ab6f-84c97194c3d2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftShoulder"",
                    ""type"": ""Button"",
                    ""id"": ""cc41819d-36b1-4f1a-8864-f8e00be526ae"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightShoulder"",
                    ""type"": ""Button"",
                    ""id"": ""40aaec60-7151-44d2-a103-c2ae257a35a2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AttackForward"",
                    ""type"": ""Button"",
                    ""id"": ""358522fa-c25f-4160-8055-ef0f932a1a2d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AttackLeft"",
                    ""type"": ""Button"",
                    ""id"": ""7e46e4a8-1f44-43f7-b47f-e0ff82ceec67"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Start"",
                    ""type"": ""Button"",
                    ""id"": ""fb7fea3e-7856-46e8-b643-3beadc11ab09"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b55c33ab-4cd9-49bb-a4d5-0e4cc18adb8f"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AttackLight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f8446215-618e-4261-a70c-aa52e7965361"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AttackLight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d1995475-c229-4b32-91fc-1a6118eaf14e"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ControllerLeftStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""baa6872a-f943-4845-a2a5-4cb2736605af"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ControllerRightStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e5416b12-e347-41e2-913a-589254b9d385"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AttackHeavy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6f6c495f-ebd8-4900-93cc-1424e199da6e"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AttackHeavy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ab4fbf4a-9dd2-4428-8bc9-eebcad9f077a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""W"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9820c6af-b6ad-4de3-addb-d1656c4a6760"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""A"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""04d8548a-f8fd-4a3f-8cc2-8d5d5654b6f6"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""S"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8002dd3c-e617-4d74-9bc6-7ce8ad9130c0"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""D"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1623b615-cab7-4dd5-a169-3aaa00729779"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b61e8147-7aef-469e-95d5-7393aacb0f1e"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftShoulder"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""344fd99c-1a7c-436a-b9e3-f9bf959df61a"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightShoulder"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""626a22b6-88ea-4714-a2d0-cd08a4a0ae04"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightShoulder"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7cd4e1e9-4fbc-4716-bb71-e56994ff749a"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AttackForward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""20bd46a7-b931-430f-96ae-eb49b91ca162"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AttackForward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f1c117fb-6cd1-4580-b8a7-06eaeca810a7"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AttackLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""da3f2ddc-a7ef-40b9-a428-4cdaa6489cbe"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AttackLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aa24eb0e-9bb8-4276-97c0-7abfd782988e"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cba4c957-77b0-4125-9e6d-8119e7b67c12"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Kaiju
        m_Kaiju = asset.FindActionMap("Kaiju", throwIfNotFound: true);
        m_Kaiju_AttackLight = m_Kaiju.FindAction("AttackLight", throwIfNotFound: true);
        m_Kaiju_ControllerLeftStick = m_Kaiju.FindAction("ControllerLeftStick", throwIfNotFound: true);
        m_Kaiju_ControllerRightStick = m_Kaiju.FindAction("ControllerRightStick", throwIfNotFound: true);
        m_Kaiju_AttackHeavy = m_Kaiju.FindAction("AttackHeavy", throwIfNotFound: true);
        m_Kaiju_W = m_Kaiju.FindAction("W", throwIfNotFound: true);
        m_Kaiju_A = m_Kaiju.FindAction("A", throwIfNotFound: true);
        m_Kaiju_S = m_Kaiju.FindAction("S", throwIfNotFound: true);
        m_Kaiju_D = m_Kaiju.FindAction("D", throwIfNotFound: true);
        m_Kaiju_Mouse = m_Kaiju.FindAction("Mouse", throwIfNotFound: true);
        m_Kaiju_LeftShoulder = m_Kaiju.FindAction("LeftShoulder", throwIfNotFound: true);
        m_Kaiju_RightShoulder = m_Kaiju.FindAction("RightShoulder", throwIfNotFound: true);
        m_Kaiju_AttackForward = m_Kaiju.FindAction("AttackForward", throwIfNotFound: true);
        m_Kaiju_AttackLeft = m_Kaiju.FindAction("AttackLeft", throwIfNotFound: true);
        m_Kaiju_Start = m_Kaiju.FindAction("Start", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Kaiju
    private readonly InputActionMap m_Kaiju;
    private IKaijuActions m_KaijuActionsCallbackInterface;
    private readonly InputAction m_Kaiju_AttackLight;
    private readonly InputAction m_Kaiju_ControllerLeftStick;
    private readonly InputAction m_Kaiju_ControllerRightStick;
    private readonly InputAction m_Kaiju_AttackHeavy;
    private readonly InputAction m_Kaiju_W;
    private readonly InputAction m_Kaiju_A;
    private readonly InputAction m_Kaiju_S;
    private readonly InputAction m_Kaiju_D;
    private readonly InputAction m_Kaiju_Mouse;
    private readonly InputAction m_Kaiju_LeftShoulder;
    private readonly InputAction m_Kaiju_RightShoulder;
    private readonly InputAction m_Kaiju_AttackForward;
    private readonly InputAction m_Kaiju_AttackLeft;
    private readonly InputAction m_Kaiju_Start;
    public struct KaijuActions
    {
        private @CharacterInputActions m_Wrapper;
        public KaijuActions(@CharacterInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @AttackLight => m_Wrapper.m_Kaiju_AttackLight;
        public InputAction @ControllerLeftStick => m_Wrapper.m_Kaiju_ControllerLeftStick;
        public InputAction @ControllerRightStick => m_Wrapper.m_Kaiju_ControllerRightStick;
        public InputAction @AttackHeavy => m_Wrapper.m_Kaiju_AttackHeavy;
        public InputAction @W => m_Wrapper.m_Kaiju_W;
        public InputAction @A => m_Wrapper.m_Kaiju_A;
        public InputAction @S => m_Wrapper.m_Kaiju_S;
        public InputAction @D => m_Wrapper.m_Kaiju_D;
        public InputAction @Mouse => m_Wrapper.m_Kaiju_Mouse;
        public InputAction @LeftShoulder => m_Wrapper.m_Kaiju_LeftShoulder;
        public InputAction @RightShoulder => m_Wrapper.m_Kaiju_RightShoulder;
        public InputAction @AttackForward => m_Wrapper.m_Kaiju_AttackForward;
        public InputAction @AttackLeft => m_Wrapper.m_Kaiju_AttackLeft;
        public InputAction @Start => m_Wrapper.m_Kaiju_Start;
        public InputActionMap Get() { return m_Wrapper.m_Kaiju; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KaijuActions set) { return set.Get(); }
        public void SetCallbacks(IKaijuActions instance)
        {
            if (m_Wrapper.m_KaijuActionsCallbackInterface != null)
            {
                @AttackLight.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnAttackLight;
                @AttackLight.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnAttackLight;
                @AttackLight.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnAttackLight;
                @ControllerLeftStick.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnControllerLeftStick;
                @ControllerLeftStick.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnControllerLeftStick;
                @ControllerLeftStick.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnControllerLeftStick;
                @ControllerRightStick.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnControllerRightStick;
                @ControllerRightStick.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnControllerRightStick;
                @ControllerRightStick.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnControllerRightStick;
                @AttackHeavy.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnAttackHeavy;
                @AttackHeavy.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnAttackHeavy;
                @AttackHeavy.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnAttackHeavy;
                @W.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnW;
                @W.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnW;
                @W.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnW;
                @A.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnA;
                @A.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnA;
                @A.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnA;
                @S.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnS;
                @S.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnS;
                @S.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnS;
                @D.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnD;
                @D.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnD;
                @D.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnD;
                @Mouse.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnMouse;
                @Mouse.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnMouse;
                @Mouse.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnMouse;
                @LeftShoulder.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnLeftShoulder;
                @LeftShoulder.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnLeftShoulder;
                @LeftShoulder.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnLeftShoulder;
                @RightShoulder.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnRightShoulder;
                @RightShoulder.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnRightShoulder;
                @RightShoulder.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnRightShoulder;
                @AttackForward.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnAttackForward;
                @AttackForward.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnAttackForward;
                @AttackForward.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnAttackForward;
                @AttackLeft.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnAttackLeft;
                @AttackLeft.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnAttackLeft;
                @AttackLeft.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnAttackLeft;
                @Start.started -= m_Wrapper.m_KaijuActionsCallbackInterface.OnStart;
                @Start.performed -= m_Wrapper.m_KaijuActionsCallbackInterface.OnStart;
                @Start.canceled -= m_Wrapper.m_KaijuActionsCallbackInterface.OnStart;
            }
            m_Wrapper.m_KaijuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @AttackLight.started += instance.OnAttackLight;
                @AttackLight.performed += instance.OnAttackLight;
                @AttackLight.canceled += instance.OnAttackLight;
                @ControllerLeftStick.started += instance.OnControllerLeftStick;
                @ControllerLeftStick.performed += instance.OnControllerLeftStick;
                @ControllerLeftStick.canceled += instance.OnControllerLeftStick;
                @ControllerRightStick.started += instance.OnControllerRightStick;
                @ControllerRightStick.performed += instance.OnControllerRightStick;
                @ControllerRightStick.canceled += instance.OnControllerRightStick;
                @AttackHeavy.started += instance.OnAttackHeavy;
                @AttackHeavy.performed += instance.OnAttackHeavy;
                @AttackHeavy.canceled += instance.OnAttackHeavy;
                @W.started += instance.OnW;
                @W.performed += instance.OnW;
                @W.canceled += instance.OnW;
                @A.started += instance.OnA;
                @A.performed += instance.OnA;
                @A.canceled += instance.OnA;
                @S.started += instance.OnS;
                @S.performed += instance.OnS;
                @S.canceled += instance.OnS;
                @D.started += instance.OnD;
                @D.performed += instance.OnD;
                @D.canceled += instance.OnD;
                @Mouse.started += instance.OnMouse;
                @Mouse.performed += instance.OnMouse;
                @Mouse.canceled += instance.OnMouse;
                @LeftShoulder.started += instance.OnLeftShoulder;
                @LeftShoulder.performed += instance.OnLeftShoulder;
                @LeftShoulder.canceled += instance.OnLeftShoulder;
                @RightShoulder.started += instance.OnRightShoulder;
                @RightShoulder.performed += instance.OnRightShoulder;
                @RightShoulder.canceled += instance.OnRightShoulder;
                @AttackForward.started += instance.OnAttackForward;
                @AttackForward.performed += instance.OnAttackForward;
                @AttackForward.canceled += instance.OnAttackForward;
                @AttackLeft.started += instance.OnAttackLeft;
                @AttackLeft.performed += instance.OnAttackLeft;
                @AttackLeft.canceled += instance.OnAttackLeft;
                @Start.started += instance.OnStart;
                @Start.performed += instance.OnStart;
                @Start.canceled += instance.OnStart;
            }
        }
    }
    public KaijuActions @Kaiju => new KaijuActions(this);
    public interface IKaijuActions
    {
        void OnAttackLight(InputAction.CallbackContext context);
        void OnControllerLeftStick(InputAction.CallbackContext context);
        void OnControllerRightStick(InputAction.CallbackContext context);
        void OnAttackHeavy(InputAction.CallbackContext context);
        void OnW(InputAction.CallbackContext context);
        void OnA(InputAction.CallbackContext context);
        void OnS(InputAction.CallbackContext context);
        void OnD(InputAction.CallbackContext context);
        void OnMouse(InputAction.CallbackContext context);
        void OnLeftShoulder(InputAction.CallbackContext context);
        void OnRightShoulder(InputAction.CallbackContext context);
        void OnAttackForward(InputAction.CallbackContext context);
        void OnAttackLeft(InputAction.CallbackContext context);
        void OnStart(InputAction.CallbackContext context);
    }
}
