using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonobehaviour<UIManager>
{

    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    private bool _pauseMenuOn = false;
    [SerializeField] private GameObject pauseMenu = null; 
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;

    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        // Toggle pause menu if escape is pressed

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (PauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }
    }

    private void EnablePauseMenu()
    {
        // Destroy any currently dragged items
        uiInventoryBar.DestroyCurrentlyDraggedItems();

        // Clear currently selected items
        uiInventoryBar.ClearCurrentlySelectedItems();

        PauseMenuOn = true;
        Player.Instance.PlayerInputIsDisabled = true; //disable player movement
        //Time.timeScale = 0; //stop update method which means freezing the game
        pauseMenu.SetActive(true);
        
        //when enabled, the pause tab will be opened with the inventory tab selected (Tab0)
        SwitchPauseMenuTab(0);

        // Trigger garbage collector to clear memory
        System.GC.Collect();

        // Highlight selected button - cause the selected tab's icon to be with full opacity and the rest of icons with low opacity
        //HighlightButtonForSelectedTab();
    }

    public void DisablePauseMenu()
    {
        // Destroy any currently dragged items
        pauseMenuInventoryManagement.DestroyCurrentlyDraggedItems();

        PauseMenuOn = false;
        Player.Instance.PlayerInputIsDisabled = false;
        //Time.timeScale = 1; //re enable the game
        pauseMenu.SetActive(false);
    }

    private void HighlightButtonForSelectedTab() //loops all tabs and looks to see if each one is active or not
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if (menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(menuButtons[i]);
            }

            else
            {
                SetButtonColorToInactive(menuButtons[i]);
            }
        }
    }

    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.pressedColor;

        button.colors = colors;

    }

    private void SetButtonColorToInactive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.disabledColor;

        button.colors = colors;

    }

    //method used by the buttons on the PauseMenu to switch between selected tab
    public void SwitchPauseMenuTab(int tabNum)
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if (i != tabNum)
            {
                menuTabs[i].SetActive(false);
            }
            else
            {
                menuTabs[i].SetActive(true);

            }
        }

        //HighlightButtonForSelectedTab();

    }
    public void QuitGame()
    {
        Application.Quit();
    }

}
