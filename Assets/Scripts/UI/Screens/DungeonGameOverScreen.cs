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
    Label m_ElapsedTimeLabel;
    public DungeonGameOverScreen(VisualElement parentElement) : base(parentElement)
    {
        SetupButtons();
        SubscribeToEvents();
        RegisterCallbacks();
    }

    private void SetupButtons()
    {

        m_RestartButton = m_RootElement.Q<Button>("restart-button");

        m_MainMenuButton = m_RootElement.Q<Button>("main-menu-button");
        m_ExitButton = m_RootElement.Q<Button>("exit-button");

        m_FinishedRoomsLabel = m_RootElement.Q<Label>("finished-rooms-label");
        m_ElapsedTimeLabel = m_RootElement.Q<Label>("elapsed-time-label");


    }

    private void SubscribeToEvents()
    {
        DungeonLogicHandler.OnPLayerDied += UpdateLabels;

        GameEvents.LevelFinishedWithTime += OnLevelFinish;
    }

   public override void Disable()
    {
        base.Disable();
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
        DungeonLogicHandler.OnPLayerDied -= UpdateLabels;
        GameEvents.LevelFinishedWithTime -= OnLevelFinish;
    }

    private void UpdateLabels(int roomsCleared)
    {
        m_FinishedRoomsLabel.text = "Rooms cleared: " + roomsCleared;
    }

    private void OnLevelFinish(string time)
    {
        m_ElapsedTimeLabel.text = time;
    }

    private void RegisterCallbacks() 
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_ExitButton, evt => Application.Quit());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_RestartButton, evt => UIScreenEvents.OnLevelRestart?.Invoke());
        m_EventRegistry.RegisterCallback<ClickEvent>(m_MainMenuButton, evt => UIScreenEvents.MainMenuClicked?.Invoke());
    }


}
