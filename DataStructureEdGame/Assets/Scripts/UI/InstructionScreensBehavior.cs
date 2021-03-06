﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The script for the instruction panels container.
 * When colliding with an instruction block, this object will 
 * reveal the instruction panel whose name in the editor is
 * equal to the stringId property of that instruction block.
 */
public class InstructionScreensBehavior : MonoBehaviour {

    public PreviousInstructionsPanelBehavior previousPanelBehavior;
    // internal reference of all of the instruction panels
    private Dictionary<string, RectTransform> instructionPanels;

    void Start()
    {
        ensureInstructionPanelReferences();
    }

    private void changeScreen(string key, bool b)
    {
        if (instructionPanels.ContainsKey(key))
        {
            instructionPanels[key].gameObject.SetActive(b);
            if (b)
            {
                previousPanelBehavior.addInstructionPanelToHistory(instructionPanels[key]);
            }
        }
    }
    
    public void showScreen(string key)
    {
        changeScreen(key, true);
    }

    public void hideScreen(string key)
    {
        changeScreen(key, false);
    }

    /**
     * When the game is initially started, this function is called to 
     * reveal all instruction blocks that were encountered in previous levels.
     * This is because a user can login and start the game on a different 
     * level other than the first level. 
     */
    public void revealPlatformsForLevels(int lvlOn)
    {
        previousPanelBehavior.ensureReferences();
        if (lvlOn >= 1)
        {
            previousPanelBehavior.addInstructionPanelToHistory(instructionPanels["MoveInstructions"]);
            previousPanelBehavior.addInstructionPanelToHistory(instructionPanels["PlatformAnalogyInstructions"]);
            previousPanelBehavior.addInstructionPanelToHistory(instructionPanels["PlatformStatesInstructions"]);
            previousPanelBehavior.addInstructionPanelToHistory(instructionPanels["CreateInstructions"]); 
        }
        if (lvlOn >= 2)
        {
            previousPanelBehavior.addInstructionPanelToHistory(instructionPanels["GeneratedCodeInstruction"]);
            previousPanelBehavior.addInstructionPanelToHistory(instructionPanels["DeleteInstructions"]);
            previousPanelBehavior.addInstructionPanelToHistory(instructionPanels["PlatformHiddenRevealInstructions"]);
        }
        if (lvlOn >= 3)
        { 
            previousPanelBehavior.addInstructionPanelToHistory(instructionPanels["HelicopterRobotInstructions"]);
        }
        if (lvlOn >= 5)
        {
            previousPanelBehavior.addInstructionPanelToHistory(instructionPanels["AddPlatformInstructions"]);
        }
    }

    public void ensureInstructionPanelReferences()
    {
        if (instructionPanels == null)
        {
            instructionPanels = new Dictionary<string, RectTransform>();
        }
        if (instructionPanels.Count == 0)
        { 
            instructionPanels = new Dictionary<string, RectTransform>();
            foreach (Transform child in transform)
            {
                instructionPanels.Add(child.gameObject.name, child.gameObject.GetComponent<RectTransform>());
            }
        }
    }
}
