using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public static Vector2 leftJoy;
    public void LeftJoy(InputAction.CallbackContext c)
    {
        leftJoy = c.ReadValue<Vector2>();
    }

    public static Vector2 rightJoy;
    public void RightJoy(InputAction.CallbackContext c)
    {
        rightJoy = c.ReadValue<Vector2>();
    }

    public static Vector2 dPad;
    public void DPad(InputAction.CallbackContext c)
    {
        if (c.performed) dPad = c.ReadValue<Vector2>();
    }

    public static bool northPressed;
    public void North(InputAction.CallbackContext c)
    {
        if (c.performed) northPressed = true;
    }

    public static bool eastPressed;
    public void East(InputAction.CallbackContext c)
    {
        if (c.performed) eastPressed = true;
    }

    public static bool southPressed;
    public void South(InputAction.CallbackContext c)
    {
        if (c.performed) southPressed = true;
    }

    public static bool westPressed;
    public void West(InputAction.CallbackContext c)
    {
        if (c.performed) westPressed = true;
        if (c.canceled) westPressed = false;
    }

    public static bool leftShoulderPressed;
    public void LeftShoulder(InputAction.CallbackContext c)
    {
        if (c.performed) leftShoulderPressed = true;
    }

    public static bool rightShoulderPressed;
    public void RightShoulder(InputAction.CallbackContext c)
    {
        if (c.performed) rightShoulderPressed = true;
    }

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

    public static bool startButtonPressed;
    public void StartButton(InputAction.CallbackContext c)
    {
        if (c.performed) startButtonPressed = true;
    }

    public static bool selectPressed;
    public void Select(InputAction.CallbackContext c)
    {
        if (c.performed) selectPressed = true;
    }

    public static bool leftJoyPressed;
    public void LeftJoyPress(InputAction.CallbackContext c)
    {
        if (c.performed) leftJoyPressed = true;
    }

    public static bool rightJoyPressed;
    public void RightJoyPress(InputAction.CallbackContext c)
    {
        if (c.performed) rightJoyPressed = true;
    }

}
