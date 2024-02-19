using UnityEngine.UIElements;

public class SettingsScreen : UIScreen
{
    Button m_BackButton;
    public SettingsScreen(VisualElement parentElement) : base(parentElement)
    {
        SetVisualElements();
        RegisterCallbacks();
    }

    private void SetVisualElements()
    {
        m_BackButton = m_RootElement.Q<Button>("back-button");
    }
    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => UIScreenEvents.ScreenClosed?.Invoke());
    }


}
