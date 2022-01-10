using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayUI : MonoBehaviour
{
    public Text waveCount, gameOver;
    private SpawnManager spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        // Get the spawn manager
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Display the wave and score texts
        waveCount.text = "Wave: " + spawnManager.wave.ToString();
        gameOver.text = "You've reached Wave "+ spawnManager.wave +"!";
    }
}
