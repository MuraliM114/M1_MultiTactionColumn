using UnityEngine;
using System.Collections;

//For using Unity's multi display support
//To use, attach this script to GameManager, will do what it needs on Start()
//Added 'screenActivated' bool for reloading scene, if bool isn't used, resetting app won't reset
//Second monitor, Also note that touch doesn't work on any display besides Display 1 as of 5/17/2017
//-Andrew
public class MultiDisplay : MonoBehaviour
{
    public static bool screenActivated = false;

    void Start()
    {
        //Activate second monitor
        if (Display.displays.Length > 1)
        {
            if (!screenActivated)
            {
                Display.displays[1].Activate();
                screenActivated = true;
            }
        }
    }
}
