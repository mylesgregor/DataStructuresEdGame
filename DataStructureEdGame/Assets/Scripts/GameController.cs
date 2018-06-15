﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
   
    // References to important objects in the scene. 
    public Transform playerRef;
    public LinkBlockBehavior startingLink; // what is this level's starting link block?
    public LinkBlockBehavior addingLink; // what Link block the player is adding a connection to, if any
    public PlatformBehavior connectingPlatform; 

    void Start()
    {
        addingLink = null;
        // ensure the starting link has the proper property
        if (startingLink != null)
        {
            startingLink.isStartingLink = true;
        }
    }

    /**
     * Set what LinkBlock is being added by the Player.
     */
    public void setAddingLink(LinkBlockBehavior aLink)
    {
        if (addingLink != null)
        {
            addingLink.setDisplaySelected(false);
        }
        addingLink = aLink; // do other data processing?

        if (addingLink != null)
        { 
            addingLink.setDisplaySelected(true);
        }
    }

    /**
     * Set what Platform is being connected to by the player. 'addingLink' must not be null.
     */ 
    public void setConnectingPlatform(PlatformBehavior platform)
    {
        if (addingLink != null) // you can only connect to a platform when there is a Link.
        {
            if (addingLink.parentPlatform != null && addingLink.parentPlatform == platform)
                return; // don't change anything if the platform is the parent of the adding link.
            
            if (connectingPlatform != null)
            {
                connectingPlatform.setDisplaySelected(false);
            }
            connectingPlatform = platform;
            if (addingLink != null && connectingPlatform != null) // only display it for this platform if you're also selecting a Link.
            {
                connectingPlatform.setDisplaySelected(true);
            }
        }
    }

    void Update()
    {
        if (playerRef != null)
        {
            // always set the camera on top of the player.
            transform.position = new Vector3(playerRef.position.x, playerRef.position.y, transform.position.z);
        }

        if (addingLink != null && !Input.GetMouseButton(0)) // mouse was released 
        {
            if (connectingPlatform != null)
            {
                addingLink.setConnectingPlatform(connectingPlatform);
            }
            // deselect when you release the mouse button.
            setConnectingPlatform(null);
            setAddingLink(null); 
        }
    }
}
