using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A timer
/// </summary>
public class EventTimer : MonoBehaviour
{
	#region Fields
	
	// timer duration
	float totalSeconds = 0;

    // interval to "tick"
    float tickInterval = 1f;
    bool isUsingTick = false;
	
	// timer execution
	float elapsedSeconds = 0;
	bool running = false;
    bool retry = false;

    // support for countdown seconds values
    int previousCountdownValue;

    // support for Finished property
    bool started = false;

    TimerFinishedEvent finishEvent = new TimerFinishedEvent();
    TimerTickEvent tickEvent = new TimerTickEvent();

	#endregion
	

	#region Properties
	
	/// <summary>
	/// Sets the duration of the timer
	/// The duration can only be set if the timer isn't currently running
    /// To add duration, use the AddTime() method
	/// </summary>
	/// <value>duration</value>
	public float Duration
    {
		set
        {
			if (!running)
            {
				totalSeconds = value;
			}
		}
	}
    /// <summary>
    /// Sets the interval of the Tick events
    /// </summary>
    public float TickInterval 
    {
        set
        {
            if (!running)
            {
                isUsingTick = true;
                tickInterval = value;
            }
        }
    }
	
    // keep elapsed seconds available
    public float ElapsedSeconds
    {
        get { return elapsedSeconds; }
    }

	/// <summary>
	/// Gets whether or not the timer has finished running
	/// This property returns false if the timer has never been started
	/// </summary>
	/// <value>true if finished; otherwise, false.</value>
	public bool Finished
    {
		get { return started && !running; } 
	}
	
	/// <summary>
	/// Gets whether or not the timer is currently running
	/// </summary>
	/// <value>true if running; otherwise, false.</value>
	public bool Running
    {
		get { return running; }
	}

    // Retrieves the tick object, so that it does not have to be stored locally
    public TimerTickEvent TickEvent { get { return tickEvent; } }

    #endregion


    #region Methods

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {	
		// update timer and check for finished
		if (running)
        {
			elapsedSeconds += Time.deltaTime;

            if (isUsingTick)
            {
                // check for new countdown value
                int newCountdownValue = GetCurrentCountdownValue();
                if (newCountdownValue != previousCountdownValue)
                {
                    previousCountdownValue = newCountdownValue;
                    tickEvent.Invoke(previousCountdownValue);
                }
            }

            // check for timer finished
            if (elapsedSeconds >= totalSeconds)
            {
                running = false;
                finishEvent.Invoke();
            }
		}
        
        if(retry) { finishEvent.Invoke(); retry = false; }

	}
	
	/// <summary>
	/// Runs the timer
	/// Because a timer of 0 duration doesn't really make sense,
	/// the timer only runs if the total seconds is larger than 0
	/// This also makes sure the consumer of the class has actually 
	/// set the duration to something higher than 0
	/// </summary>
	public void Run()
    {	
		// only run with valid duration
		if (totalSeconds > 0)
        {
			started = true;
			running = true;
            elapsedSeconds = 0;
		}
	}

    // Returns the number of "ticks" remaining (default seconds, round up)
    int GetCurrentCountdownValue()
    {
        return (int)Mathf.Ceil((totalSeconds - elapsedSeconds) / tickInterval);
    }


    /// <summary>
    /// Adds time to the total amount of time before the time finishes. Used
    /// while the timer is running, or not.
    /// </summary>
    /// <param name="seconds"></param>
    public void AddTime(float seconds)
    {
        totalSeconds += seconds;
    }

    public void Retry()
    {
        retry = true;
    }

    // Stops the timer
    public void Stop()
    {
        started = false;
        running = false;
    }
    #endregion

    #region Listener Methods
    /// <summary>
    /// Adds the given event handler as a listener
    /// </summary>
    /// <param name="handler">the event handler</param>
    public void AddListener_Tick(UnityAction<int> handler)
    {
        tickEvent.AddListener(handler);
    }

    /// <summary>
    /// Adds the given event handler as a listener
    /// </summary>
    /// <param name="handler">the event handler</param>
    public void AddListener_Finished(UnityAction handler)
    {
        finishEvent.AddListener(handler);
    }

    

	#endregion
}
