using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.WorldGeneration;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;

/**
 * A left and right button used to map through various frames in logging visualizaton
 */
public class DataVisBehavior : MonoBehaviour {


public GameController gameController;




  private Button leftButton;
  private Button rightButton;

  private bool busy;




	void Start () {


    }

     void Update()
    {


        if(Input.GetKey("left") && !busy )
        {
            mapLeft();
        }

        if(Input.GetKey("right") && !busy)
        {
            mapRight();
        }
    }
    void mapLeft()
    {

        busy = true;
        gameController.worldGenerator.startDataVis("","",-1);
        System.Threading.Thread.Sleep(60);
        busy = false;





    }


    void mapRight()
    {

      busy = true;
      gameController.worldGenerator.startDataVis("","",1);
      System.Threading.Thread.Sleep(60);
      busy = false;


    }

    /*   public void dataVisFreeze()
 {
     rb2.constraints = RigidbodyConstraints2D.FreezeAll;
 }
 */

}
