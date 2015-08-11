﻿using UnityEngine;
using System.Collections;

public class ExampleCharacterController : MonoBehaviour {
  private static CardboardControl cardboard;

  void Start () {
    /*
    * Start by capturing the helper script on CardboardControlManager
    * This script has access to all the controls and their delegates
    *
    * Unity provides a good primer on delegates here:
    * http://unity3d.com/learn/tutorials/modules/intermediate/scripting/delegates
    */
    cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();
    cardboard.magnet.OnDown += CardboardDown;  // When the magnet goes down
    cardboard.magnet.OnUp += CardboardUp;      // When the magnet comes back up
	

    // When the magnet goes down and up within the "click threshold" time
    // That click speed threshold is configurable in the inspector
    cardboard.magnet.OnClick += CardboardClick;

    // When the thing we're looking at changes, determined by a gaze
    // The gaze distance and layer mask are public as configurable in the inspector
    //cardboard.gaze.OnChange += CardboardFocus;

    // Not used here is the OnTilt delegate
    // This is triggered on rotating the device to Portrait mode
    // cardboard.box.OnTilt += ...
  }



  /*
  * In this demo, we randomize object colours for triggered events
  */
  public void CardboardDown(object sender) {
    Debug.Log("Magnet went down");
    ChangeObjectColor("SphereDown");
  }

  public void CardboardUp(object sender) {
    Debug.Log("Magnet came up");
    ChangeObjectColor("SphereUp");
  }

  public void CardboardClick(object sender) {
    ChangeObjectColor("SphereClick");

    TextMesh textMesh = GameObject.Find("SphereClick/Counter").GetComponent<TextMesh>();
    int increment = int.Parse(textMesh.text) + 1;
    textMesh.text = increment.ToString();

    // With the cardboard object, we can grab information from various controls
    // If the raycast doesn't find anything then the focused object will be null
    //string name = cardboard.gaze.Object() == null ? "nothing" : cardboard.gaze.Object().name;
   // float count = cardboard.gaze.SecondsHeld();
   // Debug.Log("We've focused on "+name+" for "+count+" seconds.");
    
    // If you need more raycast data from cardboard.gaze, the RaycastHit is exposed as gaze.Hit
  }

 // public void CardboardFocus(object sender) {
    // For more event-driven code, you can grab the data from the sender
 //   CardboardControlGaze gaze = sender as CardboardControlGaze;
    // gaze.IsHeld will make sure the gaze.Object isn't null
 //   if (gaze.IsHeld()) {
  //    ChangeObjectColor(gaze.Object().name);
  //  }
 // }

  public void ChangeObjectColor(string name) {
    GameObject obj = GameObject.Find(name);
    Color newColor = new Color(Random.value, Random.value, Random.value);
    obj.GetComponent<Renderer>().material.color = newColor;
  }



  /*
  * During our game we can utilize data from the CardboardControl API
  */
  void Update() {
    TextMesh textMesh = GameObject.Find("SphereDown/Counter").GetComponent<TextMesh>();

    // magnet.IsHeld is true when the magnet has gone down but not back up yet
    if (cardboard.magnet.IsHeld()) {
      textMesh.GetComponent<Renderer>().enabled = true;
      // magnet.SecondsHeld is the number of seconds we've held the magnet down
      textMesh.text = cardboard.magnet.SecondsHeld().ToString("#.##");
    }
    else {
      textMesh.GetComponent<Renderer>().enabled = Time.time % 1 < 0.5;
    }
  }

  /*
  * Be sure to unsubscribe before this object is destroyed
  * so the garbage collector can clean everything up
  */
  void OnDestroy() {
    cardboard.magnet.OnDown -= CardboardDown;
    cardboard.magnet.OnUp -= CardboardUp;
    cardboard.magnet.OnClick -= CardboardClick;
   // cardboard.gaze.OnChange -= CardboardFocus;
  }
}
