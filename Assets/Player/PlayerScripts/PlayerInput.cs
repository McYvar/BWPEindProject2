using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    // used for walking
    public static Vector2 leftJoy;
    public void LeftJoy(InputAction.CallbackContext c)
    {
        leftJoy = c.ReadValue<Vector2>();
    }

    // used for navigating UI
    public static Vector2 rightJoy;
    public void RightJoy(InputAction.CallbackContext c)
    {
        rightJoy = c.ReadValue<Vector2>();
    }

    // -
    public static Vector2 dPad;
    public void DPad(InputAction.CallbackContext c)
    {
        dPad = c.ReadValue<Vector2>();
    }

    // used for inventory
    public static bool northPressed;
    public void North(InputAction.CallbackContext c)
    {
        if (MenuSystem.gameIsPaused) return;
        if (c.started) northPressed = !northPressed;
    }

    // spellcast
    public static bool eastPressed;
    public void East(InputAction.CallbackContext c)
    {
        if (c.started) eastPressed = true;
        if (c.canceled) eastPressed = false;
    }

    // used for jumping two spaces and UI confirmation
    public static bool southPressed;
    public void South(InputAction.CallbackContext c)
    {
        if (c.started) southPressed = true;
        if (c.canceled) southPressed = false;

        if (MenuSystem.gameIsPaused)
        {
            startButtonPressed = false;
            southPressed = false;
        }
    }

    // used for moving faster
    public static bool westPressed;
    public void West(InputAction.CallbackContext c)
    {
        if (c.started) westPressed = true;
        if (c.canceled) westPressed = false;
    }

    // used for rotating left
    public static bool leftShoulderPressed;
    public void LeftShoulder(InputAction.CallbackContext c)
    {
        if (c.started) leftShoulderPressed = true;
        if (c.canceled) leftShoulderPressed= false;
    }

    // used for rotating right
    public static bool rightShoulderPressed;
    public void RightShoulder(InputAction.CallbackContext c)
    {
        if (c.started) rightShoulderPressed = true;
        if (c.canceled) rightShoulderPressed = false;
    }

    // -
    public static float leftTrigger;
    public void LeftTrigger(InputAction.CallbackContext c)
    {
        leftTrigger = c.ReadValue<float>();
    }

    public static float rightTrigger;
    public void RightTrigger(InputAction.CallbackContext c)
    {
        rightTrigger = c.ReadValue<float>();
    }

    // used for opening options menu
    public static bool startButtonPressed;
    public void StartButton(InputAction.CallbackContext c)
    {
        if (MenuSystem.invIsOpen) return;
        if (c.started) startButtonPressed = !startButtonPressed;
    }

    // -
    public static bool selectPressed;
    public void Select(InputAction.CallbackContext c)
    {
        if (c.performed) selectPressed = true;
        if (c.canceled) selectPressed = false;
    }

    // -
    public static bool leftJoyPressed;
    public void LeftJoyPress(InputAction.CallbackContext c)
    {
        if (c.started) leftJoyPressed = true;
        if (c.canceled) leftJoyPressed = false;
    }

    // -
    public static bool rightJoyPressed;
    public void RightJoyPress(InputAction.CallbackContext c)
    {
        if (c.started) rightJoyPressed = true;
        if (c.canceled) rightJoyPressed = false;
    }

}
