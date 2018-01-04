using System.Collections;
//using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.SceneManagement;

//For upper right, triple tap reset
//To implement, create a UI image and place in upper left corner
//Add event trigger and hook in "OnTouch()"
//Can also be used with TouchScript, to do so- uncomment commented lines
//Example object in prefabs folder
//-Andrew
public class RESET_APP : MonoBehaviour
{
    private int touchIndex;
    //public TapGesture tap;

    void Start()
    {
        //tap.Tapped += Tap_Tapped;
        touchIndex = 0;
    }

    //private void Tap_Tapped(object sender, System.EventArgs e)
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //}

    public void OnTouch()
    {
        touchIndex++;
        Debug.Log("Touch!");
        if (touchIndex >= 3)
        {
            //Put any other reset functionality here
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        StopCoroutine("Countdown");
        StartCoroutine("Countdown");
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1.5f);

        touchIndex = 0;
        Debug.Log("Touch Index Reset!");
    }
}
