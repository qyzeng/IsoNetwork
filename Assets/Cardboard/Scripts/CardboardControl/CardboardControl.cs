using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using CardboardControlDelegates;

/**
* Bring all the control scripts together to provide a convenient API
*/
public class CardboardControl : MonoBehaviour {
  [HideInInspector]
  public CardboardControlMagnet magnet;

  public bool debugChartsEnabled = false;

  public void Awake() {
    magnet = gameObject.GetComponent<CardboardControlMagnet>();
  }

  public void Update() {
    if (debugChartsEnabled) PrintDebugCharts();
  }

  public void PrintDebugCharts() {
    Debug.Log(magnet.SensorChart());
    Debug.Log(magnet.StateChart());
  }
}
