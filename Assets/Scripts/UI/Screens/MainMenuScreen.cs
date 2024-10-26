using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuScreen : UIScreen
{
    Button m_StartButton;
    Button m_LevelSelectButton;
    Button m_SettingsButton;
    Button m_QuitButton;
    Button m_StartDungeonButton;
    public MainMenuScreen(VisualElement parentElement) : base(parentElement)
    {
        SetupButtons();
        RegisterCallbacks();
        DisablePlayButtonsOnClient();

    }

    private void DisablePlayButtonsOnClient()
    {
        if (StartingSceneController.ChoosenPlayMode == StartingSceneController.PlayMode.Client)
        {
            m_StartButton.style.display = DisplayStyle.None;
            m_StartDungeonButton.style.display = DisplayStyle.None;
            m_LevelSelectButton.style.display = DisplayStyle.None;
            /*m_StartButton.SetEnabled(false);
            m_StartDungeonButton.SetEnabled(false);
            m_LevelSelectButton.SetEnabled(false);*/
        }
    }

    private void SetupButtons()
    {
        m_StartButton = m_RootElement.Q<Button>("start-button");

        m_LevelSelectButton = m_RootElement.Q<Button>("level-select-button");

        m_SettingsButton = m_RootElement.Q<Button>("settings-button");

        m_QuitButton = m_RootElement.Q<Button>("quit-button");
        m_StartDungeonButton = m_RootElement.Q<Button>("start-button-dungeon");
    }

    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_StartButton, evt => UIScreenEvents.OnGameStart?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_SettingsButton, evt => UIScreenEvents.SettingsShown?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_LevelSelectButton, evt => UIScreenEvents.LevelSelectShown?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_QuitButton, evt => Application.Quit());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_StartDungeonButton, evt => UIScreenEvents.OnDungeonGameStart?.Invoke());
    }

    public override void Show()
    {
        m_RootElement.style.display = DisplayStyle.Flex;
    }
}
