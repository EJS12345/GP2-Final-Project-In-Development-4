using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;  // Added to reference TextMeshPro

public class CarLapCounter : MonoBehaviour
{
    public TMP_Text carPositionText;  // Use TMP_Text instead of Text
    public TMP_Text lapText;  // Use TMP_Text for lap counter

    int passedCheckPointNumber = 0;
    float timeAtLastPassedCheckPoint = 0;

    int numberOfPassedCheckpoints = 0;

    int lapsCompleted = 0;
    const int lapsToComplete = 4;  // Set to 4 laps as required

    bool isRaceCompleted = false;

    int carPosition = 0;

    bool isHideRoutineRunning = false;
    float hideUIDelayTime;

    // Other components
    LapCounterUIHandler lapCounterUIHandler;

    // Events
    public event Action<CarLapCounter> OnPassCheckpoint;

    void Start()
    {
        if (CompareTag("Player"))
        {
            lapCounterUIHandler = FindObjectOfType<LapCounterUIHandler>();

            if (lapCounterUIHandler != null)
            {
                lapCounterUIHandler.SetLapText($"LAP {lapsCompleted + 1}/{lapsToComplete}");  // Initial display for lap count
            }
            else
            {
                Debug.LogError("LapCounterUIHandler not found in the scene. Please ensure it is added.");
            }
        }
    }

    public void SetCarPosition(int position)
    {
        carPosition = position;
    }

    public int GetNumberOfCheckpointsPassed()
    {
        return numberOfPassedCheckpoints;
    }

    public float GetTimeAtLastCheckPoint()
    {
        return timeAtLastPassedCheckPoint;
    }

    public bool IsRaceCompleted()
    {
        return isRaceCompleted;
    }

    IEnumerator ShowPositionCO(float delayUntilHidePosition)
    {
        hideUIDelayTime += delayUntilHidePosition;

        carPositionText.text = carPosition.ToString();  // Update text using TMP_Text

        carPositionText.gameObject.SetActive(true);

        if (!isHideRoutineRunning)
        {
            isHideRoutineRunning = true;

            yield return new WaitForSeconds(hideUIDelayTime);
            carPositionText.gameObject.SetActive(false);

            isHideRoutineRunning = false;
        }

    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("CheckPoint"))
        {
            // Once a car has completed the race, we don't need to check any checkpoints or laps.
            if (isRaceCompleted)
                return;

            CheckPoint checkPoint = collider2D.GetComponent<CheckPoint>();

            // Make sure that the car is passing the checkpoints in the correct order. The correct checkpoint must have exactly 1 higher value than the passed checkpoint
            if (passedCheckPointNumber + 1 == checkPoint.checkPointNumber)
            {
                passedCheckPointNumber = checkPoint.checkPointNumber;

                numberOfPassedCheckpoints++;

                // Store the time at the checkpoint
                timeAtLastPassedCheckPoint = Time.time;

                if (checkPoint.isFinishLine)
                {
                    passedCheckPointNumber = 0;
                    lapsCompleted++;

                    // Update the lap counter UI
                    if (lapCounterUIHandler != null)
                        lapCounterUIHandler.SetLapText($"LAP {lapsCompleted + 1}/{lapsToComplete}");

                    if (lapsCompleted >= lapsToComplete)
                    {
                        isRaceCompleted = true;
                    }
                }

                // Invoke the passed checkpoint event
                OnPassCheckpoint?.Invoke(this);

                // Now show the car's position as it has been calculated, but only do it when a car passes through the finish line
                if (isRaceCompleted)
                {
                    StartCoroutine(ShowPositionCO(100));

                    if (CompareTag("Player"))
                    {
                        GameManager.instance.OnRaceCompleted();

                        GetComponent<CarInputHandler>().enabled = false; // Disable car controls after the race is completed
                        GetComponent<CarAIHandler>().enabled = true;    // Enable AI for post-race behavior (optional)
                    }
                }
                else if (checkPoint.isFinishLine)
                    StartCoroutine(ShowPositionCO(1.5f)); // Show position for a short duration after passing the finish line
            }
        }
    }
}


