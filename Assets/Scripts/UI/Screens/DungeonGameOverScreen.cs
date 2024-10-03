using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonGameOverScreen : UIScreen
{
    Button m_RestartButton;
    Button m_MainMenuButton;
    Button m_ExitButton;

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

        m_MainMenuButton = m_RootElement.Q<Button>("main-menu-button");
        m_ExitButton = m_RootElement.Q<Button>("exit-button");

        m_FinishedRoomsLabel = m_RootElement.Q<Label>("finished-rooms-label");
        m_EnemiesKilledLabel = m_RootElement.Q<Label>("enemies-killed-label");
        m_ElapsedTimeLabel = m_RootElement.Q<Label>("elapsed-time-label");


    }

    private void SubscribeToEvents()
    {
        //DungeonLogicHandler.OnDungeonFinish += UpdateLabels;
        DungeonManager.OnDungeonFinish += UpdateLabels;

        GameEvents.LevelFinishedWithTime += OnLevelFinish;
    }

   public override void Disable()
    {
        base.Disable();
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
        //DungeonLogicHandler.OnDungeonFinish -= UpdateLabels;
        DungeonManager.OnDungeonFinish -= UpdateLabels;
        GameEvents.LevelFinishedWithTime -= OnLevelFinish;
    }

    private void UpdateLabels(int roomsCleared, int enemiesKilled, string title, string restartLabel)
    {
        m_FinishedRoomsLabel.text = "Rooms cleared: " + roomsCleared;
        m_EnemiesKilledLabel.text = "Enemies killed: " + enemiesKilled;
        m_GameOverLabel.text = title;
        m_RestartButton.text = restartLabel;
    }

    private void OnLevelFinish(string time)
    {
        m_ElapsedTimeLabel.text = "Elapsed time: " + time;
    }

    private void RegisterCallbacks() 
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_ExitButton, evt => Application.Quit());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_RestartButton, evt => UIScreenEvents.OnLevelRestart?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_MainMenuButton, evt => UIScreenEvents.MainMenuClicked?.Invoke());
    }


}
