using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MonoBehaviour
{
    [SerializeField] enum Menu { Play, Options, Inventory }
    [SerializeField] Menu menu;

    bool pressOnce;

    private void Update()
    {
        switch (menu)
        {
            case Menu.Play: PlayBehaviour(); break;
            case Menu.Options: OptionsBehaviour(); break;
            case Menu.Inventory: InventoryBehaviour(); break;
        }
    }

    void PlayBehaviour()
    {
        if (PlayerInput.startButtonPressed && !pressOnce)
        {
            pressOnce = true;
            menu = Menu.Options;
        }
        else if (PlayerInput.startButtonPressed) pressOnce = false;
    }

    void OptionsBehaviour()
    {
        if (PlayerInput.startButtonPressed && !pressOnce)
        {
            pressOnce = true;
            menu = Menu.Play;
        }
        else if (PlayerInput.startButtonPressed) pressOnce = false;
    }

    void InventoryBehaviour()
    {
        if (PlayerInput.startButtonPressed && !pressOnce)
        {
            pressOnce = true;
            menu = Menu.Play;
        }
        else if (PlayerInput.startButtonPressed) pressOnce = false;
    }

}
