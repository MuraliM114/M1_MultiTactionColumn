using UnityEngine;

public class ORTimer : MonoBehaviour
{
    public static ORTimer Execute(GameObject target, float duration, string message)
    {
        GameObject go = new GameObject("ORTimer");
        ORTimer timer = go.AddComponent<ORTimer>();
        timer.target = target;
        timer.duration = duration;
        timer.message = message;

        go.transform.parent = target.transform;

        return timer;
    }

    public GameObject target;
    public float duration;
    public string message;

    private float startTimer;

    private void OnEnable()
    {
        startTimer = Time.time;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.time - startTimer >= duration)
        {
            if(target != null && target.gameObject != null)
                target.SendMessage(message, this, SendMessageOptions.DontRequireReceiver);

            Destroy(this.gameObject);
        }
	}
}
