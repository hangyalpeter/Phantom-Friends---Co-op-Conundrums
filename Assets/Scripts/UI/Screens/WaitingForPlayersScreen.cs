using UnityEngine.UIElements;

public class WaitingForPlayersScreen : UIScreen
{
    private Button m_UnreadyButton;
    public WaitingForPlayersScreen(VisualElement parentElement) : base(parentElement)
    {

        SetUpButtons();
        RegisterCallbacks();
    }

    private void SetUpButtons()
    {

        m_UnreadyButton = m_RootElement.Q<Button>("unready-button");
    }
    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_UnreadyButton, evt => UIScreenEvents.Unready?.Invoke());
    }
}


