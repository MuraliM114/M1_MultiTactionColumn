using System.Collections;
using UnityEngine;

namespace M1.Utilities
{
    public interface IState
    {
        IEnumerator iEnter();
        IEnumerator iExit();
        IEnumerator iAutomatedTest();

        IState GetNextState();
        void Execute();

        void ButtonDown(int _num);
        void ButtonUp(int _num);
    }
}