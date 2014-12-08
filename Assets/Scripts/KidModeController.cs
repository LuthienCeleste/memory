using UnityEngine;
using System.Collections;

public class KidModeController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void Continue () {
        G g = GameObject.Find ("G").GetComponent<G> ();
        g.ActivateKidMode ();
        g.ResetLevel ();
        Application.LoadLevel ("GameScene");
	}

    public void Cancel()
    {
        Application.LoadLevel ("TittleScene");
    }
}
