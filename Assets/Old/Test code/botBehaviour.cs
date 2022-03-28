using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class botBehaviour : MonoBehaviour
{
    private BotClass bot;

    // Start is called before the first frame update
    void Start()
    {
        print("begin");

        bot = new BotClass(1, 2);
    }

    // Update is called once per frame
    void Update()
    {
        print("update");

        bot.SetSpeed(5);
    }
}
