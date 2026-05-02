using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private GameObject _doorBody; 
    [SerializeField] private ButtonInteractable _linkedButton;

    [SerializeField] private bool _closeOnPress = false;

    private void Start()
    {
        if (_doorBody == null)
            _doorBody = gameObject;

        if (_linkedButton != null)
        {
            _linkedButton.OnButtonActivated.AddListener(OnButtonDown);
            _linkedButton.OnButtonDeactivated.AddListener(OnButtonUp);

            UpdateDoorState(_linkedButton.IsPressed);
        }
    }

    private void OnButtonDown()
    {
        UpdateDoorState(true);
    }

    private void OnButtonUp()
    {
        UpdateDoorState(false);
    }

    private void UpdateDoorState(bool isButtonPressed)
    {
        bool shouldBeActive = _closeOnPress ? isButtonPressed : !isButtonPressed;
        _doorBody.SetActive(shouldBeActive);
    }

    public void OpenDoor() => UpdateDoorState(!_closeOnPress);
    public void CloseDoor() => UpdateDoorState(_closeOnPress);

    private void OnDestroy()
    {
        if (_linkedButton != null)
        {
            _linkedButton.OnButtonActivated.RemoveListener(OnButtonDown);
            _linkedButton.OnButtonDeactivated.RemoveListener(OnButtonUp);
        }
    }
}