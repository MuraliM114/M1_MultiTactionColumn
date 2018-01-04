using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M1.Utilities
{
    public class StateMachine : SingletonBehaviour<StateMachine>
    {
        private static IState currentState;
        private static Coroutine currentCoroutine;
        private static List<IState> previousStateList = new List<IState>();
        private const int maxPreviousStates = 100;
        private static bool changingState = false;

        void OnDisable()
        {
            currentState = null;
            changingState = false;
            previousStateList = null;
        }

        void OnEnable()
        {
            if (previousStateList == null)
            {
                previousStateList = new List<IState>();
            }
        }

        void Update()
        {
            if (!changingState)
            {
                if (currentState != null)
                {
                    currentState.Execute();
                }
            }
        }

        public static bool ChangeState(IState _nextState)
        {
            if (!changingState)
            {
                Instance.StartCoroutine(Instance.iChangeState(_nextState));
                return true;
            }
            else
            {
                return false;
            }

        }

        public static void NextState()
        {
            ChangeState(currentState.GetNextState());
        }

        public static bool RevertState()
        {
            if (!changingState)
            {
                Instance.StartCoroutine(Instance.iRevertState());
                return true;
            }
            else
            {
                return false;
            }
        }

        IEnumerator iChangeState(IState _nextState)
        {
            changingState = true;

            if (currentState != _nextState)
            {

                if (currentState != null)
                {
                    currentCoroutine = StartCoroutine(currentState.iExit());
                    yield return currentCoroutine;
                }

                if (_nextState != null)
                {
                    currentCoroutine = StartCoroutine(_nextState.iEnter());
                    yield return currentCoroutine;
                }

                if (currentState != null)
                {
                    previousStateList.Add(currentState);
                }

                // remove extra states
                if (previousStateList.Count >= maxPreviousStates)
                {
                    previousStateList.RemoveRange(0, previousStateList.Count - maxPreviousStates);
                }
                currentState = _nextState;
            }

            changingState = false;
        }

        IEnumerator iRevertState()
        {
            if (previousStateList.Count > 0)
            {
                changingState = true;
                IState previousState = previousStateList[previousStateList.Count - 1];
                if (previousState != null)
                {
                    previousStateList.RemoveAt(previousStateList.Count - 1);
                    if (previousState != null)
                    {
                        if (currentState != null)
                        {
                            currentCoroutine = StartCoroutine(currentState.iExit());
                            yield return currentCoroutine;
                        }

                        if (previousState != null)
                        {
                            currentCoroutine = StartCoroutine(previousState.iEnter());
                            yield return currentCoroutine;
                        }

                        currentState = previousState;
                    }
                }
                changingState = false;
            }
        }

        public static void SendButtonDown(int _num)
        {
            if (currentState != null && !changingState)
            {
                currentState.ButtonDown(_num);
            }
        }

        public static void SendButtonUp(int _num)
        {
            if (currentState != null && !changingState)
            {
                currentState.ButtonUp(_num);
            }
        }
    }
}