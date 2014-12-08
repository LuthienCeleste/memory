using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Difficulty
{
    NoTime,
    Easy,
    Normal,
    Hard
}

public class OptionsController : MonoBehaviour {
    private G g;
    private Text[] textInitDif; // Array used [1..9]; [0] ignored
    private Text[] textMaxDif;  // Array used [1..9]; [0] ignored
    private Lang lang;

	// Use this for initialization
	void Start () {
        g = GameObject.Find ("G").GetComponent<G> ();
        textInitDif = new Text[10];
        textMaxDif = new Text[10];
        for (int n=1; n<=9; ++n) {
            textInitDif[n] = GameObject.Find ("TextI" + n).GetComponent<Text> ();
            textMaxDif [n] = GameObject.Find ("TextM" + n).GetComponent<Text> ();
        }
        UpdateDifColors ();

        lang = GameObject.Find ("Lang").GetComponent<Lang> ();
    }
	
	// Update is called once per frame
	void Update () {
	}

    /** Update de color in difficulty buttons */
    void UpdateDifColors()
    {
        int initLevel = g.GetInitLevel ();
        int maxLevel = g.GetMaxLevel ();
        for (int n=1; n<=9; ++n) {
            if ( n==initLevel )
                textInitDif[n].color = Color.green;
            else
                textInitDif[n].color = Color.white;
            if ( n==maxLevel )
                textMaxDif[n].color = Color.green;
            else
                textMaxDif[n].color = Color.white;
        }
    }

    public void OnButtonIniDif( int button )
    {
        g.SetInitLevel (button);
        UpdateDifColors ();
    }

    public void OnButtonMaxDif( int button )
    {
        g.SetMaxLevel (button);
        UpdateDifColors ();
    }

    public void Back()
    {
        Application.LoadLevel ("TittleScene");
    }

    public void Spanish()
    {
        lang.SetLanguage ("es");
    }

    public void English()
    {
        lang.SetLanguage ("en");
    }
}
