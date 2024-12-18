using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [Tooltip("Required UI Document")]
    [SerializeField] UIDocument m_Document;

    UIScreen m_MainMenuScreen;
    UIScreen m_SettingsScreen;
    UIScreen m_PauseScreen;
    UIScreen m_LevelFinishScreen;
    UIScreen m_LevelSelectScreen;
    UIScreen m_DungeonGameOverScreen;
    UIScreen m_WaitingForPlayersScreen;
    UIScreen m_DisconnectMessageScreen;

    UIScreen m_CurrentScreen;

    Stack<UIScreen> m_History = new Stack<UIScreen>();

    List<UIScreen> m_Screens = new List<UIScreen>();

    public UIScreen CurrentScreen => m_CurrentScreen;
    public UIDocument Document => m_Document;

    private void OnEnable()
    {
        SubscribeToEvents();

        Coroutines.Initialize(this);

        Initialize();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void Initialize()
    {
        VisualElement root = m_Document.rootVisualElement;
        m_MainMenuScreen = new MainMenuScreen(root.Q<VisualElement>("MainMenuScreen"));
        m_SettingsScreen = new SettingsScreen(root.Q<VisualElement>("SettingsScreen"));
        m_PauseScreen = new PauseScreen(root.Q<VisualElement>("PauseScreen"));
        m_LevelFinishScreen = new LevelFinishScreen(root.Q<VisualElement>("LevelFinishScreen"));
        m_LevelSelectScreen = new LevelSelectScreen(root.Q<VisualElement>("LevelSelectScreen"));
        m_DungeonGameOverScreen = new DungeonGameOverScreen(root.Q<VisualElement>("DungeonGameOverScreen"));
        m_WaitingForPlayersScreen = new WaitingForPlayersScreen(root.Q<VisualElement>("WaitingForPlayersScreen"));
        m_DisconnectMessageScreen = new DisconnectMessageScreen(root.Q<VisualElement>("DisconnectMessageScreen"));

        RegisterUIScreens();
        HideScreens();

        Show(m_MainMenuScreen);
    }

    private void SubscribeToEvents()
    {
        UIScreenEvents.MainMenuShown += UIScreenEvents_MainMenuShown;
        UIScreenEvents.SettingsShown += UIScreenEvents_SettingsShown;
        UIScreenEvents.ScreenClosed += UIScreenEvents_ScreenClosed;
        UIScreenEvents.OnGameStart += HideScreens;
        UIScreenEvents.OnDungeonGameStart += HideScreens;
        UIScreenEvents.HideAllScreens += HideScreens;

        UIScreenEvents.PauseShown += UIScreenEvents_PauseShown;
        UIScreenEvents.LevelFinishShown += UIScreenEvents_LevelFinishShown;
        UIScreenEvents.LevelSelectShown += UIScreenEvents_LevelSelectShown;
        UIScreenEvents.OnLevelSelected += UIScreenEvents_LevelSelected;
        UIScreenEvents.DungeonGameOverShown += UIScreenEvents_DungeonGameOverShown;
        UIScreenEvents.WaitingForPlayersScreenShown += UIScreenEvents_WaitingForPlayersScreenShown;
        UIScreenEvents.DisconnectMessageShown += UIScreenEvents_DisconnectMessageShown;
    }

    private void UIScreenEvents_DisconnectMessageShown(string message)
    {
        Show(m_DisconnectMessageScreen, false);
    }

    private void UIScreenEvents_WaitingForPlayersScreenShown()
    {
        Show(m_WaitingForPlayersScreen, false);
    }

    private void UnsubscribeFromEvents()
    {
        UIScreenEvents.MainMenuShown -= UIScreenEvents_MainMenuShown;
        UIScreenEvents.SettingsShown -= UIScreenEvents_SettingsShown;
        UIScreenEvents.ScreenClosed -= UIScreenEvents_ScreenClosed;
        UIScreenEvents.OnGameStart -= HideScreens;
        UIScreenEvents.OnDungeonGameStart -= HideScreens;
        UIScreenEvents.PauseShown -= UIScreenEvents_PauseShown;
        UIScreenEvents.LevelFinishShown -= UIScreenEvents_LevelFinishShown;
        UIScreenEvents.LevelSelectShown -= UIScreenEvents_LevelSelectShown;
        UIScreenEvents.OnLevelSelected -= UIScreenEvents_LevelSelected;
        UIScreenEvents.DungeonGameOverShown -= UIScreenEvents_DungeonGameOverShown;
        UIScreenEvents.HideAllScreens -= HideScreens;
        UIScreenEvents.WaitingForPlayersScreenShown -= UIScreenEvents_WaitingForPlayersScreenShown;
    }

    private void RegisterUIScreens()
    {
        m_Screens = new List<UIScreen>()
        {
            m_MainMenuScreen,
            m_SettingsScreen,
            m_PauseScreen,
            m_LevelFinishScreen,
            m_LevelSelectScreen,
            m_DungeonGameOverScreen,
            m_WaitingForPlayersScreen,
            m_DisconnectMessageScreen
        };
    }

    public void UIScreenEvents_MainMenuShown()
    {

        m_CurrentScreen = m_MainMenuScreen;

        HideScreens();
        m_History.Push(m_MainMenuScreen);
        m_MainMenuScreen.ShowImmediately();
    }

    private void UIScreenEvents_ScreenClosed()
    {
        if (m_History.Count != 0)
        {
            Show(m_History.Pop(), false);
        }
        else
        {
            m_CurrentScreen.HideImmediately();
        }
    }

    private void UIScreenEvents_SettingsShown()
    {
        Show(m_SettingsScreen);
    }

    private void HideScreens()
    {
        m_History.Clear();

        foreach (UIScreen screen in m_Screens)
        {
            screen.HideImmediately();
        }
    }

    public void Show(UIScreen screen, bool keepInHistory = true)
    {
        if (screen == null)
            return;

        if (m_CurrentScreen != null)
        {
            if (!screen.IsTransparent)
                m_CurrentScreen.HideImmediately();

            if (keepInHistory)
            {
                m_History.Push(m_CurrentScreen);
            }
        }

        screen.ShowImmediately();
        m_CurrentScreen = screen;
    }

    private void UIScreenEvents_PauseShown()
    {
        Show(m_PauseScreen);
    }

    private void UIScreenEvents_LevelFinishShown()
    {
        Show(m_LevelFinishScreen);
    }

    private void UIScreenEvents_LevelSelectShown()
    {
        Show(m_LevelSelectScreen);
    }

    private void UIScreenEvents_DungeonGameOverShown()
    {
        Show(m_DungeonGameOverScreen);
    }

    private void UIScreenEvents_LevelSelected(string obj)
    {
        HideScreens();
    }

    public void Show(UIScreen screen)
    {
        Show(screen, true);
    }

}
