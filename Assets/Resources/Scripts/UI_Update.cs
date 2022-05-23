using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using System;

public class UI_Update : MonoBehaviour
{
    [SerializeField] TMP_Text collisionText;
    [SerializeField] MoveToGoalAgent agent;

    // Update is called once per frame
    void Update()
    {
        // Update UI text to include collision count and current reward
        collisionText.text = 
            "Collisions: " + agent.wallCollisionCount.ToString() + 
            "\nReward: " + agent.GetCumulativeReward().ToString();
    }
}
