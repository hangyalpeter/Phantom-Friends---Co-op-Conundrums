using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelSelectScreen : UIScreen
{
    Button m_Level1Button;
    Button m_Level2Button;
    Button m_BackButton;
    Label m_Level1Label;
    Label m_Level2Label;
    VisualElement[] m_StarsLevel1;
    VisualElement[] m_StarsLevel2;
    Sprite emptyStar;
    Sprite fullStar;

    public LevelSelectScreen(VisualElement parentElement) : base(parentElement)
    {
        SetupControls();
        SubscribeToEvents();
        RegisterCallbacks();

        emptyStar = Resources.Load<Sprite>("GUI_25");
        fullStar = Resources.Load<Sprite>("GUI_24");

        SetUpBestTimes();
        SetupStars();

    }

    private void SetUpBestTimes()
    {
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

    private void SetupStars()
    {

        if (PlayerPrefs.HasKey(m_Level1Label.text.Split(" ")[0]))
        {
            var stars1 = PlayerPrefs.GetInt(m_Level1Label.text.Split(" ")[0]);

            for (int i = 0; i < stars1; i++)
            {
                m_StarsLevel1[i].style.backgroundImage = new StyleBackground(fullStar);
            }
        }

        if (PlayerPrefs.HasKey(m_Level2Label.text.Split(" ")[0]))
        {
            var stars2 = PlayerPrefs.GetInt(m_Level2Label.text.Split(" ")[0]);

            for (int i = 0; i < stars2; i++)
            {
                m_StarsLevel2[i].style.backgroundImage = new StyleBackground(fullStar);
            }
        }
    }

    private void UpdateStars(string key, int stars)
    {
        if (PlayerPrefs.HasKey(key))
        {
            for (int i = 0; i < stars; i++)
            {
                m_StarsLevel1[i].style.backgroundImage = new StyleBackground(fullStar);
            }
        }
    }

    public override void Disable()
    {
        base.Disable();
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameEvents.StarsChanged += UpdateStars;
        GameEvents.BestTimesChanged += SetUpBestTimes;
    }

    private void UnsubscribeFromEvents()
    {
        GameEvents.StarsChanged -= UpdateStars;
        GameEvents.BestTimesChanged -= SetUpBestTimes;
    }

    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_Level1Button, evt => UIScreenEvents.OnHostReady?.Invoke(Scene.Level_1));

        m_EventRegistry.RegisterCallback<ClickEvent>(m_Level2Button, evt => UIScreenEvents.OnHostReady?.Invoke(Scene.Level_2));

        m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => UIScreenEvents.ScreenClosed?.Invoke());
    }

    private void SetupControls()
    {
        m_Level1Button = m_RootElement.Q<Button>("level1-button");
        m_Level2Button = m_RootElement.Q<Button>("level2-button");
        m_BackButton = m_RootElement.Q<Button>("back-button");
        m_Level1Label = m_RootElement.Q<Label>("level1-label");
        m_Level2Label = m_RootElement.Q<Label>("level2-label");
        m_StarsLevel1 = m_RootElement.Q<VisualElement>("stars-level-1").Children().ToArray();
        m_StarsLevel2 = m_RootElement.Q<VisualElement>("stars-level-2").Children().ToArray();
    }

   
}
