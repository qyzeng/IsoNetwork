using UnityEngine;
using System.Collections;

public class QuantumObjs : MonoBehaviour {

	public GameObject isoSurfacePre;
	public GameObject RoomTopPre;
	public GameObject RoomFloorPre;
	public GameObject RoomWallPre;

	GameObject isoSurface;
	GameObject RoomTop;
	GameObject RoomFloor;
	GameObject RoomWall1;
	GameObject RoomWall2;
	GameObject RoomWall3;
	GameObject RoomWall4;

	// Use this for initialization
	void Start () {

		isoSurface = (GameObject)GameObject.Instantiate (isoSurfacePre) as GameObject;
		isoSurface.gameObject.name = "isoSurface";
		/*
		RoomTop = (GameObject)GameObject.Instantiate (RoomTopPre) as GameObject;
		RoomFloor = (GameObject)GameObject.Instantiate (RoomFloorPre) as GameObject;

		RoomWall1 = (GameObject)GameObject.Instantiate (RoomWallPre) as GameObject;
		RoomWall2 = (GameObject)GameObject.Instantiate (RoomWallPre) as GameObject;
		RoomWall3 = (GameObject)GameObject.Instantiate (RoomWallPre) as GameObject;
		RoomWall4 = (GameObject)GameObject.Instantiate (RoomWallPre) as GameObject;

		MakeRoom (new Vector3 (0f, 0f, 0f), 20f, new Vector3 (5f, 2f, 5f));
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject GetIsoSurface()
	{
		return isoSurface;
	}

	public void MakeRoom(Vector3 boundCenter,float boundExtent,Vector3 Scales){

		var RoomTopCenter = boundCenter+ new Vector3(0,Scales.y*boundExtent/2f,0);
		var RoomTopSize = new Vector3(Scales.x*boundExtent,1,Scales.z*boundExtent);
		RoomTop.transform.position = RoomTopCenter;
		RoomTop.transform.localScale = RoomTopSize;
		
		var RoomFloorCenter = boundCenter- new Vector3(0,Scales.y*boundExtent/2f,0);
		var RoomFloorSize = RoomTopSize;
		RoomFloor.transform.position = RoomFloorCenter;
		RoomFloor.transform.localScale = RoomFloorSize;

		var RoomWallCenter1 = boundCenter+ new Vector3(Scales.x*boundExtent/2f,0,0);
		var RoomWallSize1 = new Vector3(1,Scales.y*boundExtent,Scales.z*boundExtent);
		RoomWall1.transform.position = RoomWallCenter1;
		RoomWall1.transform.localScale = RoomWallSize1;
		
		var RoomWallCenter2 = boundCenter- new Vector3(Scales.x*boundExtent/2f,0,0);
		var RoomWallSize2 = new Vector3(1,Scales.y*boundExtent,Scales.z*boundExtent);
		RoomWall2.transform.position = RoomWallCenter2;
		RoomWall2.transform.localScale = RoomWallSize2;

		var RoomWallCenter3 = boundCenter + new Vector3(0,0,Scales.z*boundExtent/2f);
		var RoomWallSize3 = new Vector3(Scales.x*boundExtent,Scales.y*boundExtent,1);
		RoomWall3.transform.position = RoomWallCenter3;
		RoomWall3.transform.localScale = RoomWallSize3;
		
		var RoomWallCenter4 = boundCenter - new Vector3(0,0,Scales.z*boundExtent/2f);
		var RoomWallSize4 = new Vector3(Scales.x*boundExtent,Scales.y*boundExtent,1);
		RoomWall4.transform.position = RoomWallCenter4;
		RoomWall4.transform.localScale = RoomWallSize4;
	}
}
