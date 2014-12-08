using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour {
    private G g;

	// Use this for initialization
	void Start () {
        g = GameObject.Find ("G").GetComponent<G> ();
	}
	
    public void Credits()
    {
        Application.LoadLevel ("CreditsScene");
    }

    public void Play()
    {
        g.ResetLevel ();
        Application.LoadLevel ("GameScene");    
    }

    public void Options()
    {
        Application.LoadLevel ("OptionsScene");
    }

    public void Exit()
    {
        Application.Quit ();
    }

    public void KidMode()
    {
        Application.LoadLevel ("KidModeScene");
    }
}
