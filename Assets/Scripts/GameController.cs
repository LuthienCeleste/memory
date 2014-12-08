using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public GameObject cardPrefab;
	public GameObject wallpaperPrefab;
    public GameObject gameOverPrefab_en;
    public GameObject gameOverPrefab_es;
    public float minGameOverTime;

    private G g;

	private GameObject quitDialog;

    private bool isQuitScreenShowing;

    private bool isGameOver;
    private float remainingMinGameOverTime;

    private Lang lang;

    // Use this for initialization
    void Start ()
    {
        g = GameObject.Find ("G").GetComponent<G> ();
        quitDialog = GameObject.Find ("QuitDialog");
        quitDialog.gameObject.SetActive (false);
        SetupLevel ();
        isQuitScreenShowing = false;
        isGameOver = false;
        lang = GameObject.Find ("Lang").GetComponent<Lang> ();
    }
	
    // Update is called once per frame
    void Update ()
    {
        CheckEscape ();
        if (isGameOver) {
            remainingMinGameOverTime -= Time.deltaTime;
            if ( Input.GetButtonUp("Fire1") && remainingMinGameOverTime <= 0.0f )
            {
                if ( g.isKidModeActive() )
                {
                    g.ResetLevel();
                    Application.LoadLevel("GameScene");
                }
                else
                    Application.LoadLevel ("TittleScene");
            }
        }
    }

    public bool IsQuitScreenShowing()
    {
        return isQuitScreenShowing;
    }

    private void SetupLevel ()
    {
        SetupScreenSize ();
		SetupWallpaper ();
		SetupNewBoard ();
    }

    private float screenHeight;
    private float screenWidth;
	private float gameWidth;
	private float gameHeight;
	private Camera mainCamera;
    private Vector3 cardScale;
    private Vector3 boardOrigin;
	private float cardSizeX;
	private float cardSizeY;
	private float cardStepX;
	private float cardStepY;
	
	private void SetupScreenSize ()
    {
        mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
        screenHeight = mainCamera.orthographicSize * 2f;
        screenWidth = mainCamera.aspect * mainCamera.orthographicSize * 2f;

		int tilesX;
		int tilesY;
		g.GetLevelSize ( out tilesX, out tilesY);

		float targetRatio =  tilesX / ( 1.5f * tilesY );

        if (screenWidth / screenHeight < targetRatio ) {
            // Podemos usar todo el ancho
            gameWidth = screenWidth;
			gameHeight = gameWidth / targetRatio;
        } else {
			// Use all height
			gameHeight = screenHeight;
            gameWidth = screenHeight * targetRatio;
        }
		Transform cardFront = cardPrefab.transform.Find ("Front") as Transform;
		cardSizeX = cardFront.renderer.bounds.size.x;
		cardSizeY = cardFront.renderer.bounds.size.y;
		Debug.Log ( "Game " + gameWidth + "," + gameHeight + " card " + cardSizeX + "," +
		           cardSizeY + " tiles " + tilesX + "," + tilesY );
		cardScale = new Vector3 (gameWidth / cardSizeX / tilesX, gameHeight / cardSizeY / tilesY, 0f);
		cardStepX = gameWidth  / tilesX;
		cardStepY = gameHeight / tilesY;

        boardOrigin = new Vector3 (- (tilesX - 1f) / 2f * cardStepX, 
		                           - (tilesY - 1f) / 2f * cardStepY );

    }

	private void SetupWallpaper()
	{
		float wallSizeX;
		float wallSizeY;
		wallSizeX = wallpaperPrefab.renderer.bounds.size.x;
		wallSizeY = wallpaperPrefab.renderer.bounds.size.y;
		float wallRatio = wallSizeX / wallSizeY;
		float screenRatio = screenWidth / screenHeight;
		Vector3 wallScale;
		if (screenRatio > wallRatio) 
		{
			// Use all wallpaper width and crop top and bottom
			wallScale = new Vector3( screenWidth/wallSizeX, screenWidth/wallSizeX, 1.0f);
		}
		else
		{
			// Use all wallpaper height and crop left and right
			wallScale = new Vector3( screenHeight/wallSizeY, screenHeight/wallSizeY, 1.0f );
		}
		GameObject wall = Instantiate (wallpaperPrefab, 
		                               new Vector3( 0.0f, 0.0f, 1.0f),
		                                Quaternion.identity) 
			as GameObject;
		wall.transform.localScale = wallScale;
        // Select random wallpaper
        SpriteRenderer wallRend = wall.GetComponent<SpriteRenderer> ();
        Sprite[] wallpapers = g.getWallpapers ();
        wallRend.sprite = wallpapers [Random.Range (0, wallpapers.Length)];
	}

    private void SetupNewBoard ()
    {
        List<int> typesUsed = new List<int> ();
        bool[,] usedPosition;

		int tilesX;
		int tilesY;
		g.GetLevelSize ( out tilesX, out tilesY);
		usedPosition = new bool[tilesX, tilesY];
        for (int i=0; i<tilesX; ++i) {
            for (int j=0; j<tilesY; ++j)
                usedPosition [i, j] = false;
        }

        if ( (tilesX*tilesY) % 2 != 0) {
            usedPosition [tilesX / 2, tilesY / 2] = true;
        }

		int backUsed = Random.Range (0, g.getCardBacks ().Length);

        int numPairs = tilesX * tilesY / 2;
        for (int i=0; i<numPairs; ++i) {
            int type = getNonUsedType (typesUsed);
            typesUsed.Add (type);
            int x1;
            int y1;
            getNonUsedPosition (usedPosition, out x1, out y1);
            usedPosition [x1, y1] = true;
            int x2;
            int y2;
            getNonUsedPosition (usedPosition, out x2, out y2);
            usedPosition [x2, y2] = true;
            GameObject tile1 = Instantiate (cardPrefab, 
                                           boardOrigin + Vector3.right * cardStepX * x1 + 
			                               Vector3.up * cardStepY * y1,
                                           Quaternion.identity) 
                                as GameObject;
			tile1.transform.localScale = cardScale;
			CardController cont1 = tile1.GetComponent<CardController> ();
            cont1.setData (type, x1, y1, this);
			Transform front1 = tile1.transform.Find("Front") as Transform;
			front1.GetComponent<SpriteRenderer>().sprite = g.getCardFronts()[ type ]; 
			Transform back1  = tile1.transform.Find ("Back") as Transform;
			back1 .GetComponent<SpriteRenderer>().sprite = g.getCardBacks() [backUsed];
			GameObject tile2 = Instantiate (cardPrefab, 
			                                boardOrigin + Vector3.right * cardStepX * x2 + 
			                                Vector3.up * cardStepY * y2,
			                                Quaternion.identity) 
				as GameObject;
			tile2.transform.localScale = cardScale;
			CardController cont2 = tile2.GetComponent<CardController> ();
			cont2.setData (type, x2, y2, this);
			Transform front2 = tile2.transform.Find("Front") as Transform;
			front2.GetComponent<SpriteRenderer>().sprite = g.getCardFronts()[ type ]; 
			Transform back2  = tile2.transform.Find ("Back") as Transform;
			back2 .GetComponent<SpriteRenderer>().sprite = g.getCardBacks() [backUsed];

            activeTiles = tilesX*tilesY;
            if ( activeTiles%2!=0 )
                --activeTiles;
        }

        selectedTile1 = null;
        selectedTile2 = null;
    }

    private Color RandomColor ()
    {
        Color res = new Color (Random.Range (.4f, .6f),
                               Random.Range (.4f, .6f),
                               Random.Range (.7f, .6f),
                               1f);
        switch (Random.Range (0, 3)) {
        case 0:
            res.r = Random.Range (.7f, 1f);
            break;
        case 1:
            res.g = Random.Range (.7f, 1f);
            break;
        case 2:
            res.b = Random.Range (.7f, 1f);
            break;
        }
        return res;
    }

    private int getNonUsedType (List<int> typesUsed)
    {
        int res;
        do {
            res = Random.Range (0, g.getCardFronts ().Length);
        } while ( typesUsed.Contains(res) );
        return res;
    }

    private void getNonUsedPosition (bool[,] usedPosition, out int x, out int y)
    {
		int tilesX;
		int tilesY;
		g.GetLevelSize ( out tilesX, out tilesY);
		do {
            x = Random.Range (0, tilesX);
            y = Random.Range (0, tilesY);
        } while ( usedPosition[x,y] );
    }

    private CardController selectedTile1;
    private CardController selectedTile2;
    private int activeTiles;

    public void CardClicked (CardController tile)
    {
        Debug.Log ("CardClicked ");
        if (selectedTile1 == null ) {
            if ( selectedTile2!=tile )
            {
                selectedTile1 = tile;
                tile.StartTurning();
            }
            return;
        }

        if (selectedTile2 != null) {
            // Ignore clicks while there are 2 selected tiles
            return;
        }

        if (selectedTile1 != tile) {
            selectedTile2 = tile;
            tile.StartTurning();
        }
    }

    public void CardTurningFinished()
    {
        if (selectedTile1 == null || selectedTile2 == null)
            return;
        if (selectedTile1.GetAnimationState () == CardAnimState.FRONT &&
            selectedTile2.GetAnimationState () == CardAnimState.FRONT) {
            Debug.Log("check equal tiles " + selectedTile1.GetCardType() + "," +
                      selectedTile2.GetCardType() );
            if ( selectedTile1.GetCardType()==selectedTile2.GetCardType() )
            {
                // Equal tiles
                selectedTile1.StartDestroy();
                selectedTile2.StartDestroy();
            }
            else
            {
                // Different tiles
                selectedTile1.StartUnturning();
                selectedTile2.StartUnturning();
            }
            selectedTile1 = null;
            selectedTile2 = null;
        }
    }

    public void CardDestroyFinished()
    {
        --activeTiles;
        CheckEndOfLevel ();
    }

    void CheckEndOfLevel()
    {
        if (activeTiles <= 0) {
            g.GetNextLevel ();
            if ( g.GetLevel()>g.GetMaxLevel() )
                GameOver();
            else
                Application.LoadLevel ("GameScene");
        }
    }

    private void DestroyTile (GameObject tile)
    {
        GameObject.Destroy (tile);
    }
	
    public void OnClickContinue()
    {
        isQuitScreenShowing = false;
        quitDialog.SetActive (false);
    }

    public void OnClickQuit()
    {
        Application.LoadLevel("TittleScene");
    }

    private void CheckEscape()
    {
        if (Input.GetKeyDown (KeyCode.Escape) && !g.isKidModeActive() ) {
            quitDialog.SetActive(true);
            isQuitScreenShowing = true;
        }
    }

    private void GameOver()
    {
        float sizeX;
        float sizeY;
        GameObject gameOverPrefab;
        string language = lang.GetLanguage ();
        if ( language.Equals( "en"  ) )
            gameOverPrefab = gameOverPrefab_en;
        else if ( language.Equals( "es" ) )
            gameOverPrefab = gameOverPrefab_es;
        else
            gameOverPrefab = gameOverPrefab_en;
        sizeX = gameOverPrefab.renderer.bounds.size.x;
        sizeY = gameOverPrefab.renderer.bounds.size.y;
        float gameOverRatio = sizeX / sizeY;
        float screenRatio = screenWidth / screenHeight;
        Vector3 scale;
        if (gameOverRatio > screenRatio) 
        {
            // Use width equal to screen width
            scale = new Vector3( screenWidth/sizeX, screenWidth/sizeX, 1.0f);
        }
        else
        {
            // Use height equl to screen height
            scale = new Vector3( screenHeight/sizeY, screenHeight/sizeY, 1.0f );
        }
        GameObject wall = Instantiate (gameOverPrefab, 
                                       new Vector3( 0.0f, 0.0f, -1.0f),
                                       Quaternion.identity) 
            as GameObject;
        wall.transform.localScale = scale;

        isGameOver = true;
        remainingMinGameOverTime = minGameOverTime;
    }

}
