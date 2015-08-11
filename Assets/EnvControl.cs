using UnityEngine;
using System.Collections;

public class EnvControl : MonoBehaviour {
	public GameObject Classroom;
	public GameObject OutDoors;
	private bool classroomActive = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.T)){
			classroomActive = !classroomActive;
			Classroom.SetActive(classroomActive);
		}
	}
}
