﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.WorldGeneration;

public class LoggingManager : MonoBehaviour
{

    public GameController gameRef;
    public int currentPlayerID;
    public bool enableLogging;
    public string loginAttemptResponse;  // store the response from the login attempt.
    private string worldStateField;

    private IEnumerator sendLogToServer(string actionMsg, string timestamp)
    {
        string logUrl = "http://localhost/test/sendingDataToPHP.php";
        WWWForm logForm = new WWWForm();
        logForm.AddField("playerID", currentPlayerID);
        string levelFileName = "NO LEVEL";
        if (gameRef.worldGenerator.levelFileIndex < gameRef.worldGenerator.levelDescriptionJsonFiles.Length)
        {
            levelFileName = gameRef.worldGenerator.levelDescriptionJsonFiles[gameRef.worldGenerator.levelFileIndex].name;
        }
        logForm.AddField("levelFile", levelFileName);
        logForm.AddField("actionMsg", actionMsg);
        logForm.AddField("timestamp", timestamp);
        logForm.AddField("worldState", worldStateField);
        //Debug.Log("About to send");
        using (UnityWebRequest www = UnityWebRequest.Post(logUrl, logForm))
        {
            yield return www.Send();
            if (www.isError)
            {
                Debug.Log("Error with sending the log message");
            } else
            {
                //Debug.Log(www.downloadHandler.text);
            }
        }
    }


    public void send_To_Server(string actionMsg)
    {
        List<Block> blockList;
        List<LinkBlock> linkyList;
        List<LLPlatformForLogging> singleLLlist;
        Block player = new Block();
        Block helicopter = new Block();

        linkyList = new List<LinkBlock>();
        blockList = new List<Block>();
        singleLLlist = new List<LLPlatformForLogging>();
        foreach (Transform t in gameRef.worldGenerator.levelEntities)
        {
            if (t.GetComponent<LinkBlockBehavior>() != null)
            {
                //then this is of type LinkedBlock

                LinkBlock newB = new LinkBlock();
                newB.x = t.position.x;
                newB.y = t.position.y;
                newB.type = "linkBlock";
                newB.logId = t.GetComponent<LinkBlockBehavior>().logId;
                if(t.GetComponent<LinkBlockBehavior>().connectingPlatform != null)
                {
                    newB.objIDConnectingTo = t.GetComponent<LinkBlockBehavior>().connectingPlatform.logId;
                }
                else
                {
                    newB.objIDConnectingTo = "";
                }
                linkyList.Add(newB);
            }

            else if (t.GetComponent<GroundBehavior>() != null)
            {
                /**
                Block groundB = new Block();
                groundB.x = t.position.x;
                groundB.y = t.position.y;
                groundB.type = "ground";
                groundB.logId = t.GetComponent<GroundBehavior>().logId;
                blockList.Add(groundB);
                */

            }
            else if(t.GetComponent<PlatformBehavior>() != null)
            {
                Debug.Log("I made it here");
                LLPlatformForLogging platB = new LLPlatformForLogging();
                platB.x = t.position.x;
                platB.y = t.position.y;
                platB.type = "LLplatform";
                platB.logId = t.GetComponent<PlatformBehavior>().logId;
                platB.objId = t.GetComponent<PlatformBehavior>().logId;
                platB.childLinkBlockConnectId = t.GetComponent<PlatformBehavior>().childLink.GetComponent<LinkBlockBehavior>().logId;
                platB.value = t.GetComponent<PlatformBehavior>().getValue();
                //platB.toAdd = t.GetComponent<PlatformBehavior>().toadd;
                platB.toAdd = gameRef.platformsToAdd.Contains(t.GetComponent<PlatformBehavior>());
                //for (int i = 0; i < gameRef.platformsToAdd.Count; i++)
                //{
                //    Debug.Log("I made it to counting platforms to add" + gameRef.platformsToAdd.Count);
                //    if (t.GetComponent<PlatformBehavior>().logId == platB.logId)
                //    {
                //        platB.toAdd = true;
                //        Debug.Log("I set it to true");
                //    }
                //}
                platB.isHidden = t.GetComponent<PlatformBehavior>().isHidden;
                //Debug.Log("Hidden: "  + platB.isHidden + platB.logId);
                platB.isSolid = !(t.GetComponent<PlatformBehavior>().isPhasedOut);
                //Debug.Log("Solid: " + platB.isSolid + platB.logId);
                singleLLlist.Add(platB);
                //Debug.Log(singleLLlist.Count);
                //Platform Behavior has fields to tell if the platform isHidden, isSolid. Include?
            }
            else if(t.GetComponent<PlayerBehavior>() != null)
            {
                player.x = Math.Round(t.position.x, 2);
                player.y = Math.Round(t.position.y, 2);
                player.type = "player";
                player.logId = t.GetComponent<PlayerBehavior>().logId;
            }
            else if(t.GetComponent<HelicopterRobotBehavior>() != null)
            {

                helicopter.x = Math.Round(t.position.x, 2);
                helicopter.y = Math.Round(t.position.y, 2);
                helicopter.type = "helicopter";
                helicopter.logId = t.GetComponent<HelicopterRobotBehavior>().logId;
            }
        }

        LogMsgRepresentation current = new LogMsgRepresentation();
        //current.blockPart = blockList.ToArray();
        current.linkBlockPart = linkyList.ToArray();
        current.platformPart = singleLLlist.ToArray();
        current.player = player;
        current.helicopter = helicopter;
        worldStateField = current.SaveString();
        Debug.Log(current.SaveString());

        //it should be a list of all these types of 

        if (enableLogging) { 
            String timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            StartCoroutine(sendLogToServer(actionMsg, timestamp));
        }
    }


    private IEnumerator attemptLogin(string playerId, string pw)
    {
        string loginUrl = "http://localhost/test/loginAuthentication.php";
        WWWForm loginForm = new WWWForm();
        loginForm.AddField("playerID", playerId);
        loginForm.AddField("pw", pw);

        using (UnityWebRequest www = UnityWebRequest.Post(loginUrl, loginForm))
        {
            yield return www.Send();
            if (!www.isError)
            {
                loginAttemptResponse = www.downloadHandler.text;
            }

        }
    }

    public void beginAttemptLogin(string playerId, string pw) {
        loginAttemptResponse = ""; 
        StartCoroutine(attemptLogin(playerId, pw));
    }


    private IEnumerator updatePlayerLevelOn()
    {
        string logUrl = "http://localhost/test/updateLevelOn.php";
        WWWForm logForm = new WWWForm();
        logForm.AddField("playerID", currentPlayerID);
        logForm.AddField("levelOn", gameRef.worldGenerator.levelFileIndex);

        using (UnityWebRequest www = UnityWebRequest.Post(logUrl, logForm))
        {
            yield return www.Send();
            if (www.isError)
            {
                Debug.Log("Error with updating the level on.");
            }
        }
    }

    public void beginUpdateLastLevelOn()
    {
        StartCoroutine(updatePlayerLevelOn());
    }
}
