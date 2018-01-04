using UnityEngine;
using System.Collections;
using M1.Utilities;
using System;

[Serializable]
public class ExampleState01 : MonoBehaviour, IState
{
    public GameObject nextState;
    public CanvasGroup uiPanel;

    /// <summary>
    /// Update loop equivalent to unity's "Update()", executed only when state is active.
    /// </summary>
    public void Execute()
    {
    }

    /// <summary>
    /// Triggered after previous state's iExit() completes. Use for transitions into state.
    /// (Resets, animations, fading, etc.)
    /// </summary>
    public IEnumerator iEnter()
    {
        //Transition in calls go here (Animations, fades, etc)//

        FadeManager.FadeIn(uiPanel, 0.5f);  //Fade this state's UI elements in



        ////////////////////////////////////////////////////////

        Debug.Log(this.name + " Enter: " + Time.time);
        yield return new WaitForSeconds(0.5f);

        //Trigger automated test, if automation build
        if (GameManager.Instance.Is_Automation_Test_Build)
            StartCoroutine(iAutomatedTest());
    }

    /// <summary>
    /// Triggered after GameManager.NextState() is called. Use for transitions out of state.
    /// (Resets, animations, fading, etc.)
    /// </summary>
    public IEnumerator iExit()
    {
        //Fade this state's UI elements out
        FadeManager.FadeOut(uiPanel, 0.5f, true);

        Debug.Log(this.name + " Exit: " + Time.time);
        yield return new WaitForSeconds(0.5f);
    }

    public void ButtonDown(int _num)
    {
        Debug.Log(_num + " ButtonDown: " + Time.time);
        GameManager.NextState();
    }

    public void ButtonUp(int _num)
    {
        Debug.Log(_num + " ButtonUp: " + Time.time);
    }

    public IState GetNextState()
    {
        return nextState.GetComponent<IState>();
    }

    /// <summary>
    /// For test automation builds, this function should contain what you want to test
    /// in this state and functionality that needs to be triggered to advance automatically.
    /// (ie. button event calls, animations, etc)
    /// 
    /// This should be called after the state is done transitioning in iEnter()
    /// </summary>
    public IEnumerator iAutomatedTest()
    {
        yield return new WaitForSeconds(1f);
        ButtonDown(0);
    }
}
