using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// We will use this to control the display panel at the top of the "Battle Scene". 
/// For now it will just display selected unit stats and stuff. 
/// We should probably add an event for clicking on an enemy unit so that we may view its stats. 
/// </summary>
public class InfoPanelController : MonoBehaviour {

    /// <summary>
    /// You need this to get events from the CellGrid. 
    /// </summary>
    public CellGrid CellGrid;

    /// <summary>
    /// You need this to get events from the units.
    /// </summary>
    public GameObject UnitsParent;

    /// <summary>
    /// This is pretty cool because it makes this button non-interactable when it isn't the player's turn. 
    /// </summary>
    public Button NextTurnButton;

    /// <summary>
    /// Use this just to set the panel active or unactive. 
    /// </summary>
    public GameObject InfoPanel;

    //public GameObject GameOverPanel;          // Not needed yet because we don't have such a thing. 
    //public Canvas Canvas;                     // Not needed because the panel will always stay attached to its canvas and doesn't need to be reattached. 

    //private GameObject _infoPanel;
    //private GameObject _gameOverPanel;

    /// <summary>
    /// The character's name to display. 
    /// </summary>
    public Text characterName;
    /// <summary>
    /// The unit's HP to display. 
    /// </summary>
    public Text hp;
    /// <summary>
    /// The unit's attack to display. 
    /// </summary>
    public Text attack;
    /// <summary>
    /// The unit's defense to display. 
    /// </summary>
    public Text defense;
    /// <summary>
    /// Additional information to display, such as "game over" or whose turn it is. 
    /// </summary>
    public Text information;

    void Start()
    {
        // Assign our functions to the CellGrid's existing event handlers. 
        // They will then call our functions when needed.  
        CellGrid.GameStarted += OnGameStarted;
        CellGrid.TurnEnded += OnTurnEnded;
        CellGrid.GameEnded += OnGameEnded;

        // Turn the panel off to start.
        InfoPanel.SetActive(false);
    }

    /// <summary>
    /// This is a cool event-based system that basically causes this function to start listening  
    /// to all of the units on the screen when the game starts. 
    /// It is actually fascinating and simple (to look at). 
    /// </summary>
    private void OnGameStarted(object sender, EventArgs e)
    {
        foreach (Transform unit in UnitsParent.transform)
        {
            unit.GetComponent<Unit>().UnitHighlighted += OnUnitHighlighted;
            unit.GetComponent<Unit>().UnitDehighlighted += OnUnitDehighlighted;
            unit.GetComponent<Unit>().UnitDestroyed += OnUnitDestroyed;
            unit.GetComponent<Unit>().UnitAttacked += OnUnitAttacked;
        }
    }

    /// <summary>
    /// This looks like it sets the "NextTurnButton" to an active state only if the current player is a human. 
    /// </summary>
    /// <param name="sender">This is the CellGrid that sent the event. We can ask it whether the current player is a human player. </param>
    /// <param name="e">I don't think EventArgs are passed in this event. </param>
    private void OnTurnEnded(object sender, EventArgs e)
    {
        NextTurnButton.interactable = ((sender as CellGrid).CurrentPlayer is HumanPlayer);
    }


    private void OnGameEnded(object sender, EventArgs e)
    {
        //_gameOverPanel = Instantiate(GameOverPanel);
        //_gameOverPanel.transform.Find("InfoText").GetComponent<Text>().text = "Player " + ((sender as CellGrid).CurrentPlayerNumber + 1) + "\nwins!";

        //_gameOverPanel.transform.Find("DismissButton").GetComponent<Button>().onClick.AddListener(DismissPanel);

        //_gameOverPanel.GetComponent<RectTransform>().SetParent(Canvas.GetComponent<RectTransform>(), false);

    }

    /// <summary>
    /// When any unit is attacked, it will try to run these methods:
    /// 1. Don't do anything whatsoever if the player attacking isn't the human player. Ok. 
    /// 2. Try to run "OnUnitDehighlighted" otherwise. 
    /// 3. If the attacked unit is dead, don't do anything more. 
    /// 4. Try to run "OnUnitHighlighted" otherwise. 
    /// </summary>
    /// <param name="sender">The unit that is being attacked (I think). </param>
    /// <param name="e">Information about the attack. </param>
    private void OnUnitAttacked(object sender, AttackEventArgs e)
    {
        Debug.Log("[InfoPanelController] OnUnitAttacked: " + sender.ToString() + ", AttackEventArgs - Attacker: " + e.Attacker + " Defender: " + e.Defender + " Damage: " + e.Damage);

        if (!(CellGrid.CurrentPlayer is HumanPlayer)) return;

        OnUnitDehighlighted(sender, e);

        if ((sender as Unit).HitPoints <= 0) return;

        OnUnitHighlighted(sender, e);
    }

    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        InfoPanel.SetActive(false);
    }

    private void OnUnitDehighlighted(object sender, EventArgs e)
    {
        InfoPanel.SetActive(false);
    }

    /// <summary>
    /// Note that this can be used for enemy or friendly units! 
    /// Unfortunately it probably won't be usable for mobile. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnUnitHighlighted(object sender, EventArgs e)
    {
        // Get the unit that sent its information to us. 
        var unit = sender as GenericUnit;

        // Turn on the panel to show the information. 
        InfoPanel.SetActive(true);

        // Assign various properties to the panel. 
        characterName.text = unit.UnitName;
        hp.text = unit.HitPoints.ToString() + "/" + unit.TotalHitPoints.ToString();
        attack.text = unit.AttackFactor.ToString();
        defense.text = unit.DefenceFactor.ToString();

        // Do something with the "extra info" text. 
        // Note that this will always show the actions of the target that's being hovered over, and not the attacker. This makes it a bit awkward to use. 
        information.text = "Actions remaining: " + unit.ActionPoints.ToString();
    }

    public void DismissPanel()
    {
        InfoPanel.SetActive(false);
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene("Battle Scene");
    }
}
