using SpriteParticleEmitter;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GenericUnit : Unit
{
    public string UnitName;

    private Coroutine PulseCoroutine;

    /// <summary>
    /// Adding this so that something can be jiggled when "animated". 
    /// We can change this to different animations obviously. 
    /// </summary>
    public SpriteRenderer imageToBeJerked;

    /// <summary>
    /// This are the images whose color we will adjust when the unit is done attacking or dead or something.
    /// For now it will include the frame and the health bar. 
    /// We will restore them when the unit is ready to attack again. 
    /// </summary>
    public SpriteRenderer[] imagesToBeColored;

    /// <summary>
    /// This can be all of the UI elements to color grey, such as the health bar or status effects. 
    /// </summary>
    public Image[] uISpritesToColor;

    /// <summary>
    /// This should hold a reference to the frame that will be swapped between a blue and a grey one. 
    /// </summary>
    public SpriteRenderer frameToBeSwapped;

    /// <summary>
    /// This will be the regular blue frame when the unit is available.
    /// </summary>
    public Sprite activeFrame;

    /// <summary>
    /// This will be the grey frame sprite swap when the unit has spend all its actions. 
    /// </summary>
    public Sprite inactiveFrame;

    /// <summary>
    /// Use this to turn a glow on and off when the unit is selected. 
    /// </summary>
    public StaticEmitterContinuous[] selectedEmitters;

    /// <summary>
    /// This is the bar that will fill between 0 and 1 to represent the unit's health. 
    /// We will keep the friendly bars green and enemy bars red. 
    /// </summary>
    public Image topHealthBar;

    /// <summary>
    /// A reference to the text UI object that stores a number of how much health the unit has. 
    /// </summary>
    public Text healthText;



    public override void Initialize()
    {
        base.Initialize();
        transform.position += new Vector3(0, 0, -0.1f);

        // Probably want to update the health bar right off the start. 
        UpdateHealthBar();
    }

    public override void OnUnitDeselected()
    {
        base.OnUnitDeselected();
        //StopCoroutine(PulseCoroutine);
        //transform.localScale = new Vector3(1,1,1);
        if (selectedEmitters != null)
            foreach (StaticEmitterContinuous item in selectedEmitters)
            {
                item.Stop();
            }
                
        //Debug.Log("[GenericUnit] OnUnitDeselected: " + UnitName, this);
    }

    public override void MarkAsAttacking(Unit other)
    {
        StartCoroutine(Jerk(other));
    }
    public override void MarkAsDefending(Unit other)
    {
        StartCoroutine(Glow(new Color(1, 0, 0, 0.5f), 1));
    }
    public override void MarkAsDestroyed()
    {
    }

    /// <summary>
    /// Here we can update our health bars. 
    /// </summary>
    /// <param name="other"></param>
    /// <param name="damage"></param>
    protected override void Defend(Unit other, int damage)
    {
        base.Defend(other, damage);
        UpdateHealthBar();
    }

    /// <summary>
    /// Potentially we can move this to another script if we need to. 
    /// For now it's small and can sit in here. 
    /// </summary>
    private void UpdateHealthBar()
    {
        if (topHealthBar != null)
        {
            // Figure out how much we want the topmost healthbar (green for friendlies, red for enemies) to fill. 
            topHealthBar.fillAmount = (float)((float)HitPoints / (float)TotalHitPoints);
            
            
            
            // Instead of sliding the green bar away to expose a red bar, we can just shrink the bar while simulataneously changing its color. 
            //healthBar.color = Color.Lerp(Color.red, Color.green, (float)((float)HitPoints / (float)TotalHitPoints));
        }

        // Fill out the health text overlay. 
        if (healthText != null)
        {
            healthText.text = HitPoints.ToString() + "/" + TotalHitPoints.ToString();
        }

    }

    private IEnumerator Jerk(Unit other)
    {
        //GetComponent<SpriteRenderer>().sortingOrder = 6;
        imageToBeJerked.sortingOrder = 6;   // Not sure why this is set. 

        var heading = other.transform.position - transform.position;
        var direction = heading / heading.magnitude;
        float startTime = Time.time;

        while (startTime + 0.25f > Time.time)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + (direction / 50f), ((startTime + 0.25f) - Time.time));
            yield return 0;
        }
        startTime = Time.time;
        while (startTime + 0.25f > Time.time)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - (direction / 50f), ((startTime + 0.25f) - Time.time));
            yield return 0;
        }
        transform.position = Cell.transform.position + new Vector3(0, 0, -0.1f);

        //GetComponent<SpriteRenderer>().sortingOrder = 4;
        imageToBeJerked.sortingOrder = 4;
    }
    private IEnumerator Glow(Color color, float cooloutTime)
    {
        var _renderer = transform.Find("Marker").GetComponent<SpriteRenderer>();
        float startTime = Time.time;

        while (startTime + cooloutTime > Time.time)
        {
            _renderer.color = Color.Lerp(new Color(1,1,1,0), color, (startTime + cooloutTime) - Time.time);
            yield return 0;
        }

        _renderer.color = Color.clear;
    }
    private IEnumerator Pulse(float breakTime, float delay, float scaleFactor)
    {
        var baseScale = transform.localScale;
        while (true)
        {
            float growingTime = Time.time;
            while (growingTime + delay > Time.time)
            {
                transform.localScale = Vector3.Lerp(baseScale * scaleFactor, baseScale, (growingTime + delay) - Time.time);
                yield return 0;
            }

            float shrinkingTime = Time.time;
            while (shrinkingTime + delay > Time.time)
            {
                transform.localScale = Vector3.Lerp(baseScale, baseScale * scaleFactor, (shrinkingTime + delay) - Time.time);
                yield return 0;
            }

            yield return new WaitForSeconds(breakTime);
        }
    }

    /// <summary>
    /// This seems to be the opposite of "MarkAsFinished"! 
    /// It can be used to set up the target at the start of the turn. 
    /// I think this is because "OnUnitDeselected" cannot really be called since an "Finished" unit cannot be selected in the first place. 
    /// Again, we may need to change this, but for it works so we use it. 
    /// </summary>
    public override void MarkAsFriendly()
    {
        //SetColor(new Color(0.8f, 1, 0.8f));

        // Set the frame to the active frame. 
        frameToBeSwapped.sprite = activeFrame;

        // Turn the sprite colors back to white. 
        SetColor(Color.white);
    }

    public override void MarkAsReachableEnemy()
    {
        //SetColor(new Color(1,0.8f,0.8f));

        // This is a bit intense, but it works for now. 
        if (selectedEmitters != null)
            foreach (StaticEmitterContinuous item in selectedEmitters)
            {
                item.Play();
            }
    }
    /// <summary>
    /// This will get triggered when we need to mark the unit as selected.
    /// We will change the existing functionality to instead give it a nice glow with particles.
    /// </summary>
    public override void MarkAsSelected()
    {
        //PulseCoroutine = StartCoroutine(Pulse(1.0f, 0.5f, 1.25f));
        //SetColor(new Color(0.8f, 0.8f, 1));

        if (selectedEmitters != null)
            foreach (StaticEmitterContinuous item in selectedEmitters)
            {
                item.Play();
            }

        //Debug.Log("[GenericUnit] MarkAsSelected: " + UnitName, this);
    }
    public override void MarkAsFinished()
    {
        // Set to the grey frame. 
        frameToBeSwapped.sprite = inactiveFrame;

        // Color the images grey. 
        SetColor(Color.grey);
    }
    /// <summary>
    /// Here we can turn off the emitter when the unit is unselected. 
    /// </summary>
    public override void UnMark()
    {
        //SetColor(Color.white);

        if (selectedEmitters != null)
            foreach (StaticEmitterContinuous item in selectedEmitters)
            {
                item.Stop();
            }


        //Debug.Log("[GenericUnit] UnMark: " + UnitName, this);
    }

    /// <summary>
    /// Use this to set the color of all our "imagesToBeColored" to a single sprite shade. 
    /// </summary>
    /// <param name="color"></param>
    private void SetColor(Color color)
    {
        if (imagesToBeColored != null)
        {
            foreach (SpriteRenderer image in imagesToBeColored)
            {
                image.color = color;
            }
        }

        if (uISpritesToColor != null)
        {
            foreach (Image image in uISpritesToColor)
            {
                image.color = color;
            }
        }
    }
}

