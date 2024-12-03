using UnityEngine.UIElements;

public class DungeonGameOverScreen : UIScreen
{
    Button m_RestartButton;
    Button m_MainMenuButton;
    Button m_NewDungeonButton;

    Label m_FinishedRoomsLabel;
    Label m_EnemiesKilledLabel;
    Label m_ElapsedTimeLabel;
    Label m_GameOverLabel;
    public DungeonGameOverScreen(VisualElement parentElement) : base(parentElement)
    {
        SetupButtons();
        SubscribeToEvents();
        RegisterCallbacks();
    }
    private void SetupButtons()
    {
        m_GameOverLabel = m_RootElement.Q<Label>("game-over-label");

        m_RestartButton = m_RootElement.Q<Button>("restart-button");
        m_NewDungeonButton = m_RootElement.Q<Button>("new-dungeon-button");

        m_MainMenuButton = m_RootElement.Q<Button>("main-menu-button");

        m_FinishedRoomsLabel = m_RootElement.Q<Label>("finished-rooms-label");
        m_EnemiesKilledLabel = m_RootElement.Q<Label>("enemies-killed-label");
        m_ElapsedTimeLabel = m_RootElement.Q<Label>("elapsed-time-label");

    }
    private void SubscribeToEvents()
    {
        DungeonManager.OnDungeonFinish += UpdateLabels;

        GameEvents.LevelFinishedWithTime += OnLevelFinish;
        StartingSceneController.PlayModeChanged += StartingSceneController_PlaymodeChanged;
    }
    private void StartingSceneController_PlaymodeChanged(StartingSceneController.PlayMode mode)
    {
        if (mode == StartingSceneController.PlayMode.Client)
        {
            m_NewDungeonButton.style.display = DisplayStyle.None;
            m_RestartButton.style.display = DisplayStyle.None;
            m_MainMenuButton.style.display = DisplayStyle.None;
        }

        if (mode == StartingSceneController.PlayMode.Host)
        {
            m_NewDungeonButton.style.display = DisplayStyle.Flex;
            m_RestartButton.style.display = DisplayStyle.Flex;
            m_MainMenuButton.style.display = DisplayStyle.Flex;
        }

        if (mode == StartingSceneController.PlayMode.CouchCoop)
        {
            m_NewDungeonButton.style.display = DisplayStyle.Flex;
            m_RestartButton.style.display = DisplayStyle.Flex;
            m_MainMenuButton.style.display = DisplayStyle.Flex;
        }
    }
    public override void Disable()
    {
        base.Disable();
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
        DungeonManager.OnDungeonFinish -= UpdateLabels;
        GameEvents.LevelFinishedWithTime -= OnLevelFinish;
    }
    private void UpdateLabels(int roomsCleared, int enemiesKilled, string title, string restartLabel)
    {
        m_GameOverLabel.text = title;
        m_FinishedRoomsLabel.text = "Rooms cleared: " + roomsCleared;
        m_EnemiesKilledLabel.text = "Enemies killed: " + enemiesKilled;
        if (restartLabel == "Restart")
        {

            if (StartingSceneController.ChoosenPlayMode != StartingSceneController.PlayMode.Client)
            {
                m_NewDungeonButton.style.display = DisplayStyle.None;
                m_RestartButton.style.display = DisplayStyle.Flex;
            }
        }
        else
        {
            if (StartingSceneController.ChoosenPlayMode != StartingSceneController.PlayMode.Client)
            {
                m_NewDungeonButton.style.display = DisplayStyle.Flex;
                m_RestartButton.style.display = DisplayStyle.None;
            }
        }
    }
    private void OnLevelFinish(string time)
    {
        m_ElapsedTimeLabel.text = "Elapsed time: " + time;
    }

    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_RestartButton, evt => UIScreenEvents.OnLevelRestart?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_NewDungeonButton, evt => GameEvents.OnNewDungeon?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_MainMenuButton, evt => UIScreenEvents.MainMenuClicked?.Invoke());
    }
}
