using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelFinishScreen : UIScreen
{
    Button m_RestartButton;
    Button m_MainMenuButton;
    Button m_NextLevelButton;
    Label m_ElapsedTimeLabel;
    public LevelFinishScreen(VisualElement parentElement) : base(parentElement)
    {
        SetupButtons();
        RegisterCallbacks();
        SubscribeToEvents();
    }

    public override void Disable()
    {
        base.Disable();
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameEvents.LevelFinishedWithTime += GameEvents_LevelFinishedWithTime;
    }

    private void UnsubscribeFromEvents()
    {
        GameEvents.LevelFinishedWithTime -= GameEvents_LevelFinishedWithTime;
    }

    private void GameEvents_LevelFinishedWithTime(string time)
    {
        m_ElapsedTimeLabel.text = "Elapsed time: " + time;
    }

    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_RestartButton, evt => UIScreenEvents.OnLevelRestart?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_MainMenuButton, evt => UIScreenEvents.MainMenuClicked?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_NextLevelButton, evt => UIScreenEvents.OnNextLevel?.Invoke());
    }

    private void SetupButtons()
    {
        m_NextLevelButton = m_RootElement.Q<Button>("next-level-button");
        m_RestartButton = m_RootElement.Q<Button>("restart-button");
        m_MainMenuButton = m_RootElement.Q<Button>("main-menu-button");
        m_ElapsedTimeLabel = m_RootElement.Q<Label>("elapsed-time-label");
    }
}
