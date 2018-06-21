﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.WorldGeneration;
using UnityEngine;

public class WorldGenerationBehavior : MonoBehaviour {

    public GameController gameController;

    // world generation properties.
    public int levelFileIndex;
    public TextAsset[] levelDescriptionJsonFiles;
    public bool generateWorld;
    public List<Transform> levelEntities; // all of the entities that were generated for the level.
    public List<LinkBlockBehavior> levelLinkBlocks;

    // references to PreFabs
    public Transform groundPreFab;
    public Transform playerPreFab;
    public Transform goalPortalPreFab;
    public Transform objectiveBlockPreFab;
    public Transform linkBlockPreFab;
    public Transform singleLinkedListPreFab;
    public Transform helicopterRobotPreFab;

    // references to Sprite
    public Sprite startLinkBlockSprite;
    public Sprite nullStartLinkBlockSprite;

    //reference to list of platformBehavior objects to add
    public PlatformBehavior listOfPlatsToAdd;

    // Use this for initialization
    void Start () {
        // ManualStartGenerator();
    }

    public void ManualStartGenerator()
    {
        if (generateWorld)
        {
            CreateWorldFromLevelDescription();
            Debug.Log("CREATED WORLD");
        }
    }

    /**
     * This function maps a string value to a PreFab value for level generation.
     */
    public Transform GetAssocInstanceFromType(string type)
    {
        if (type.Equals("GROUND"))
        {
            return groundPreFab;
        }

        return null;
    }

    public GameController.WinCondition GetWinConditionFromString(string str)
    {
        if (str.Equals("SortListAscending"))
        {
            return GameController.WinCondition.SortListAscending;
        } else if (str.Equals("SortListDescending"))
        {
            return GameController.WinCondition.SortListDescending;
        }
        return GameController.WinCondition.None;
    }

    /**
     * Generate a world using the given .JSON file in 'levelDescriptionJson'
     */
    public void CreateWorldFromLevelDescription()
    {
        LevelEntities level = JsonUtility.FromJson<LevelEntities>(levelDescriptionJsonFiles[levelFileIndex].text);
        gameController.winConditon = GetWinConditionFromString(level.winCondition);
        // while generating the level, add things to levelEntities list so it can be destroyed for the next level generated.
        for (int i = 0; i < level.blocks.Length; i++)
        {
            Vector2 blockPos = new Vector2((int)level.blocks[i].x, (int)level.blocks[i].y);
            Transform objToInstances = GetAssocInstanceFromType(level.blocks[i].type);
            if (objToInstances != null)
            {
                Transform obj = Instantiate(objToInstances, blockPos, Quaternion.identity);
                levelEntities.Add(obj);
            }
        }
        // create the player
        if (level.player != null)
        {
            gameController.playerRef = Instantiate(playerPreFab, new Vector2((int)level.player.x, (int)level.player.y), Quaternion.identity);
            gameController.playerRef.GetComponent<PlayerBehavior>().gameController = gameController;
            levelEntities.Add(gameController.playerRef);
        }
        if (level.goalPortal != null)
        {
            Transform goal = Instantiate(goalPortalPreFab, new Vector2((int)level.goalPortal.x, (int)level.goalPortal.y), Quaternion.identity);
            levelEntities.Add(goal);
        }
        if (level.helicopterRobot != null)
        {
            Transform robot = Instantiate(helicopterRobotPreFab, new Vector2((int)level.helicopterRobot.x, (int)level.helicopterRobot.y), Quaternion.identity);
            HelicopterRobotBehavior robotBehavior = robot.GetComponent<HelicopterRobotBehavior>();
            robotBehavior.gameController = gameController;
            robotBehavior.targetLocation = robot.position;
            gameController.helicopterRobotRef = robot;
            robotBehavior.childLink = robot.Find("LinkBlock").gameObject;
            levelEntities.Add(robot);
        }
        
        // list of link blocks we are creating
        levelLinkBlocks = new List<LinkBlockBehavior>();
        // corresponding list of IDs telling the link blocks what they should point to when the level is generated
        List<string> levelLinkBlocksConnIds = new List<string>();
        gameController.objectiveBlocks = new List<ObjectiveBlockBehavior>();
        gameController.platformEntities = new List<PlatformBehavior>();
        gameController.platformsToAdd = new List<PlatformBehavior>();
        // create the start link
        if (level.startLink != null)
        {
            Transform startLinkTran = Instantiate(linkBlockPreFab, new Vector2((int)level.startLink.x, (int)level.startLink.y), Quaternion.identity);
            LinkBlockBehavior startLinkBehavior = startLinkTran.GetComponent<LinkBlockBehavior>();
            startLinkBehavior.gameController = gameController;
            startLinkBehavior.isStartingLink = true;  // mark link as start.
            startLinkBehavior.defaultSprite = startLinkBlockSprite;
            startLinkBehavior.nullLinkSprite = nullStartLinkBlockSprite;
            startLinkBehavior.GetComponent<SpriteRenderer>().sprite = startLinkBlockSprite;
            gameController.startingLink = startLinkBehavior; // set start link reference
            levelLinkBlocks.Add(startLinkBehavior);
            levelLinkBlocksConnIds.Add(level.startLink.objIDConnectingTo);
            levelEntities.Add(startLinkTran);
        }

        // create the link blocks
        for (int i = 0; i < level.linkBlocks.Length; i++)
        {
            Transform newLink = Instantiate(linkBlockPreFab, new Vector2((int)level.linkBlocks[i].x, (int)level.linkBlocks[i].y), Quaternion.identity);
            LinkBlockBehavior lb = newLink.GetComponent<LinkBlockBehavior>();
            lb.gameController = gameController;
            levelLinkBlocks.Add(lb);
            Debug.Log("Made me some level link blocks");
            levelLinkBlocksConnIds.Add(level.linkBlocks[i].objIDConnectingTo);
            levelEntities.Add(newLink);
        }

        // create the objective blocks
        Debug.Log(level.objectiveBlocks);
        for (int i = 0; i < level.objectiveBlocks.Length; i++)
        {
            Transform newOBlock = Instantiate(objectiveBlockPreFab, new Vector2((int)level.objectiveBlocks[i].x, (int)level.objectiveBlocks[i].y), Quaternion.identity);
            ObjectiveBlockBehavior ob = newOBlock.GetComponent<ObjectiveBlockBehavior>(); 
            gameController.objectiveBlocks.Add(ob);
            levelEntities.Add(newOBlock);
        }

        // create the platforms.
        Dictionary<string, PlatformBehavior> listPlatformMap = new Dictionary<string, PlatformBehavior>();
        for (int i = 0; i < level.singleLinkedListPlatforms.Length; i++)
        {
            Transform newLLPlatform = Instantiate(singleLinkedListPreFab, new Vector2((int)level.singleLinkedListPlatforms[i].x, (int)level.singleLinkedListPlatforms[i].y), Quaternion.identity);
            PlatformBehavior newPlat = newLLPlatform.GetComponent<PlatformBehavior>();
            LinkBlockBehavior innerLink = newLLPlatform.Find("LinkBlock").GetComponent<LinkBlockBehavior>();
            innerLink.gameController = gameController;
            innerLink.parentPlatform = newPlat;
            newPlat.gameController = gameController;
            newPlat.childLink = innerLink.gameObject;
            newPlat.childValueBlock = newLLPlatform.Find("ValueBlock").gameObject;
            newPlat.setValue(level.singleLinkedListPlatforms[i].value);
            listPlatformMap.Add(level.singleLinkedListPlatforms[i].objId, newPlat);
            levelLinkBlocks.Add(innerLink); // add it to the list of blocks for references
            levelLinkBlocksConnIds.Add(level.singleLinkedListPlatforms[i].childLinkBlockConnectId);
            levelEntities.Add(newLLPlatform);
            gameController.platformEntities.Add(newPlat);

            if (level.singleLinkedListPlatforms[i].toAdd == true)
            {
                Debug.Log("It needs to be added");
                newLLPlatform.gameObject.SetActive(false);
                gameController.platformsToAdd.Add(newPlat);
            }
        }

        // establishing links for the link blocks with the platforms
        for (int i = 0; i < levelLinkBlocksConnIds.Count; i++) {
            if (levelLinkBlocksConnIds[i].Length > 0) // if this link has a connection.
            {
                string platformId = levelLinkBlocksConnIds[i];
                // establish the connection
                levelLinkBlocks[i].connectingPlatform = listPlatformMap[platformId];
                levelLinkBlocks[i].connectingPlatform.addIncomingConnectingLink(levelLinkBlocks[i]);
                levelLinkBlocks[i].renderArrow = true;
            }
        }

        // update the win conditions for the objective blocks
        gameController.updateObjectiveHUDAndBlocks();
        gameController.updatePlatformEntities();
    }

    /**
     * Reset the level, rebuilding the level that is in the levelDescriptionFile value.
     */
    public void resetLevel()
    {
        foreach (LinkBlockBehavior lb in levelLinkBlocks)
        {
            if (lb.linkArrow != null)
            {
                Destroy(lb.linkArrow.gameObject);
                Destroy(lb.linkArrowHead.gameObject);
                lb.linkArrow = null;
            }
        }

        foreach (Transform t in levelEntities)
        {
            // delete any arrows associated with link blocks.
            if (t.GetComponent<HelicopterRobotBehavior>() != null && t.GetComponent<HelicopterRobotBehavior>().childLink.GetComponent<LinkBlockBehavior>() != null)
            {
                if (t.GetComponent<HelicopterRobotBehavior>().childLink.GetComponent<LinkBlockBehavior>().linkArrow != null)
                {
                    Destroy(t.GetComponent<HelicopterRobotBehavior>().childLink.GetComponent<LinkBlockBehavior>().linkArrow.gameObject);
                    Destroy(t.GetComponent<HelicopterRobotBehavior>().childLink.GetComponent<LinkBlockBehavior>().linkArrowHead.gameObject);
                }
            } 
            Destroy(t.gameObject);
        }
        gameController.clearReferenceLists(); // clear the references here
        levelEntities.Clear();

        // make sure there is a level file for this
        if (levelFileIndex < levelDescriptionJsonFiles.Length)
        {
            gameController.setStatusText("");
            CreateWorldFromLevelDescription();
        } else
        {
            gameController.setStatusText("You have won!");
            /// .Log("GAME IS WON!");
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
