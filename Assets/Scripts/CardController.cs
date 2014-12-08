using UnityEngine;
using System.Collections;

public enum CardAnimState
{
    BACK,
    TURNING,
    FRONT,
    UNTURNING,
    DESTROYING,
    DESTROYED,
    PAUSE_DISPLAY,
}

public class CardController : MonoBehaviour
{
    public float rotationTime;
    public float unrotationTime;
    public float destroyTime;
    public int destroyTurns;
    public float pauseDisplayTime;

    private int type;
    private int boardX;
    private int boardY;
    private GameController controller;
    private Transform transformBack;
    private Transform transformFront;
    private SpriteRenderer rendererBack;
    private SpriteRenderer rendererFront;
    private float remainingTurnTime;
    private int remainingTurns;
    CardAnimState animState;

    // Use this for initialization
    void Start ()
    {
        transformBack = gameObject.transform.Find ("Back");
        transformFront = gameObject.transform.Find ("Front");
        animState = CardAnimState.BACK;
        rendererBack  = transformBack.GetComponent<SpriteRenderer> ();
        rendererFront = transformFront.GetComponent<SpriteRenderer> ();
        rendererBack.enabled = true;
        rendererFront.enabled = false;
    }
    
    // Update is called once per frame
    void Update ()
    {
        collider2D.enabled = !controller.IsQuitScreenShowing ();
        if (animState == CardAnimState.TURNING)
            AnimateTurning ();
        else if (animState == CardAnimState.UNTURNING)
            AnimateUnturning ();
        else if (animState == CardAnimState.DESTROYING)
            AnimateDestroying ();
        else if (animState == CardAnimState.PAUSE_DISPLAY)
            AnimatePauseDisplay ();
    }

    void AnimatePauseDisplay()
    {
        remainingTurnTime -= Time.deltaTime;
        if (remainingTurnTime <= 0.0f) {
            animState = CardAnimState.FRONT;
            controller.CardTurningFinished ();
        }
    }

    void AnimateTurning()
    {
        remainingTurnTime -= Time.deltaTime;
        if (remainingTurnTime <= 0.0f) {
            animState = CardAnimState.PAUSE_DISPLAY;
            remainingTurnTime = pauseDisplayTime;
            transform.rotation = Quaternion.Euler( 0.0f, 180.0f, 0.0f );
            rendererFront.enabled = true;
            rendererBack.enabled = false;
        }
        else
        {
            float lerpParm = 1.0f - remainingTurnTime / rotationTime;
            if ( lerpParm>.5f && !rendererFront.enabled )
            {
                rendererFront.enabled = true;
                rendererBack.enabled = false;
            }
            transform.localRotation = Quaternion.Euler (
                0.0f, 
                Mathf.Lerp (0.0f, 180.0f, lerpParm),
                0.0f);
        }
    }
    
    void AnimateUnturning()
    {
        remainingTurnTime -= Time.deltaTime;
        if (remainingTurnTime <= 0.0f) {
            animState = CardAnimState.BACK;
            transform.rotation = Quaternion.Euler( 0.0f, 0.0f, 0.0f );
            rendererFront.enabled = false;
            rendererBack.enabled = true;
        }
        else
        {
            float lerpParm = 1.0f - remainingTurnTime / unrotationTime;
            if ( lerpParm>.5f && !rendererBack.enabled )
            {
                rendererFront.enabled = false;
                rendererBack.enabled = true;
            }
            transform.localRotation = Quaternion.Euler (
                0.0f, 
                Mathf.Lerp (180.0f, 0.0f, lerpParm),
                0.0f);
        }
    }
    
    void AnimateDestroying()
    {
        remainingTurnTime -= Time.deltaTime;
        while (remainingTurnTime<=0.0f && remainingTurns>0) {
            remainingTurnTime += destroyTime;
            --remainingTurns;
        }
        if (remainingTurnTime <= 0.0f && remainingTurns<=0 ) {
            animState = CardAnimState.DESTROYED;
            controller.CardDestroyFinished();
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            float lerpParm = 1.0f - remainingTurnTime / destroyTime;
            if ( lerpParm<.5f)
            {
                if ( !rendererFront.enabled )
                {
                    rendererFront.enabled = true;
                    rendererBack.enabled = false;
                }
            }
            else
            {
                if ( !rendererBack.enabled )
                {
                    rendererFront.enabled = false;
                    rendererBack.enabled = true;
                }
            }
            transform.localRotation = Quaternion.Euler (
                0.0f, 
                Mathf.Lerp (180.0f, 540, lerpParm),
                0.0f);
        }
    }
    
    public void StartTurning()
    {
        if ( animState!=CardAnimState.BACK )
            Debug.LogError("StartTurning when animState!=CardAnimState.BACK");
        remainingTurnTime = rotationTime;
        animState = CardAnimState.TURNING;
    }
    
    public void StartUnturning()
    {
        if ( animState!=CardAnimState.FRONT )
            Debug.LogError("StartUnturning when animState!=CardAnimState.FRONT");
        remainingTurnTime = unrotationTime;
        animState = CardAnimState.UNTURNING;
    }

    public void StartDestroy()
    {
        if (animState != CardAnimState.FRONT)
            Debug.LogError ("StartDestroy when animState!=CardAnimState.FRONT");
        remainingTurnTime = destroyTime;
        remainingTurns = destroyTurns;
        animState = CardAnimState.DESTROYING;
    }
    
    void OnMouseUpAsButton ()
    {
        Debug.Log ("OnMouseUpAsButton " + boardX + "," + boardY);
        if ( animState==CardAnimState.BACK )
            controller.CardClicked (this);
    }
    
    public void setData (int type, int boardX, int boardY, GameController controller)
    {
        this.type = type;
        this.boardX = boardX;
        this.boardY = boardY;
        this.controller = controller;
    }
    
    public int GetCardType ()
    {
        return type;
    }
    
    public int GetBoardX ()
    {
        return boardX;
    }
    
    public int GetBoardY ()
    {
        return boardY;
    }

    public CardAnimState GetAnimationState()
    {
        return animState;
    }
}
