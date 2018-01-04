using UnityEngine;
using System.Collections;
using M1.Utilities;
using System;

[Serializable]
public class ExampleState03 : MonoBehaviour, IState
{
    public GameObject nextState;
    public CanvasGroup uiPanel;

    public void Execute()
    {
    }

    public IEnumerator iEnter()
    {
        FadeManager.FadeIn(uiPanel, 0.5f);

        Debug.Log(this.name + " Enter: " + Time.time);
        yield return new WaitForSeconds(0.5f);

        //Trigger automated test, if automation build
        if (GameManager.Instance.Is_Automation_Test_Build)
            StartCoroutine(iAutomatedTest());
    }

    public IEnumerator iExit()
    {
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
    /// <returns></returns>
    public IEnumerator iAutomatedTest()
    {
        yield return new WaitForSeconds(1f);
        ButtonDown(0);
    }
}
