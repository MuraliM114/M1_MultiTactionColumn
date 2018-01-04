using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
	IEnumerator Start ()
    {
        Application.runInBackground = true;
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
