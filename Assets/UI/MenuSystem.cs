using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuSystem : MonoBehaviour
{
    #region Variables
    [SerializeField] enum Menu { Play, Options, Inventory, Death }
    [SerializeField] Menu menu;

    [Header("Play menu")]
    [SerializeField] GameObject playMenuObject;
    [SerializeField] TMP_Text OnScreenStats;

    [Header("Options menu")]
    [SerializeField] GameObject optionsMenuObject;
    [SerializeField] GameObject resumeButton;
    public static bool gameIsPaused;

    [Header("Inventory menu")]
    [SerializeField] GameObject inventoryMenuObject;
    [SerializeField] GameObject[] inventoryButtons;
    [SerializeField] Sprite emptyImage;
    public static ItemObject[] inventoryItems;
    public static bool invIsOpen;

    [Header("Death menu")]
    [SerializeField] GameObject deathMenuObject;
    [SerializeField] GameObject mainMenuButton;
    bool isDead;

    Player player;
    GameObject activeMenu;
    bool firstButtonIsSet;
    bool isMovingItem;
    int selectedItem;

    // chat related stuff
    [SerializeField] TMP_Text onScreenChat;
    List<string> chat;

    #endregion


    #region OnEnable/Start/Update
    private void OnEnable()
    {
        player = FindObjectOfType<Player>();
    }


    private void Start()
    {
        inventoryItems = new ItemObject[5];
        invIsOpen = false;
        gameIsPaused = false;
        chat = new List<string>();
    }

    private void Update()
    {
        if (player.healt <= 0 && !isDead)
        {
            isDead = true;
            SwitchMenu(deathMenuObject, Menu.Death);
        }

        switch (menu)
        {
            case Menu.Play: PlayBehaviour(); break;
            case Menu.Options: OptionsBehaviour(); break;
            case Menu.Inventory: InventoryBehaviour(); break;
            case Menu.Death: DeathBehaviour(); break;
        }
        StatsLog();
    }
    #endregion


    #region Play related
    void PlayBehaviour()
    {
        Time.timeScale = 1;
        invIsOpen = false;
        gameIsPaused = false;

        // If player pressed button to go to options
        if (PlayerInput.startButtonPressed)
        {
            SwitchMenu(optionsMenuObject, Menu.Options);
        }

        // If player presses button go to inventory
        if (PlayerInput.northPressed && !GameStates.enemyRunning)
        {
            SwitchMenu(inventoryMenuObject, Menu.Inventory);
        }
    }
    #endregion


    #region Options related
    void OptionsBehaviour()
    {
        Time.timeScale = 0;
        gameIsPaused = true;
        SetFirstSelectedButton(resumeButton);

        // If player presses button to resume game
        if (!PlayerInput.startButtonPressed)
        {
            SwitchMenu(playMenuObject, Menu.Play);
        }
    }


    public void ResumeButton()
    {
        SwitchMenu(playMenuObject, Menu.Play);
    }


    public void QuitGame()
    {
        Application.Quit();
    }


    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    #endregion


    #region Inventory related
    void InventoryBehaviour()
    {
        Time.timeScale = 0;
        invIsOpen = true;

        // Player exits inventory menu
        if (!PlayerInput.northPressed)
        {
            SwitchMenu(playMenuObject, Menu.Play);
            CloseAllSubMenu();
            if (isMovingItem) { moveItem(selectedItem); }
        }

        // Player selects in between items IF there are any
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] == null)
            {
                inventoryButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = emptyImage;
            }
            else
            {
                inventoryButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = inventoryItems[i].spriteImage;
                SetFirstSelectedButton(inventoryButtons[i]);
            }
        }

        if (!firstButtonIsSet) SetFirstSelectedButton(inventoryButtons[0]);
    }


    public void UseItem(int itemIndex)
    {
        if (inventoryItems[itemIndex] == null) return;
        if (!inventoryItems[itemIndex].hasActive)
        {
            StartCoroutine(DisplayThenRemoveChat(inventoryItems[itemIndex].itemName + " has no active!"));
            return;
        }
        inventoryItems[itemIndex].DoActive();
        inventoryItems[itemIndex] = null;
        CloseAllSubMenu();
        firstButtonIsSet = false;
    }


    public void DropItem(int itemIndex)
    {
        if (inventoryItems[itemIndex] == null) return;
        inventoryItems[itemIndex] = null;
        firstButtonIsSet = false;
        if (isMovingItem) { moveItem(selectedItem); }
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


    public void moveItem(int itemIndex)
    {
        if (isMovingItem)
        {
            if (inventoryItems[itemIndex] != inventoryItems[selectedItem])
            {
                ItemObject temp = inventoryItems[selectedItem];
                inventoryItems[selectedItem] = inventoryItems[itemIndex];
                inventoryItems[itemIndex] = temp;
            }
            isMovingItem = false;
            ColorBlock resetBlock = inventoryButtons[itemIndex].GetComponent<Button>().colors;
            resetBlock.normalColor = new Color(0, 0, 0, 0);
            resetBlock.selectedColor = new Color(0.716f, 0.716f, 0.716f);
            inventoryButtons[selectedItem].GetComponent<Button>().colors = resetBlock;
            return;
        }

        isMovingItem = true;
        selectedItem = itemIndex;
        ColorBlock setBlock = inventoryButtons[itemIndex].GetComponent<Button>().colors;
        setBlock.normalColor = Color.cyan;
        setBlock.selectedColor = Color.cyan;
        inventoryButtons[selectedItem].GetComponent<Button>().colors = setBlock;
    }
    #endregion


    #region UI related
    void SetFirstSelectedButton(GameObject button)
    {
        if (firstButtonIsSet) return;
        firstButtonIsSet = true;
        EventSystem.current.SetSelectedGameObject(button);
    }


    void SwitchMenu(GameObject newMenu, Menu menuName)
    {
        activeMenu?.SetActive(false);
        activeMenu = newMenu;
        activeMenu?.SetActive(true);

        menu = menuName;
        firstButtonIsSet = false;
    }


    void ChatLog()
    {
        onScreenChat.text = "";
        if (chat.Count <= 0) return;

        foreach (string text in chat)
        {
            onScreenChat.text += text + "\n";
        }
    }


    public IEnumerator DisplayThenRemoveChat(string addText)
    {
        chat.Add(addText);
        ChatLog();
        yield return new WaitForSecondsRealtime(3);
        chat.RemoveAt(0);
        ChatLog();
    }


    void StatsLog()
    {
        OnScreenStats.text = "Attack damage: " + Player.currentDamage + "\nShield: " + Player.currentShield + "\nHealth: " + player.healt;
    }
    #endregion


    #region Death related
    void DeathBehaviour()
    {
        gameIsPaused = true;
        deathMenuObject.SetActive(true);
        SetFirstSelectedButton(mainMenuButton);
    }
    #endregion
}
