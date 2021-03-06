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
    private GameController gameController;
    private string loginAttemptResponse;  // store the response from the login attempt.
    private string worldStateField;

    public int currentPlayerID; // to identify players in log data.

    [Header("Logging configuration")]
    public bool enableLogging;
    public string loginAuthenticationUrl;
    public string updateLevelOnUrl;
    public string sendLogDataUrl;

    void Start()
    {
        if (!loginAuthenticationUrl.EndsWith("/"))
            loginAuthenticationUrl = loginAuthenticationUrl + "/";
        if (!updateLevelOnUrl.EndsWith("/"))
            updateLevelOnUrl = updateLevelOnUrl + "/";
        if (!sendLogDataUrl.EndsWith("/"))
            sendLogDataUrl = sendLogDataUrl + "/";
    }

    private IEnumerator sendLogToServer(string actionMsg, string timestamp)
    {
        WWWForm logForm = new WWWForm();
        logForm.AddField("playerID", currentPlayerID);
        string levelFileName = "NO LEVEL";
        if (gameController.worldGenerator.levelFileIndex < gameController.worldGenerator.levelDescriptionJsonFiles.Length)
        {
            levelFileName = gameController.worldGenerator.levelDescriptionJsonFiles[gameController.worldGenerator.levelFileIndex].name;
        }
        logForm.AddField("levelFile", levelFileName);
        logForm.AddField("actionMsg", actionMsg);
        logForm.AddField("timestamp", timestamp);
        logForm.AddField("worldState", worldStateField);

        using (UnityWebRequest www = UnityWebRequest.Post(sendLogDataUrl + "SendLogData.php", logForm))
        {
            yield return www.Send();
            if (www.isError)
            {
                Debug.Log("Error with sending the log message");
            }  
        }
    }


    public void sendLogToServer(string actionMsg)
    {
        List<BlockJSON> blockList;
        List<LinkBlockJSON> linkyList;
        List<LLPlatformForLogging> singleLLlist;
        BlockJSON player = new BlockJSON();
        BlockJSON helicopter = new BlockJSON();

        linkyList = new List<LinkBlockJSON>();
        blockList = new List<BlockJSON>();
        singleLLlist = new List<LLPlatformForLogging>();
        foreach (Transform t in gameController.worldGenerator.levelEntities)
        {
            if (t.GetComponent<LinkBehavior>() != null)
            {
                //then this is of type LinkedBlock

                LinkBlockJSON newB = new LinkBlockJSON();
                newB.x = t.position.x;
                newB.y = t.position.y;
                newB.type = "linkBlock";
                newB.logId = t.GetComponent<LoggableBehavior>().getLogID();
                if(t.GetComponent<LinkBehavior>().connectableEntity != null)
                {
                    newB.objIDConnectingTo = t.GetComponent<LinkBehavior>().connectableEntity.GetComponent<LoggableBehavior>().getLogID();
                }
                else
                {
                    newB.objIDConnectingTo = "";
                }
                linkyList.Add(newB);
            } else if(t.GetComponent<PlatformBehavior>() != null)
            {
                LLPlatformForLogging platB = new LLPlatformForLogging();
                platB.x = t.position.x;
                platB.y = t.position.y;
                platB.type = "LLplatform";
                platB.logId = t.GetComponent<LoggableBehavior>().getLogID();
                platB.objId = t.GetComponent<LoggableBehavior>().getLogID();
                if (t.GetComponent<PlatformBehavior>().getChildLink().connectableEntity != null)
                {
                    platB.childLinkBlockConnectId = t.GetComponent<PlatformBehavior>().getChildLink().connectableEntity.GetComponent<LoggableBehavior>().getLogID();
                } else
                {
                    platB.childLinkBlockConnectId = "";
                }
                platB.value = t.GetComponent<ContainerEntityBehavior>().GetChildComponent<ValueBehavior>().getValue();
                platB.toAdd = gameController.platformsToAdd.Contains(t.GetComponent<PlatformBehavior>());

                platB.isHidden = t.GetComponent<ContainerEntityBehavior>().isHidden();
                platB.isSolid = !t.GetComponent<PlatformBehavior>().isPhasedOut();
                singleLLlist.Add(platB);
            }
            else if(t.GetComponent<PlayerBehavior>() != null)
            {
                player.x = Math.Round(t.position.x, 2);
                player.y = Math.Round(t.position.y, 2);
                player.type = "player";
                player.logId = t.GetComponent<LoggableBehavior>().getLogID();
            }
            else if(t.GetComponent<HelicopterRobotBehavior>() != null)
            {

                helicopter.x = Math.Round(t.position.x, 2);
                helicopter.y = Math.Round(t.position.y, 2);
                helicopter.type = "helicopter";
                helicopter.logId = t.GetComponent<LoggableBehavior>().getLogID();
            }
        }

        LogMsgRepresentation current = new LogMsgRepresentation();
        current.linkBlockPart = linkyList.ToArray();
        current.platformPart = singleLLlist.ToArray();
        current.player = player;
        current.helicopter = helicopter;
        worldStateField = current.SaveString();

        if (enableLogging) { 
            String timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            StartCoroutine(sendLogToServer(actionMsg, timestamp));
        }
    }


    private IEnumerator attemptLogin(string playerId, string pw)
    {
        WWWForm loginForm = new WWWForm();
        loginForm.AddField("playerID", playerId);
        loginForm.AddField("pw", pw);
        using (UnityWebRequest www = UnityWebRequest.Post(loginAuthenticationUrl + "LoginAuthentication.php", loginForm))
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
        WWWForm logForm = new WWWForm();
        logForm.AddField("playerID", currentPlayerID);
        logForm.AddField("levelOn", gameController.worldGenerator.levelFileIndex);

        using (UnityWebRequest www = UnityWebRequest.Post(updateLevelOnUrl + "UpdateLevelOn.php", logForm))
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

    public string getLoginAttemptResponse()
    {
        return loginAttemptResponse;
    }

    public void setGameController(GameController gc)
    {
        gameController = gc;
    }
}
