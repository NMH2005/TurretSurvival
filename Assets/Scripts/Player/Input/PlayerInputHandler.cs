using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour {
    public Vector2 RawMovementInput { get; private set; }
    public bool fireInput => isActiveAndEnabled &&
                             Application.isFocused &&
                             Time.timeScale > 0f &&
                             !requireFireRelease &&
                             attackAction != null &&
                             attackAction.IsPressed();
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public Vector2 MouseWorldPosition
    {
        get
        {
            if (Mouse.current == null || mainCamera == null)
                return transform.position;

            return mainCamera.ScreenToWorldPoint(
                Mouse.current.position.ReadValue());
        }
    }
    private Camera mainCamera;
    private WeaponSystem weaponSystem;
    private InputAction attackAction;
    private bool requireFireRelease;

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();
    }

    public void OnFireInput(InputAction.CallbackContext context)
    {
        attackAction = context.action;

        if (context.canceled)
        {
            requireFireRelease = false;
            return;
        }

        if (Time.timeScale <= 0f || !Application.isFocused)
            ClearFireInput();
        else if (context.started)
            requireFireRelease = false;
    }

    public void ClearFireInput()
    {
        requireFireRelease = true;
    }

    private void OnDisable()
    {
        ClearFireInput();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) ClearFireInput();
    }

    public void OnReloadInput(InputAction.CallbackContext context)
    {
        if (!context.performed || Time.timeScale <= 0f) return;
        if (weaponSystem == null) weaponSystem = GetComponentInChildren<WeaponSystem>();
        if (weaponSystem != null) weaponSystem.ManualWeapon?.TryReload();
    }
}
