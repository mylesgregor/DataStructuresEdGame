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



       // leftButton = transform.Find("Left").GetComponent<Button>();
       // rightButton = transform.Find("Right").GetComponent<Button>();

      //  leftButton.onClick.AddListener(mapLeft);

      //  rightButton.onClick.AddListener(mapRight);


    }

     void Update()
    {
        if(Input.GetKey("left"))
        {
            mapLeft();
        }

        if(Input.GetKey("right"))
        {
            mapRight();
        }
    }
    void mapLeft()
    {

        gameController.worldGenerator.startDataVis("no","no",-1);
        System.Threading.Thread.Sleep(100);
       
      
      
    }


    void mapRight()
    {
      gameController.worldGenerator.startDataVis("no","no",1);
        System.Threading.Thread.Sleep(100);
    }

    /*   public void dataVisFreeze()
 {
     rb2.constraints = RigidbodyConstraints2D.FreezeAll;
 }
 */

}
