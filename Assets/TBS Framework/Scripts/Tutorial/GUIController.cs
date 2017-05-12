using UnityEngine;

public class GUIController : MonoBehaviour
{
    public CellGrid CellGrid;
	
    void Start()
    {
        Debug.Log("Press 'n' to end turn");
    }

	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            CellGrid.EndTurn();//User ends his turn by pressing "n" on keyboard.
        }
	}

    /// <summary>
    /// Use this so it can be called from a UI button.
    /// </summary>
    public void EndTurn()
    {
        CellGrid.EndTurn();
    }
}
