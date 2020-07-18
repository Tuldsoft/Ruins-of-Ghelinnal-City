using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This EventTimer object is used extensively throughout the project. Its origin is from a
/// course in C# and Unity from Coursera. This adaptation of it includes Event management, such
/// that when the timer is completed, it triggers a localized event, heard only to the script it
/// was instantiated in. That allows a script to cause a method of its choosing to execute after
/// a certain period of time.
/// This functionality largely duplicates coroutine-timed "Yield" keywords. At a later date,
/// many such timers should be replaced by coroutines, as Unity would like to deprecate the
/// Event system that this timer relies on.
/// Its "tick" system is unused in RoGC, but was used in another project.
/// It has a "Retry" feature, where if an invocation did not have the desired effect, the 
/// timer can be told to re-invoke the same method in the next frame.
/// </summary>
public class EventTimer : MonoBehaviour
{
	#region Fields
	
	// Timer duration
	float totalSeconds = 0;

    // Interval to "tick" in seconds, unused in RoGC
    float tickInterval = 1f;
    bool isUsingTick = false;
	
	// Timer execution
	float elapsedSeconds = 0; 
	bool running = false;
    bool retry = false;       // whether or not the current invocation is a "retry" attempt

    // support for countdown seconds values
    int previousCountdownValue;

    // support for Finished property
    bool started = false;

    // Localized Event Management - does not use an Event Manager
    TimerFinishedEvent finishEvent = new TimerFinishedEvent();
    TimerTickEvent tickEvent = new TimerTickEvent();            // tickEvents are not implemented in RoGC

	#endregion

	#region Properties
	
	/// <summary>
	/// Sets the duration of the timer.
	/// The duration can only be set if the timer isn't currently running.
    /// To add duration, use the AddTime() method.
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
    /// Sets the interval of the Tick events (unused in RoGC)
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
	
    // Make elapsed seconds available for viewing
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

    // Retrieves the tick object, so that it does not have to be stored locally, unused in RoGC
    public TimerTickEvent TickEvent { get { return tickEvent; } }

    #endregion


    #region Methods

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {	
		// Update timer and check for finished
		if (running)
        {
			elapsedSeconds += Time.deltaTime;

            if (isUsingTick) // unused in RoGC
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
        
        // if this was a retry attempt, disable a retry on the next frame.
        if(retry) { finishEvent.Invoke(); retry = false; }

	}
	
	/// <summary>
	/// Runs the timer.
	/// Because a timer of 0 duration doesn't really make sense,
	/// the timer only runs if the total seconds is larger than 0.
	/// This also makes sure the consumer of the class has actually 
	/// set the duration to something higher than 0.
	/// </summary>
	public void Run()
    {	
		// Only run with valid duration
		if (totalSeconds > 0)
        {
			started = true;
			running = true;
            elapsedSeconds = 0;
		}
	}

    // Returns the number of "ticks" remaining (default seconds, round up), unused in RoGC
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

    // Invokes its finished method in the very next frame (Update() )
    public void Retry()
    {
        retry = true;
    }

    // Stops the timer, preventing the Finished invocation. Does not change remaining duration.
    public void Stop()
    {
        started = false;
        running = false;
    }
    #endregion

    #region Listener Methods
    /// <summary>
    /// Adds the given event handler as a listener to this invoker
    /// </summary>
    /// <param name="handler">the event handler</param>
    public void AddListener_Tick(UnityAction<int> handler)
    {
        tickEvent.AddListener(handler);
    }

    /// <summary>
    /// Adds the given event handler as a listener to this invoker
    /// </summary>
    /// <param name="handler">the event handler</param>
    public void AddListener_Finished(UnityAction handler)
    {
        finishEvent.AddListener(handler);
    }
	#endregion
}
