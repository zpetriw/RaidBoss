using UnityEngine;

/// <summary>
/// Use this as a singleton that we can use to create floating text for our scene. 
/// We can probably just keep it simple and create/destroy the objects without pooling them. 
/// </summary>
public sealed class FloatingTextController : MonoBehaviour {

    private static FloatingTextController instance;
    /// <summary>
    /// Returns the instance of this singleton. 
    /// This doesn't look like it checks to see if existing ones match. Do we need that functionality?
    /// </summary>
    public static FloatingTextController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (FloatingTextController)FindObjectOfType(typeof(FloatingTextController));

                if (instance == null)
                {
                    Debug.LogError("An instance of " + typeof(FloatingTextController) +
                       " is needed in the scene, but there is none.");
                }
                else
                {
                    instance.Init();
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// This will be a reference to the floating text prefab. 
    /// </summary>
    private GameObject popupText;

    /// <summary>
    /// This is a reference to the canvas on which we will be displaying the floating text. 
    /// </summary>
    public GameObject canvas;

    /// <summary>
    /// Use this to control how much jitter in x and y we'll apply to each floating text we create. 
    /// These are the bounds in both directions. I.e. X = 10 means it could be anywhere from -10 to +10. 
    /// </summary>
    public Vector2 jitter;

    /// <summary>
    /// If we set it up this way, this should be guaranteed to run at least once before 
    /// the contents "CreateFloatingText" are ever executed.
    /// </summary>
	private void Init() {

        // Get the prefab from the Resources folder. 
        popupText = Resources.Load<GameObject>("Prefabs/FloatingTextParent");
    }
	
    /// <summary>
    /// We can probably add some extra information like color, 
    /// or even supply the Text object itself. I don't know. 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="location"></param>
    /// <param name="beneficial">If set to true, text will take a green color. Otherwise, it becomes red.</param>
	public void CreateFloatingText(string text, Transform location, bool beneficial)
    {
        GameObject floatingText = Instantiate(popupText);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);


        floatingText.transform.SetParent(canvas.transform, false);
        
        // Apply random jitter, measured in pixels. 
        screenPosition.Set(screenPosition.x + Random.Range(-jitter.x, jitter.x), screenPosition.y + Random.Range(-jitter.y, jitter.y));

        floatingText.transform.position = screenPosition;

        //floatingText.transform.position.Set( = 5;

        floatingText.GetComponent<FloatingText>().SetText(text, beneficial);
    }
}
