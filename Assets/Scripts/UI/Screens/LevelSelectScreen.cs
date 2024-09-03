using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelSelectScreen : UIScreen
{
    Button m_Level1Button;
    Button m_Level2Button;
    Button m_BackButton;
    Label m_Level1Label;
    Label m_Level2Label;
    
    public LevelSelectScreen(VisualElement parentElement) : base(parentElement)
    {
        SetupControls();
        SubscribeToEvents();
        RegisterCallbacks();

        if (PlayerPrefs.HasKey(m_Level1Label.text + "_BestCompletionTime"))
        {
            var bestCompletionTime1 = PlayerPrefs.GetFloat(m_Level1Label.text + "_BestCompletionTime");
            m_Level1Label.text += " - Best Time: " + bestCompletionTime1.ToString("F2");
        }

        if (PlayerPrefs.HasKey(m_Level2Label.text + "_BestCompletionTime"))
        {
            var bestCompletionTime2 = PlayerPrefs.GetFloat(m_Level2Label.text + "_BestCompletionTime");
            m_Level2Label.text += " - Best Time: " + bestCompletionTime2.ToString("F2");
        }

    }

    public override void Disable()
    {
        base.Disable();
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {

    }

    private void UnsubscribeFromEvents()
    {
    }

    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_Level1Button, evt => UIScreenEvents.OnLevelSelected?.Invoke(m_Level1Button.text));
        m_EventRegistry.RegisterCallback<ClickEvent>(m_Level2Button, evt => UIScreenEvents.OnLevelSelected?.Invoke(m_Level2Button.text));
        m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => UIScreenEvents.ScreenClosed?.Invoke());
    }

    private void SetupControls()
    {
        m_Level1Button = m_RootElement.Q<Button>("level1-button");
        m_Level2Button = m_RootElement.Q<Button>("level2-button");
        m_BackButton = m_RootElement.Q<Button>("back-button");
        m_Level1Label = m_RootElement.Q<Label>("level1-label");
        m_Level2Label = m_RootElement.Q<Label>("level2-label");
    }

   
}
