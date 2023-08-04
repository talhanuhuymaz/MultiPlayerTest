using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayersManager : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> playersInGame = new NetworkVariable<int>(0);
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI timerText;

    private bool timerStarted = false;
    private NetworkVariable<float> currentTimer = new NetworkVariable<float>(0f); // Use NetworkVariable<float> for currentTimer

    public float timerDuration = 60f; // Set the duration of the timer in seconds

    void Start()
    {
        UpdatePlayerCountText();

        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer)
            {
                playersInGame.Value += 1;
                CheckStartTimer();
            }
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (IsServer)
            {
                playersInGame.Value -= 1;
            }
        };

        playersInGame.OnValueChanged += (prevValue, newValue) =>
        {
            UpdatePlayerCountText();
            CheckStartTimer();
        };

        // Add a listener for currentTimer to update the timer text on all clients when the value changes
        currentTimer.OnValueChanged += (prevValue, newValue) =>
        {
            UpdateTimerText();
        };
    }

    private void CheckStartTimer()
    {
        if (IsServer && playersInGame.Value >= 2 && !timerStarted)
        {
            StartTimer();
        }
    }

    private void StartTimer()
    {
        timerStarted = true;
        currentTimer.Value = timerDuration; // Set the initial value of currentTimer
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        while (currentTimer.Value > 0)
        {
            yield return new WaitForSeconds(1f);
            currentTimer.Value -= 1f;
        }

        // Timer finished, do something here
        Debug.Log("Timer finished!");
    }

    private void UpdatePlayerCountText()
    {
        if (playerCountText != null)
        {
            playerCountText.text = "Players in Game: " + playersInGame.Value.ToString();
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTimer.Value / 60);
            int seconds = Mathf.FloorToInt(currentTimer.Value % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}