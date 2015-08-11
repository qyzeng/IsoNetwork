using UnityEngine;
using System.Collections;
using CardboardControlDelegates;

/**
* Creating a vision raycast and handling the data from it
* Relies on Google Cardboard SDK API's
*/
public class CardboardControlMagnet : MonoBehaviour {  
  public float clickSpeedThreshold = 0.4f;
  public bool vibrateOnDown = false;
	public bool vibrateOnUp = false;
	public bool vibrateOnClick = false;
  public KeyCode magnetKey = KeyCode.Space;

  private ParsedSensorData sensor;
  private enum MagnetState { Up, Down }
  private MagnetState currentMagnetState = MagnetState.Up;
  private float clickStartTime = 0f;

  public CardboardControlDelegate OnUp = delegate {};
  public CardboardControlDelegate OnDown = delegate {};
  public CardboardControlDelegate OnClick = delegate {};

  public void Start() {
    sensor = new ParsedSensorData();
  }
  
  public void Update() {
    sensor.Update();
    CheckMagnet();
  }

  private bool KeyFor(string direction) {
    switch(direction) {
      case "down":
        return Input.GetKeyDown(magnetKey);
      case "up":
        return Input.GetKeyUp(magnetKey);
      default:
        return false;
    }
  }

  private void CheckMagnet() {
    if (!sensor.IsCalibrating() && !sensor.IsJostled() && !sensor.IsRotatedQuickly()) {
      if (sensor.IsDown() || KeyFor("down")) ReportDown();
      if (sensor.IsUp() || KeyFor("up")) ReportUp();
    }
  }
  
  private void ReportDown() {
    if (currentMagnetState == MagnetState.Up) {
      currentMagnetState = MagnetState.Down;
      OnDown(this);
#if UNITY_ANDROID && !UNITY_EDITOR
      if (vibrateOnDown) Handheld.Vibrate();
#endif
      clickStartTime = Time.time;
    }
  }
  
  private void ReportUp() {
    if (currentMagnetState == MagnetState.Down) {
      currentMagnetState = MagnetState.Up;
      OnUp(this);
#if UNITY_ANDROID && !UNITY_EDITOR
	  if (vibrateOnUp) Handheld.Vibrate();
#endif
      CheckForClick();
    }
  }
  
  private void CheckForClick() {
    bool withinClickThreshold = SecondsHeld() <= clickSpeedThreshold;
    clickStartTime = 0f;
    if (withinClickThreshold) ReportClick();
  }

  private void ReportClick() {
    OnClick(this);
#if UNITY_ANDROID && !UNITY_EDITOR
    if (vibrateOnClick) Handheld.Vibrate();
#endif
  }

  public float SecondsHeld() {
    return Time.time - clickStartTime;
  }

  public bool IsHeld() {
    return (currentMagnetState == MagnetState.Down);
  }

  public string SensorChart() {
    string chart = "";
    chart += "Sensor Readings\n";
    chart += !sensor.IsJostled() ? "***** steady " : "!!!!! jostled ";
    if (!sensor.IsJostled()) {
      chart += sensor.IsDown() ? "vvv " : "    ";
      chart += sensor.IsUp() ? "^^^ " : "    ";
    }
    return chart;
  }

  public string StateChart() {
    string chart = "";
    chart += "Magnet State\n";
    chart += (IsHeld()) ? "U " : "x ";
    chart += (!IsHeld()) ? "D " : "x ";
    chart += "\n";
    return chart;
  }
}
