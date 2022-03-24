using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] GameObject[] inventoryButtons;
    public static ItemObject[] inventoryItems;

    GameObject activeMenu;
    bool firstButtonIsSet;

    private void Start()
    {
        inventoryItems = new ItemObject[5];
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
        if (PlayerInput.startButtonPressed)
        {
            SwitchMenu(optionsMenuObject, Menu.Options); firstButtonIsSet = false;
        }
        if (PlayerInput.northPressed && !GameStates.enemyRunning)
        {
            SwitchMenu(inventoryMenuObject, Menu.Inventory);
            firstButtonIsSet = false;
        }
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
        Time.timeScale = 0;
        if (!PlayerInput.northPressed)
        {
            SwitchMenu(playMenuObject, Menu.Play);
            firstButtonIsSet = false;
            CloseAllSubMenu();
        }

        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] == null)
            {
                inventoryButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                inventoryButtons[i].SetActive(false);
            }
            else
            {
                inventoryButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = inventoryItems[i].spriteImage;
                SetFirstSelectedButton(inventoryButtons[i]);
                inventoryButtons[i].SetActive(true);
            }
        }
    }

    public void UseItem(int itemIndex)
    {
        if (inventoryItems[itemIndex] == null) return;
        Debug.Log("Item slot " + itemIndex + " contains " + inventoryItems[itemIndex].itemName);
    }


    public void DropItem(int itemIndex)
    {
        if (inventoryItems[itemIndex] == null) return;
        inventoryItems[itemIndex] = null;
        firstButtonIsSet = false;
    }


    public void OpenItemSubMenu(int itemIndex)
    {
        if (inventoryItems[itemIndex] == null) return;
        inventoryButtons[itemIndex].transform.GetChild(1).gameObject.SetActive(true);
        inventoryButtons[itemIndex].transform.GetChild(2).gameObject.SetActive(true);
    }


    public void CloseAllSubMenu()
    {
        for (int i = 0; i < inventoryButtons.Length; i++)
        {
            inventoryButtons[i].transform.GetChild(1).gameObject.SetActive(false);
            inventoryButtons[i].transform.GetChild(2).gameObject.SetActive(false);
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }


    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }


}
