using System;
using UnityEngine.UIElements;

public class DisconnectMessageScreen : UIScreen
{
    private Button m_OkButton;
    private Label m_MessageLabel;
    public DisconnectMessageScreen(VisualElement parentElement) : base(parentElement)
    {
        SetUpButtons();
        RegisterCallbacks();
        SubscribeToEvents();
    }

    private void UIScreenEvents_DisconnectMessageShown(string obj)
    {
        m_MessageLabel.text = obj;
    }

    private void SetUpButtons()
    {
        m_OkButton = m_RootElement.Q<Button>("ok-button");
        m_MessageLabel = m_RootElement.Q<Label>("disconnect-label");
    }
    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_OkButton, evt => UIScreenEvents.ScreenClosed?.Invoke());
    }
    private void SubscribeToEvents()
    {
        UIScreenEvents.DisconnectMessageShown += UIScreenEvents_DisconnectMessageShown;
    }
    private void UnsubscribeFromEvents()
    {
        UIScreenEvents.DisconnectMessageShown -= UIScreenEvents_DisconnectMessageShown;
    }
    public override void Disable()
    {
        base.Disable();
        UnsubscribeFromEvents();
    }


}


