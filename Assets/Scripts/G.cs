using UnityEngine;
using System.Collections;
using System;

public class G : MonoBehaviour {
    private int initLevel;
    private int maxLevel;
	private int level;
	private Sprite[] cardFronts;
	private Sprite[] cardBacks;
	private Sprite[] wallpapers;
    private bool kidModeActive;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this);
        LoadLevelConfig ();
		cardFronts = Resources.LoadAll<Sprite> ("CardFronts");
		Debug.Log ("Loaded " + cardFronts.Length + " CardFronts");
		cardBacks = Resources.LoadAll<Sprite> ("CardBacks");
		Debug.Log ("Loaded " + cardBacks.Length + " CardBacks");
		wallpapers = Resources.LoadAll<Sprite> ("Wallpapers");
		Debug.Log ("Loaded " + wallpapers.Length + " Wallpapers");
        kidModeActive = false;
	}

    public void ActivateKidMode()
    {
        kidModeActive = true;
    }

    public bool isKidModeActive()
    {
        return kidModeActive;
    }

	public Sprite[] getCardBacks()
	{
		return cardBacks;
	}

	public Sprite[] getCardFronts()
	{
		return cardFronts;
	}

	public Sprite[] getWallpapers()
	{
				return wallpapers;
	}

	public void SetInitLevel(int level)
	{
        if (level <= maxLevel) {
            initLevel = level;
            SaveLevelConfig ();
        }
	}

    public void SetMaxLevel( int level )
    {
        if (level >= initLevel) {
            maxLevel = level;
            SaveLevelConfig ();
        }
    }

    public void ResetLevel()
    {
        level = initLevel;
    }

    public int GetInitLevel()
    {
        return initLevel;
    }

    public int GetMaxLevel()
    {
        return maxLevel;
    }

	public void GetLevelSize( out int sizeX, out int sizeY )
	{
		sizeX = 2 + level/2;
		sizeY = 2 + (level-1)/2;
	}

    public int GetNextLevel()
    {
        return ++level;
    }

    public int GetLevel()
    {
        return level;
    }

    public void SaveLevelConfig()
    {
        PlayerPrefs.SetInt ("InitLevel", initLevel );
        PlayerPrefs.SetInt ("MaxLevel", maxLevel );
        PlayerPrefs.Save ();    
    }

    void LoadLevelConfig()
    {
        if (PlayerPrefs.HasKey ("InitLevel"))
            initLevel = PlayerPrefs.GetInt ("InitLevel");
        else
            initLevel = 2;
        if (PlayerPrefs.HasKey ("MaxLevel"))
            maxLevel = PlayerPrefs.GetInt ("MaxLevel");
        else
            maxLevel = 9;
    }
}
