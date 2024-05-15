using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    // ��ͣ�˵��Ƿ��
    private bool _pauseMenuOn = false;

    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;

    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();
        // ��ʼ��ʱ����ͣ�˵���Ϊ�ǻ״̬
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        // ����Esc��ʱ�л���ͣ�˵�״̬
        if (Input.GetKeyDown(KeyCode.Escape))
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

    public void EnablePauseMenu()
    {
        uiInventoryBar.DestroyCurrentlyDraggedItems();
        uiInventoryBar.ClearCurrentlySelectedItems();

        PauseMenuOn = true;
        Player.Instance.PlayerInputIsDisabled = true;
        Time.timeScale = 0; // ��ͣ��Ϸʱ��
        pauseMenu.SetActive(true);

        System.GC.Collect(); // ������������

        HighlightButtonForSelectedTab();
    }

    public void DisablePauseMenu()
    {
        pauseMenuInventoryManagement.DestroyCurrentlyDraggedItems();

        PauseMenuOn = false;
        Player.Instance.PlayerInputIsDisabled = false;
        Time.timeScale = 1; // �ָ���Ϸʱ��
        pauseMenu.SetActive(false);
    }

    private void HighlightButtonForSelectedTab()
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

        HighlightButtonForSelectedTab();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}