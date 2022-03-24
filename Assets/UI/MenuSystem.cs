using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuSystem : MonoBehaviour
{
    [SerializeField] enum Menu { Play, Options, Inventory }
    [SerializeField] Menu menu;

    [Header("Play menu")]
    [SerializeField] GameObject playMenuObject;

    [Header("Options menu")]
    [SerializeField] GameObject optionsMenuObject;
    [SerializeField] GameObject resumeButton;

    [Header("Inventory menu")]
    [SerializeField] GameObject inventoryMenuObject;
    [SerializeField] GameObject firstInventoryButton;
    [SerializeField] GameObject[] inventoryItems;

    GameObject activeMenu;
    bool firstButtonIsSet;

    private void Awake()
    {
        inventoryItems = new GameObject[5];
    }

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
        Time.timeScale = 1;
        if (PlayerInput.startButtonPressed) { SwitchMenu(optionsMenuObject, Menu.Options); firstButtonIsSet = false;
    }
        if (PlayerInput.northPressed && !GameStates.enemyRunning) { SwitchMenu(inventoryMenuObject, Menu.Inventory); firstButtonIsSet = false; }
    }

    void OptionsBehaviour()
    {
        SetFirstSelectedButton(resumeButton);
        Time.timeScale = 0;
    }

    void SwitchMenu(GameObject newMenu, Menu menuName)
    {
        activeMenu?.SetActive(false);
        activeMenu = newMenu;
        activeMenu?.SetActive(true);

        menu = menuName;
    }


    public void ResumeButton()
    {
        SwitchMenu(playMenuObject, Menu.Play);
        firstButtonIsSet = false;
    }


    void SetFirstSelectedButton(GameObject button)
    {
        if (firstButtonIsSet) return;
        firstButtonIsSet = true;
        EventSystem.current.SetSelectedGameObject(button);
    }

    void InventoryBehaviour()
    {
        SetFirstSelectedButton(firstInventoryButton);
        Time.timeScale = 0;
        if (!PlayerInput.northPressed) { SwitchMenu(playMenuObject, Menu.Play); firstButtonIsSet = false; }

        // add items etc...
    }

    public void UseItem(int itemIndex)
    {
        if (inventoryItems[itemIndex] == null) return;
    }

}
