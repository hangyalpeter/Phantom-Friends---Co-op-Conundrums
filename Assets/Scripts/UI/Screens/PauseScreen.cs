using UnityEngine.UIElements;

public class PauseScreen : UIScreen
{
    Button m_ResumeButton;
    Button m_RestartButton;
    Button m_MainMenuButton;
    Button m_SettingsButton;
    Label m_ElapsedTimeLabel;
    public PauseScreen(VisualElement parentElement) : base(parentElement)
    {
        SetupButtons();
        RegisterCallbacks();
        SubscribeToEvents();
    }

    private void SetupButtons()
    {
        m_ResumeButton = m_RootElement.Q<Button>("resume-button");

        m_RestartButton = m_RootElement.Q<Button>("restart-button");

        m_MainMenuButton = m_RootElement.Q<Button>("main-menu-button");

        m_SettingsButton = m_RootElement.Q<Button>("settings-button");

        m_ElapsedTimeLabel = m_RootElement.Q<Label>("elapsed-time-label");
    }

    private void SubscribeToEvents()
    {
        GameEvents.GamePaused += GameEvents_GamePaused;
        StartingSceneController.PlayModeChanged += StartingSceneController_PlaymodeChanged;
    }

    private void StartingSceneController_PlaymodeChanged(StartingSceneController.PlayMode mode)
    {
        if (mode == StartingSceneController.PlayMode.Client)
        {
            m_RestartButton.style.display = DisplayStyle.None;
            m_MainMenuButton.style.display = DisplayStyle.None;
        }

        if (mode == StartingSceneController.PlayMode.Host)
        {
            m_RestartButton.style.display = DisplayStyle.Flex;
            m_MainMenuButton.style.display = DisplayStyle.Flex;
        }

        if (mode == StartingSceneController.PlayMode.CouchCoop)
        {
            m_RestartButton.style.display = DisplayStyle.Flex;
            m_MainMenuButton.style.display = DisplayStyle.Flex;
        }
    }

    private void GameEvents_GamePaused(string elapsedTime)
    {
        m_ElapsedTimeLabel.text = "Elapsed time: " + elapsedTime;
    }

    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_ResumeButton, evt => UIScreenEvents.ScreenClosed?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_RestartButton, evt => UIScreenEvents.OnLevelRestart?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_MainMenuButton, evt => UIScreenEvents.MainMenuClicked?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_SettingsButton, evt => UIScreenEvents.SettingsShown?.Invoke());
    }

    public override void Disable()
    {
        base.Disable();
        GameEvents.GamePaused -= GameEvents_GamePaused;
        StartingSceneController.PlayModeChanged -= StartingSceneController_PlaymodeChanged;
    }
}
