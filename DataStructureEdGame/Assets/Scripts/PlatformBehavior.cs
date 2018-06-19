﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformBehavior : MonoBehaviour {

    public GameController gameController; // reference to the very important game controller.
    public List<LinkBlockBehavior> incomingConnectionLinkBlocks; // the link block that connects to this platform, or null if not being connected to.
    public GameObject childLink; // reference to the child link object.
    public GameObject childValueBlock; // reference to the child link object.
    private int value;

    public Sprite defaultSprite; // initial sprite
    public Sprite phasedOutSprite; // sprite to display when phased out.

    // game specific values
    public bool isHidden; // if not Hidden, then Revealed.
    public bool isPhasedOut; // if not Phased Out, then Solid. 

    /**
     * Remove an incoming link reference to this platform. 
     */
    public void removeIncomingConnectingLink(LinkBlockBehavior link)
    {
        incomingConnectionLinkBlocks.Remove(link); // remove this reference to the link
        link.connectingPlatform = null; // remove the reference to this platform in the link 
        gameController.updatePlatformEntities(); //  updatePlatformValuesAndSprite();
    }

    /**
     * Set what Link block this platform is being connected by. Also updates Platform state and render information.
     */
    public void addIncomingConnectingLink(LinkBlockBehavior link)
    {
        incomingConnectionLinkBlocks.Add(link);
        gameController.updatePlatformEntities(); // updatePlatformValuesAndSprite();
    }

    /**
     * Update the sprite based on the isHidden, isPhasedOut values. 
     */
    public void updatePlatformValuesAndSprite()
    {
        isPhasedOut = !isPlatformConnectingToStart();
        if (isPhasedOut)
        {
            isHidden = true; // phased out blocks are always hidden.
            if (GetComponent<SpriteRenderer>().sprite != phasedOutSprite)
            {
                GetComponent<SpriteRenderer>().sprite = phasedOutSprite;
            }
            GetComponent<BoxCollider2D>().isTrigger = true; // so player can pass through it.   
        } else // solid
        {
            if (GetComponent<SpriteRenderer>().sprite != defaultSprite) // default sprite for solid platform.
            {
                GetComponent<SpriteRenderer>().sprite = defaultSprite;
            }
            GetComponent<BoxCollider2D>().isTrigger = false;

            // see if you are connected to a helicopter link. If so then make block Revealed
            isHidden = true;
            foreach (LinkBlockBehavior lnk in incomingConnectionLinkBlocks)
            {
                if (lnk.isHelicopterLink)
                {
                    isHidden = false; // Reveal block.
                    break;
                }
            }
        }

        /*if (childLink.GetComponent<LinkBlockBehavior>().connectingPlatform != null) // update all blocks in the future chain 
        {
            childLink.GetComponent<LinkBlockBehavior>().connectingPlatform.updatePlatformValuesAndSprite();
        }*/
        // update the link block arrow for the inner child
        childLink.GetComponent<LinkBlockBehavior>().UpdateLinkArrow();

        // set the visibility of the children objects based on hidden value 
        childLink.SetActive(!isHidden);
        childValueBlock.SetActive(!isHidden);
    }

    /**
     * Check if this platform is connected by the special starting link
     */
    public bool isPlatformConnectingToStart()
    {
        List<PlatformBehavior> alreadySearchedPlatforms = new List<PlatformBehavior>();
        PlatformBehavior temp = gameController.startingLink.connectingPlatform;
        while (temp != null)
        {
            if (temp == this)
            {
                return true;
            }
            if (temp.childLink != null)
            {
                alreadySearchedPlatforms.Add(temp);
                temp = temp.childLink.GetComponent<LinkBlockBehavior>().connectingPlatform;
                if (alreadySearchedPlatforms.Contains(temp)) // you have reached the end of the list or there is an infinite loop
                    return false;
            }
        }
        return false;
    }

    void OnMouseEnter()
    {
        if (gameController.debugLinkControlVersion == 0) { 
            gameController.setConnectingPlatform(this); // when the mouse goes over a platform, create reference in game controller.
            gameController.updateObjectiveBlocks();
        }
    }

    void OnMouseExit()
    {
        if (gameController.debugLinkControlVersion == 0) { 
            gameController.setConnectingPlatform(null); // when the mouse leaves a platform, remove the reference in game controller.
            gameController.updateObjectiveBlocks();
        }
    }

    public void setDisplaySelected(bool b)
    { 
        transform.Find("SelectMarker").gameObject.SetActive(b);
    }

    public void setValue(int s)
    {
        if (childValueBlock != null) { 
            value = s;
            childValueBlock.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + value;
        }
    }

    public int getValue()
    {
        return value;
    }
    
    // Use this for initialization
    void Start () {
        // defaultSprite = GetComponent<SpriteRenderer>().sprite;
        // establish child link connections
        childLink = transform.Find("LinkBlock").gameObject;
        childValueBlock = transform.Find("ValueBlock").gameObject;
        childLink.GetComponent<LinkBlockBehavior>().gameController = gameController;
        childLink.GetComponent<LinkBlockBehavior>().parentPlatform = this;
        // updatePlatformValuesAndSprite();
    }

    // Update is called once per frame
    void Update () {

    }
}
