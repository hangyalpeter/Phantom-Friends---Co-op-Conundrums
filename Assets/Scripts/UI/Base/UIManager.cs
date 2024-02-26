using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{

    [Tooltip("Required UI Document")]
    [SerializeField] UIDocument m_Document;

    UIScreen m_MainMenuScreen;
    UIScreen m_SettingsScreen;

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

        RegisterUIScreens();
        HideScreens();

        Debug.Log("UI Manager Initialized");
        m_CurrentScreen = m_MainMenuScreen;
        m_History.Push(m_MainMenuScreen);
        m_MainMenuScreen.ShowImmediately();
    }

    private void SubscribeToEvents()
    {
        UIScreenEvents.MainMenuShown += UIScreenEvents_MainMenuShown;
        UIScreenEvents.SettingsShown += UIScreenEvents_SettingsShown;
        UIScreenEvents.ScreenClosed += UIScreenEvents_ScreenClosed;
        UIScreenEvents.GameStarted += HideScreens;
        UIScreenEvents.SettingsShownInPlayMode += UIScreenEvents_SettingsShownInPlayMode;
    }

    private void UnsubscribeFromEvents()
    {
        UIScreenEvents.MainMenuShown -= UIScreenEvents_MainMenuShown;
        UIScreenEvents.SettingsShown -= UIScreenEvents_SettingsShown;
        UIScreenEvents.ScreenClosed -= UIScreenEvents_ScreenClosed;
        UIScreenEvents.GameStarted -= HideScreens;
        UIScreenEvents.SettingsShownInPlayMode -= UIScreenEvents_SettingsShownInPlayMode;
    }

    private void RegisterUIScreens()
    {
        m_Screens = new List<UIScreen>()
        {
            m_MainMenuScreen,
            m_SettingsScreen
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
        } else
        {
           m_CurrentScreen.HideImmediately(); 
        }
    }

    private void UIScreenEvents_SettingsShown()
    {
        Show(m_SettingsScreen);
    }

    // TODO: this will be used for pause menu, this is just a test if this works
    private void UIScreenEvents_SettingsShownInPlayMode(bool playMode)
    {
        if (playMode)
        {
            Show(m_SettingsScreen, false);
        }
        else
        {
            Show(m_SettingsScreen);
        }
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
        Debug.Log("Current Screen: " + m_CurrentScreen.ToString());
        Debug.Log("History Count: " + m_History.Count);
    }

    public void Show(UIScreen screen)
    {
        Show(screen, true);
    }

}
