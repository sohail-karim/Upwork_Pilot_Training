using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTextCounter : MonoBehaviour
{
    CirclePatternManager circlePatternManager;
    // Start is called before the first frame update
    void Start()
    {
        circlePatternManager = CirclePatternManager.instance;
    }

    public void onClicked()
    {
        circlePatternManager.counter++;
    }
}
