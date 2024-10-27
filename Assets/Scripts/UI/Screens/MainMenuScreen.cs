using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuScreen : UIScreen
{
    Button m_ReadyButton;
    Button m_StartButton;
    Button m_LevelSelectButton;
    Button m_SettingsButton;
    Button m_QuitButton;
    Button m_StartDungeonButton;
    public MainMenuScreen(VisualElement parentElement) : base(parentElement)
    {
        SetupButtons();
        RegisterCallbacks();
        StartingSceneController.PlayModeChanged += DisablePlayButtonsOnClient;
    }

    private void DisablePlayButtonsOnClient(StartingSceneController.PlayMode playmode)
    {
        if (playmode == StartingSceneController.PlayMode.Client)
        {
            m_ReadyButton.style.display = DisplayStyle.Flex;
            m_StartButton.style.display = DisplayStyle.None;
            m_StartDungeonButton.style.display = DisplayStyle.None;
            m_LevelSelectButton.style.display = DisplayStyle.None;
        }

        if (playmode == StartingSceneController.PlayMode.Host)
        {
            m_ReadyButton.style.display = DisplayStyle.None;
            m_StartButton.style.display = DisplayStyle.Flex;
            m_StartDungeonButton.style.display = DisplayStyle.Flex;
            m_LevelSelectButton.style.display = DisplayStyle.Flex;
        }

        if (playmode == StartingSceneController.PlayMode.CouchCoop)
        {
            m_ReadyButton.style.display = DisplayStyle.None;
            m_StartButton.style.display = DisplayStyle.Flex;
            m_StartDungeonButton.style.display = DisplayStyle.Flex;
            m_LevelSelectButton.style.display = DisplayStyle.Flex;
        }
    }

    private void SetupButtons()
    {
        m_ReadyButton = m_RootElement.Q<Button>("ready-button");

        m_StartButton = m_RootElement.Q<Button>("start-button");

        m_LevelSelectButton = m_RootElement.Q<Button>("level-select-button");

        m_SettingsButton = m_RootElement.Q<Button>("settings-button");

        m_QuitButton = m_RootElement.Q<Button>("quit-button");
        m_StartDungeonButton = m_RootElement.Q<Button>("start-button-dungeon");
    }

    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_ReadyButton, evt => UIScreenEvents.OnClientReady?.Invoke());

        m_EventRegistry.RegisterCallback<ClickEvent>(m_StartButton, evt => UIScreenEvents.OnHostReady?.Invoke(Scene.Level_1));

        m_EventRegistry.RegisterCallback<ClickEvent>(m_SettingsButton, evt => UIScreenEvents.SettingsShown?.Invoke());

        m_EventRegistry.RegisterCallback<ClickEvent>(m_LevelSelectButton, evt => UIScreenEvents.LevelSelectShown?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_QuitButton, evt => UIScreenEvents.OnBackToTitleScreen?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_StartDungeonButton, evt => UIScreenEvents.OnHostReady?.Invoke(Scene.Dungeon_Crawler));
    }

    public override void Show()
    {
        m_RootElement.style.display = DisplayStyle.Flex;
    }
}
