using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For running things on the main thread, (i.e. functions not in a monobehaviour)
//Example - DoOnMainThread.ExecuteOnMainThread.Enqueue(() => { DoStuff(); })
public class DoOnMainThread : MonoBehaviour
{

    public readonly static Queue<Action> ExecuteOnMainThread = new Queue<Action>();

    public virtual void Update()
    {
        // dispatch stuff on main thread
        while (ExecuteOnMainThread.Count > 0)
        {
            ExecuteOnMainThread.Dequeue().Invoke();
        }
    }
}
