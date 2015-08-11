using UnityEngine;
using System.Collections;
using System.Timers;
using WP.Character;
using WP.Controller;

public class QuantumWorldManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		mObjs = (QuantumObjs)gameObject.GetComponent<QuantumObjs> ();
		myNet = gameObject.GetComponent<NetworkView>();
	}

	// Update is called once per frame
	void Update () {
		ControlIsoSurface();

		bool IsoClientControl = Input.GetKeyDown (KeyCode.C);
		if (GetComponent<QuantumNetworkUI>().getIsServer() && IsoClientControl) {
			myNet.RPC("SetIsoCon",RPCMode.OthersBuffered);
		}
	}
	void ControlIsoSurface(){
		bool ZoomIn = Input.GetKey (KeyCode.I);
		bool ZoomOut = Input.GetKey (KeyCode.O);
		bool RotateXp = Input.GetKey (KeyCode.Y);
		bool RotateXn = Input.GetKey (KeyCode.U);
		bool RotateYp = Input.GetKey (KeyCode.H);
		bool RotateYn = Input.GetKey (KeyCode.J);
		bool RotateZp = Input.GetKey (KeyCode.N);
		bool RotateZn = Input.GetKey (KeyCode.M);
		if (IsoControl) {
			if (ZoomIn) {
				//var localScale = myIsoSurface.transform.localScale;
				var localScale = mObjs.GetIsoSurface ().transform.localScale;
				localScale += localScale*0.01f;
				myNet.RPC ("ObjTransformZoom", RPCMode.AllBuffered, "isoSurface", localScale);
			}
			if (ZoomOut) {
				//var localScale = myIsoSurface.transform.localScale;
				var localScale = mObjs.GetIsoSurface ().transform.localScale;
				localScale -= localScale*0.01f;//new Vector3 (0.01f, 0.01f, 0.01f); 
				myNet.RPC ("ObjTransformZoom", RPCMode.AllBuffered, "isoSurface", localScale);
			}
			if (RotateXp) {
				var RotateAngle = new Vector3 (RotateSpeed * Time.deltaTime, 0, 0);
				myNet.RPC ("ObjRotate", RPCMode.AllBuffered, "isoSurface", RotateAngle);
			}
			if (RotateXn) {
				var RotateAngle = new Vector3 (-(RotateSpeed * Time.deltaTime), 0, 0);
				myNet.RPC ("ObjRotate", RPCMode.AllBuffered, "isoSurface", RotateAngle);
			}
			
			if (RotateYp) {
				var RotateAngle = new Vector3 (0, (RotateSpeed * Time.deltaTime), 0);
				myNet.RPC ("ObjRotate", RPCMode.AllBuffered, "isoSurface", RotateAngle);
			}
			if (RotateYn) {
				var RotateAngle = new Vector3 (0, -(RotateSpeed * Time.deltaTime), 0);
				myNet.RPC ("ObjRotate", RPCMode.AllBuffered, "isoSurface", RotateAngle);
			}
			if (RotateZp) {
				var RotateAngle = new Vector3 (0, 0, (RotateSpeed * Time.deltaTime));
				myNet.RPC ("ObjRotate", RPCMode.AllBuffered, "isoSurface", RotateAngle);
			}
			if (RotateZn) {
				var RotateAngle = new Vector3 (0, 0, -(RotateSpeed * Time.deltaTime));
				myNet.RPC ("ObjRotate", RPCMode.AllBuffered, "isoSurface", RotateAngle);
			}
		}

		bool IsoXp = Input.GetKey (KeyCode.W);
		bool IsoXn = Input.GetKey (KeyCode.S);
		bool IsoYp = Input.GetKey (KeyCode.Z);
		bool IsoYn = Input.GetKey (KeyCode.X);
		bool IsoZp = Input.GetKey (KeyCode.A);
		bool IsoZn = Input.GetKey (KeyCode.D);
		
		if (IsoXp) {
			var ObjMotion = new Vector3 (1, 0, 0);
			var objPosistion = mObjs.GetIsoSurface ().transform.localPosition;
			objPosistion += ObjMotion*Time.deltaTime;
			myNet.RPC ("MoveObj", RPCMode.AllBuffered, "isoSurface", objPosistion);
			//myNet.RPC ("MoveObj", RPCMode.AllBuffered, "isoSurface", ObjMotion);
		}
		if (IsoXn) {
			var ObjMotion = new Vector3 (-1, 0, 0);
			var objPosistion = mObjs.GetIsoSurface ().transform.localPosition;
			objPosistion += ObjMotion*Time.deltaTime;
			myNet.RPC ("MoveObj", RPCMode.AllBuffered, "isoSurface", objPosistion);
			//myNet.RPC ("MoveObj", RPCMode.AllBuffered, "isoSurface", ObjMotion);
		}
		if (IsoYp) {
			var ObjMotion = new Vector3 (0, 1, 0);
			var objPosistion = mObjs.GetIsoSurface ().transform.localPosition;
			objPosistion += ObjMotion*Time.deltaTime;
			myNet.RPC ("MoveObj", RPCMode.AllBuffered, "isoSurface", objPosistion);
			//myNet.RPC ("MoveObj", RPCMode.AllBuffered, "isoSurface", ObjMotion);
		}
		if (IsoYn) {
			var ObjMotion = new Vector3 (0, -1, 0);
			var objPosistion = mObjs.GetIsoSurface ().transform.localPosition;
			objPosistion += ObjMotion*Time.deltaTime;
			myNet.RPC ("MoveObj", RPCMode.AllBuffered, "isoSurface", objPosistion);
			//myNet.RPC ("MoveObj", RPCMode.AllBuffered, "isoSurface", ObjMotion);
		}
		if (IsoZp) {
			var ObjMotion = new Vector3 (0, 0, 1);
			var objPosistion = mObjs.GetIsoSurface ().transform.localPosition;
			objPosistion += ObjMotion*Time.deltaTime;
			myNet.RPC ("MoveObj", RPCMode.AllBuffered, "isoSurface", objPosistion);
			//myNet.RPC ("MoveObj", RPCMode.AllBuffered, "isoSurface", ObjMotion);
		}
		if (IsoZn) {
			var ObjMotion = new Vector3 (0, 0, -1);
			var objPosistion = mObjs.GetIsoSurface ().transform.localPosition;
			objPosistion += ObjMotion*Time.deltaTime;
			myNet.RPC ("MoveObj", RPCMode.AllBuffered, "isoSurface", objPosistion);
			//myNet.RPC ("MoveObj", RPCMode.AllBuffered, "isoSurface", ObjMotion);
		}
	}

	QuantumObjs mObjs;
	NetworkView myNet;
	public float RotateSpeed = 45.0f;

	private bool IsoControl = false;

	public void SetIsoControl(bool iscontrol){
		IsoControl = iscontrol;
	}

	public void ChangeIsoControl(){
		IsoControl = !IsoControl;
	}

	[RPC]
	void SetIsoCon(){
		ChangeIsoControl();
	}
	
	protected CharacterStateMachine _playerChar;
	public CharacterStateMachine PlayerChar {
		get {
			return _playerChar;
		}
	}
	
	public delegate void PlayerCharacterReadyHandler ();
	public event PlayerCharacterReadyHandler OnPlayerReady;
	
	public GameObject ReferenceModel;
	public GameObject ReferencePlayerObject;
	
	public CameraControl CamControl;
	public CameraControl CarBoardCamControl;
	[SerializeField]
	protected CameraControl
		_referenceCamControlToUse;
	
	protected CameraControl _currentCamControl;

	public CameraControl currentCamControl{
		get{
			return _currentCamControl;
		}
	}
	
	public Vector3 PlayerSpawnPoint;
	
	
	private Transform _worldCenter = null;
	public Transform WorldCenter {
		get {
			if (_worldCenter == null) {
				_worldCenter = GameObject.Find ("WorldCenter").transform;
				if (_worldCenter == null) {
					_worldCenter = new GameObject ("WorldCenter").transform;
					_worldCenter.position = Vector3.zero;
				}
			}
			return _worldCenter;
		}
	}
	
	public bool UseCardBoard {
		get {
			return _UseCardBoard;
		}
		set {
			if (_UseCardBoard != value) {
				_UseCardBoard = value;
				VerifyUseCardBoard ();
			}
		}
	}
	[SerializeField]
	protected bool
		_UseCardBoard = false;
	
	protected void VerifyUseCardBoard ()
	{
		if (_UseCardBoard) {
			_referenceCamControlToUse = CarBoardCamControl;
		} else {
			_referenceCamControlToUse = CamControl;
		}
	}
	
	#if UNITY_EDITOR
	protected virtual void OnValidate ()
	{
		VerifyUseCardBoard ();
	}
	#endif

	public void InitCharacter (CharacterStateMachine character)
	{
		if (OnPlayerReady != null) {
			OnPlayerReady ();
		}
	}
	
	public virtual void Init ()
	{
		if (myNet) {
			myNet.RPC("SetUseCardBoard",RPCMode.AllBuffered,_UseCardBoard);
		}

		PlayerSpawnPoint = new Vector3(Random.Range(-9,6),0f,Random.Range(-9,9));

		if (ReferencePlayerObject != null) {
			_playerChar = ((GameObject)Network.Instantiate (ReferencePlayerObject, PlayerSpawnPoint, Quaternion.identity, 0)).GetComponent<CharacterStateMachine> ();
			InitCharacter (_playerChar);
		}

		VerifyUseCardBoard ();
		if (_referenceCamControlToUse != null) {
			_currentCamControl = ((GameObject)GameObject.Instantiate (_referenceCamControlToUse.gameObject)).GetComponent<CameraControl> ();
		}
		//WP.Controller.ControllerUtility.OnControllerModeChanged += OnControllerModeChanged;
	}

	[RPC]
	void SetUseCardBoard(bool _CardBoard)
	{
		UseCardBoard = _CardBoard;
	}

	public virtual void LateInit ()
	{
		if (myNet) {
			myNet.RPC("RpcLateInit",RPCMode.AllBuffered,true);
		}
		/*
		if (_playerChar != null) {
			_playerChar.AddController (WP.Controller.StandalonePlayerController.Singleton);
		}
		if (_currentCamControl != null) {
			_currentCamControl.AddController (WP.Controller.StandalonePlayerController.Singleton);
			if (_playerChar)
				_currentCamControl.LookAtTarget = _playerChar.gameObject;
		}*/
	}

	[RPC]
	void RpcLateInit(bool a){
		if(UseCardBoard){
			if (_playerChar != null) {
				_playerChar.AddController(WP.Controller.CardBoardController.Singleton);
				//_playerChar.AddController (WP.Controller.StandalonePlayerController.Singleton);
			}
			if (_currentCamControl != null) {
				_currentCamControl.AddController(WP.Controller.CardBoardController.Singleton);
				//_currentCamControl.AddController (WP.Controller.StandalonePlayerController.Singleton);
				if (_playerChar)
					_currentCamControl.LookAtTarget = _playerChar.gameObject;
			}
		}
		else{
			if (_playerChar != null) {
				_playerChar.AddController (WP.Controller.StandalonePlayerController.Singleton);
			}
			if (_currentCamControl != null) {
				_currentCamControl.AddController (WP.Controller.StandalonePlayerController.Singleton);
				if (_playerChar)
					_currentCamControl.LookAtTarget = _playerChar.gameObject;
			}
		}
	}

	private void OnControllerModeChanged(WP.Controller.CONTROLLER_MODE mode)
	{
		if (mode == WP.Controller.CONTROLLER_MODE.NORMAL) {
			int stateC=1;
			myNet.RPC("SetPlayerStateInWorld",RPCMode.AllBuffered,stateC);

		} else {
			int stateC =2;
			myNet.RPC("SetPlayerStateInWorld",RPCMode.AllBuffered,stateC);
		}
	}

	[RPC]
	void SetPlayerStateInWorld(int stateC){
		if (_playerChar) {

			switch(stateC){
			case(1):
				_playerChar.SetState (CharacterState.FLY);
				break;
			case(2):
				_playerChar.SetState(CharacterState.IDLE);
				break;
			}
		}
	}

	private IEnumerator MakeMyMesh(string str)
	{
		string [] parts;
		parts = str.Split ('|');

		int N_vertices = int.Parse (parts [1]);
		int N_triangles = int.Parse (parts [2]);
		int Num = parts.GetLength (0);
		var my_vertices = new float[N_vertices * 3];
		
		for (int i=0; i<(N_vertices*3); i++) {
			my_vertices [i] = float.Parse (parts [i + 3]);//(float.TryParse (parts[i+3], out my_vertices[i]))? my_vertices[i] : 0f;
		}
		var my_triangles = new int[N_triangles * 3];
		
		for (int i =0; i<(N_triangles*3); i++) {
			my_triangles [i] = int.Parse (parts [i + 3 + N_vertices * 3]);
			//yield return null;
		}
		
		var my_phase = new float[N_vertices];
		float my_isovalue = float.Parse(parts[3+N_vertices * 3+N_triangles*3]);
		for(int i =0; i<N_vertices;i++){
			my_phase[i] = float.Parse(parts[i+3+N_vertices * 3+N_triangles*3+1]);
			//yield return null;
		}
		yield return null;

		myNet.RPC ("CreateIsoSurfaceMesh", RPCMode.AllBuffered,"isoSurface", my_vertices,my_triangles,my_isovalue,my_phase);

	}

	private IEnumerator MakeMyMeshWithoutPhase(string str)
	{
		string [] parts;
		parts = str.Split ('|');
		
		int N_vertices = int.Parse (parts [1]);
		int N_triangles = int.Parse (parts [2]);
		int Num = parts.GetLength (0);
		var my_vertices = new float[N_vertices * 3];
		
		for (int i=0; i<(N_vertices*3); i++) {
			my_vertices [i] = float.Parse (parts [i + 3]);//(float.TryParse (parts[i+3], out my_vertices[i]))? my_vertices[i] : 0f;
		}
		var my_triangles = new int[N_triangles * 3];
		
		for (int i =0; i<(N_triangles*3); i++) {
			my_triangles [i] = int.Parse (parts [i + 3 + N_vertices * 3]);
			//yield return null;
		}
		yield return null;
		
		myNet.RPC ("CreateIsoSurfaceMeshWithoutColor", RPCMode.AllBuffered,"isoSurface", my_vertices,my_triangles);
		
	}

	public virtual void ResetPlayer ()
	{
		if (_playerChar != null) {
			Network.Destroy (_playerChar.gameObject);
			Destroy(_playerChar.gameObject);
		}
		if (_currentCamControl != null) {
			Destroy (_currentCamControl.gameObject);
		}
	}

	public void OnMessageReceived(string mstring){
		char cmd = mstring[0];
		switch(cmd){
		case('R'):
			StartCoroutine(MakeMyMesh(mstring));
			break;
		case('W'):
			StartCoroutine(MakeMyMeshWithoutPhase(mstring));
			break;
		}
		//StartCoroutine(MakeMyMesh(mstring));

	}
}
