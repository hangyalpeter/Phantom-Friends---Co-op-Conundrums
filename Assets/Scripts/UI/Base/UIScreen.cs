using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

// Copied over (and modified a little bit) from the official UQuiz Unity tutorial project
// https://assetstore.unity.com/packages/essentials/tutorial-projects/quizu-a-ui-toolkit-sample-268492
// downloaded from the Unity Asset Store on 2024-02-19
public abstract class UIScreen
{
    public const string k_VisibleClass = "screen-visible";
    public const string k_HiddenClass = "screen-hidden";

    protected bool m_HideOnAwake = true;

    // Is the UI partially see-through? (i.e. use overlay effect)
    protected bool m_IsTransparent;

    // Use USS style classes to transition to the next View
    protected bool m_UseTransition = true;
    protected float m_TransitionDelay = 0.1f;

    // The topmost visual element of the screen (this often is the rootVisualElement
    // but can be a different element if necessary)
    protected VisualElement m_RootElement;
    protected EventRegistry m_EventRegistry;

    // Used by helper class to hide/show with a delay
    protected Coroutine m_DisplayRoutine;

    public VisualElement ParentElement => m_RootElement;

    public bool IsTransparent => m_IsTransparent;
    public bool IsHidden => m_RootElement.style.display == DisplayStyle.None;

    // Constructor
    public UIScreen(VisualElement parentElement)
    {
        // Required topmost VisualElement 
        m_RootElement = parentElement ?? throw new ArgumentNullException(nameof(parentElement));
        Initialize();
    }

    // Registers a callback, TransitionEndEvent, on the m_ParentElement; creates a default EventRegistry
    public virtual void Initialize()
    {
        if (m_HideOnAwake)
        {
            HideImmediately();
        }

        m_EventRegistry = new EventRegistry();
        m_EventRegistry.RegisterCallback<TransitionEndEvent>(m_RootElement, ParentElement_TransitionEnd);
    }

    // Unregister events from an external objects
    public virtual void Disable()
    {
        m_EventRegistry.Dispose();
    }

    // Event-handling method

    // If the m_ParentElement is fading off, hide it once the USS transition is complete
    private void ParentElement_TransitionEnd(TransitionEndEvent evt)
    {
        if (evt.target == m_RootElement && m_RootElement.ClassListContains(k_HiddenClass))
        {
            HideImmediately();
        }
    }

    // Show and use the transition, if enabled
    public virtual void Show()
    {
        // Use helper class to run coroutines
        Coroutines.StopCoroutine(ref m_DisplayRoutine);
        m_DisplayRoutine = Coroutines.StartCoroutine(ShowWithDelay(m_TransitionDelay));
    }

    // Show with a variable second delay
    private IEnumerator ShowWithDelay(float delayInSecs)
    {
        yield return new WaitForSeconds(delayInSecs);

        m_RootElement.style.display = DisplayStyle.Flex;

        if (m_UseTransition)
        {
            m_RootElement.AddToClassList(k_VisibleClass); // Add visible class
            m_RootElement.BringToFront();
            m_RootElement.RemoveFromClassList(k_HiddenClass); // Remove hidden class
        }
    }

    // Hide and use transition, if enabled
    public virtual void Hide(float delay = 0f)
    {
        // Use helper class to run coroutines
        Coroutines.StopCoroutine(ref m_DisplayRoutine);

        m_DisplayRoutine = Coroutines.StartCoroutine(HideWithDelay(delay));
    }

    // Hide with a variabel second delay 
    private IEnumerator HideWithDelay(float delayInSecs)
    {
        yield return new WaitForSeconds(delayInSecs);

        if (m_UseTransition)
        {
            m_RootElement.AddToClassList(k_HiddenClass); // Add hidden class
            m_RootElement.RemoveFromClassList(k_VisibleClass); // Remove visible class
        }
        else
        {
            HideImmediately();
        }
    }

    // Hide without a transition
    public void HideImmediately()
    {
        m_RootElement.style.display = DisplayStyle.None;
    }

    public void ShowImmediately()
    {
        m_RootElement.style.display = DisplayStyle.Flex;
    }
}
