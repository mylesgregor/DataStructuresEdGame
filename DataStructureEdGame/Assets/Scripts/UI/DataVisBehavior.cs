using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.WorldGeneration;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/**
 * A left and right button used to map through various frames in logging visualizaton
 */
public class DataVisBehavior : MonoBehaviour {


public GameController gameController;




  private Button leftButton;
  private Button rightButton;
 


	void Start () {



        leftButton = transform.Find("Left").GetComponent<Button>();
        rightButton = transform.Find("Right").GetComponent<Button>();

        leftButton.onClick.AddListener(mapLeft);
        rightButton.onClick.AddListener(mapRight);


    }

    void mapLeft()
    {

        gameController.worldGenerator.startDataVis("NULL","NULL",-1);

    }

    void mapRight()
    {
      gameController.worldGenerator.startDataVis("NULL","NULL",1);
    }

    /*   public void dataVisFreeze()
 {
     rb2.constraints = RigidbodyConstraints2D.FreezeAll;
 }
 */

}
