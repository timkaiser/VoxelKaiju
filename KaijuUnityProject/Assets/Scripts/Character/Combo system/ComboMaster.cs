using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static GameManager;

public enum Combostate
{
    Ready,
    Running,
    Done
}

public interface IComboMessage : IEventSystemHandler
{
    void MessageFinishedQuest(Combo lastCombo);
}

public class ComboMaster : MonoBehaviour
{
    static float defaultComboTime = 3.5f;
    public GameObject comboListener = null; //Link the listener here
    Character.KaijuController kaijuController;

    private Combo runningCombo;
    private Combo lastCombo;

    private Highscore highscore;

    private void Start()
    {
        runningCombo = new Combo(defaultComboTime);
        highscore = GameManager.Instance.GetComponent<Highscore>();
        kaijuController = transform.GetComponent<Character.KaijuController>();
    }

    private void Update()
    {
        if(runningCombo != null)
        {
            if(runningCombo.combostate == Combostate.Running)
            {
                runningCombo.Tick();
            }
            if(runningCombo.combostate == Combostate.Done)
            {
                BreakCombo();
            }
        }
    }

    /// <summary>
    /// Breaks the last running combo and stores it as last (finished) Combo
    /// Returns true if combo successful broken
    /// </summary>
    private void BreakCombo()
    {
        if (runningCombo.combostate == Combostate.Ready)
            return;
        lastCombo = runningCombo;
        runningCombo = new Combo(defaultComboTime);
        //Test
        highscore.ResetMultiplier();
        //
        ExecuteEvents.Execute<IComboMessage>(comboListener, null, (x, y) => x.MessageFinishedQuest(lastCombo));
    }

    /// <summary>
    /// Called to begin or refresh the combo
    /// </summary>
    public void RegisterAttack (Attack attack, uint destructLevel)
    {
        if(destructLevel >= kaijuController.GetGrowthLevel())
        {
            runningCombo.Refresh(attack);
            //Maybe do the current combocount calculation after this step -> then tell the highscore to set the multiplier
            if (runningCombo.getChainCount() % 5 == 0)
                highscore.AddToMultiplier(0.5f);
        }
    }

    /// <summary>
    /// get the current state of the running combo
    /// </summary>
    /// <returns></returns>
    public Combostate GetRunningComboState ()
    {
        return runningCombo.combostate;
    }

    /// <summary>
    /// Get the last FINISHED combo
    /// </summary>
    /// <returns></returns>
    public Combo GetLastCombo()
    {
        return lastCombo;
    }
}


public class Combo
{
    private List<Attack> attackChain;

    public Combostate combostate = Combostate.Ready;
    float comboTime;
    float fullComboTime;

    public Combo(float comboTime)
    {
        attackChain = new List<Attack>();
        this.comboTime = comboTime;
        this.fullComboTime = comboTime;
        this.combostate = Combostate.Ready;
    }

    public void Tick()
    {
        if(combostate == Combostate.Running)
        {
            if (GameManager.Instance.GameState.Equals(GameStates.Paused))
                return;
            if (comboTime < 0.0f)
            {
                combostate = Combostate.Done;
                FinishCombo();
            }

            comboTime -= Time.deltaTime;
        }
    }

    public void Refresh(Attack attack)
    {
        if(this.combostate == Combostate.Ready)
        {
            combostate = Combostate.Running;
        }
        if(this.combostate == Combostate.Running)
        {
            comboTime = fullComboTime;            
        }
        attackChain.Add(attack);
    }

    public List<Attack> getAttackChain() { return attackChain; }

    public int getChainCount() { return attackChain.Count; }

    //Do what is nessesary after a finished combo
    private void FinishCombo()
    {
        Debug.Log("Combo timed out");
    }
}
