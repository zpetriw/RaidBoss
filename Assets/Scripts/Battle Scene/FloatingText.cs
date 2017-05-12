using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Use this class to manage the floating text UI element in the Battle Scene. 
/// </summary>
public class FloatingText : MonoBehaviour {

    /// <summary>
    /// This is what has the animation for the floating text. 
    /// </summary>
    public Animator animator;

    /// <summary>
    /// This the text of how much damage occured or what effect was applied.
    /// We'll keep it generic for now. 
    /// </summary>
    public Text displayText;

    /// <summary>
    /// Use this to set a bad color for damage and debuffs.
    /// </summary>
    public Color negativeColor;

    /// <summary>
    /// Use this to set a good color for heals and buffs. 
    /// </summary>
    public Color positiveColor;

    /// <summary>
    /// Store a reference to the clip info. 
    /// </summary>
    private AnimatorClipInfo[] clipInfo;

	// Use this for initialization
	void Awake ()
    {

        // Set the positive and negative colors. 
        //negativeColor = new Color(1, 0, 0);
        //positiveColor = new Color(0, 1, 0);
        
        // Get the info of the one and only clip we have. 
        //clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        // Assign the text component based on the gameobject that the animator is attached to. 
        //displayText = animator.GetComponent<Text>();

        //Debug.Log("Finished initialization for FloatingText script.", gameObject);
	}

    /// <summary>
    /// Let's keep it simple and follow the video a little more closely. 
    /// https://www.youtube.com/watch?v=fbUOG7f3jq8&t=1248s
    /// </summary>
    void Start()
    {
        clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        // Destroy this object after the clip is done playing.
        // We could return it to an object pool if we wanted but I don't think this will save us much memory right now.
        Destroy(gameObject, clipInfo[0].clip.length);

        //displayText = animator.GetComponent<Text>();
    }

    //void OnEnable()
    //{
    //    // We want to disable it after a certain amount of time, since we want it to return to the object pool.
    //    StartCoroutine(DisableAfterTime(clipInfo[0].clip.length));
    //}

    /// <summary>
    /// Use this to set damage effect text to either a damage color or healing color. 
    /// </summary>
    /// <param name="text">The text to be displayed.</param>
    /// <param name="beneficial">
    /// Whether the effect was beneficial to the target or not. 
    /// This decides whether to show it in the positive or negative color.
    /// </param>
    public void SetText(string text, bool beneficial)
    {
        try
        {
            Debug.Log("Running SetText for this object.", gameObject);

            // Set the color of the text based on whether this was a buff or debuff, or damage or heal. 
            displayText.color = beneficial ? positiveColor : negativeColor;

            // Set the actual text of this animation. 
            displayText.text = text;
        }
        catch (System.Exception ex)
        {
            string msg = "[FloatingText] SetText Exception: " + ex.Message + "\nSource: " + ex.Source + "\nTargetSite: " + ex.TargetSite;
            Debug.LogError(msg, gameObject);
            //MessageDisplayManager.Instance.DisplayException(msg);
        }

    }


    /// <summary>
    /// Use this to disable the object after a set amount of time. 
    /// </summary>
    /// <returns></returns>
    //private IEnumerator DisableAfterTime(float seconds)
    //{
    //    yield return new WaitForSeconds(seconds);

    //    gameObject.SetActive(false);

    //    Debug.Log("Turned off floating text object after " + seconds.ToString() + " seconds.", gameObject);
    //}
}
