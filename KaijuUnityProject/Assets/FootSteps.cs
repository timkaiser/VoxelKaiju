using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    private KaijuController controller;

    private SoundManager manager;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<KaijuController>();
        manager = SoundManager.Instance;
    }

    public void Step()
    {
        switch (controller.GetGrowthLevel())
        {
            case 0:
                manager.PlayStep("FootStepLight");
                break;
            case 1:
                manager.PlayStep("FootStepMedium");
                break;
            case 2:
                manager.PlayStep("FootStepHeavy");
                break;
            default:
                break;
        }        
    }
}
