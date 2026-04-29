using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerPhysicsController _playerController;
    [SerializeField] private PlayerFormManager _formManager;

    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _nextFormAction;
    private InputAction _previousFormAction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        if (_playerController == null)
            _playerController = GetComponent<PlayerPhysicsController>();

        if (_formManager == null)
            _formManager = GetComponent<PlayerFormManager>();

        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _nextFormAction = _playerInput.actions["NextForm"];
        _previousFormAction = _playerInput.actions["PreviousForm"];
    }

    private void OnEnable()
    {
        _jumpAction.performed += OnJumpPerformed;
        _nextFormAction.performed += OnNextFormPerformed;
        _previousFormAction.performed += OnPreviousFormPerformed;
    }

    private void OnDisable()
    {
        _jumpAction.performed -= OnJumpPerformed;
        _nextFormAction.performed -= OnNextFormPerformed;
        _previousFormAction.performed -= OnPreviousFormPerformed;
    }

    private void FixedUpdate()
    {
        float moveInput = _moveAction.ReadValue<float>();
        _playerController.SetMoveInput(moveInput);

    }

    private void OnJumpPerformed(InputAction.CallbackContext Context)
    {
        _playerController.RequestJump();
    }

    private void OnNextFormPerformed(InputAction.CallbackContext Context)
    {
        _formManager.SwitchToNextForm();
    }

    private void OnPreviousFormPerformed(InputAction.CallbackContext Context)
    {
        _formManager.SwitchToPreviousForm();
    }
}