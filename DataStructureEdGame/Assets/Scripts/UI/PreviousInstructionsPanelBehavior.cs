﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * For the interface to open a menu with buttons to open previously encountered instruction panels.
 */
public class PreviousInstructionsPanelBehavior : MonoBehaviour {

    public RectTransform viewInstructionButtonPrefab;

    private List<RectTransform> previousInstructions;
    private RectTransform containerPanel;
    private Button toggleButton;
    private bool showingPanel;

	void Start ()
    {
        toggleButton = transform.Find("PreviousInstructionsButton").GetComponent<Button>();
        toggleButton.onClick.AddListener(TogglePanelView);
        ensureReferences();
    }

    public void addInstructionPanelToHistory(RectTransform panel)
    {
        if (previousInstructions == null)
        {
            showingPanel = false;
            previousInstructions = new List<RectTransform>();
        }
        if (!previousInstructions.Contains(panel))
        {
            previousInstructions.Add(panel); // add to the end of the list.

            // find the button in the container
            foreach (ViewInstructionPanelButton viewBtn in containerPanel.GetComponentsInChildren<ViewInstructionPanelButton>(true))
            {
                if (viewBtn.panelToReveal == panel) {
                    viewBtn.gameObject.SetActive(true);
                }
            }
        }
    }

    void TogglePanelView()
    {
        if (showingPanel)
        {
            hideContainer();
        } else
        {
            showContainer();
        }
    }

    public void hideContainer()
    {
        showingPanel = false;
        containerPanel.gameObject.SetActive(false);
    }

    public void showContainer()
    {
        showingPanel = true;
        containerPanel.gameObject.SetActive(true);
    }

    public void ensureReferences()
    {
        if (containerPanel == null)
        {
            containerPanel = transform.Find("ContainerPanel").GetComponent<RectTransform>();
        }
    }
}
