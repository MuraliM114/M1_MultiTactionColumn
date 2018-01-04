using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M1.Utilities;

public class GameManager : SingletonBehaviour<GameManager>
{
    public GameObject startState;
    public bool Is_Automation_Test_Build = false;
    
    IEnumerator Start()
    {
        yield return null;
        GoToStartState();
    }
    
    public static void NextState()
    {
        StateMachine.NextState();
    }

    public static void GoToStartState()
    {
        StateMachine.ChangeState(Instance.startState.GetComponent<IState>());
    }
}
