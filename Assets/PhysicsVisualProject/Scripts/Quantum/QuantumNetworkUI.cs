using UnityEngine;
using System.Collections;
using JsonFx;
using CardboardControlDelegates;
using FMG;
using UnityEngine.UI;

public class QuantumNetworkUI : MonoBehaviour {

	public GameObject startCam;
	public GameObject mainMenu;
	public GameObject HostMenu;
	public GameObject ClientMenu;
	public GameObject ExitMenu;

	private int HostPortNum = 25000;
	private string HostStr = "127.0.0.1";
	private int ClientPortNum = 25000;

	public GameObject HostPort;
	public GameObject ClientHost;
	public GameObject ClientPort;

	private bool _isServer = false;
	private int _portNumber = 25000;
	private string _hostString = "127.0.0.1";
	private bool _isConnected = false;

	public QuantumWorldManager WorldManager;

	QuantumTcp myTCP;
	QuantumTcpServer myTcpServer;

	private bool _setupSocket = false;

	// Use this for initialization
	void Start () {
		if(WorldManager.UseCardBoard){
//			CardboardOnGUI.onGUICallback += this.OnGUI;
		}
	}
	// Update is called once per frame
	void Update () {
		OnEscapeButton();
	}
	
	void OnDestroy() {
		if (WorldManager.UseCardBoard){
//			CardboardOnGUI.onGUICallback -= this.OnGUI;
		}
		// ...
	}
	
	/*void OnGUI ()
	//void CardboardOnGUI()
	{
		//Debug.Log(CardboardOnGUI.OKToDraw(this));
		//if (!CardboardOnGUI.OKToDraw(this)) return;
		GUILayout.BeginVertical ();
		string s_c_string = _isServer ? "Switch to Client Mode" : "Switch to Host Mode";
		if(!_isConnected){
			if (GUILayout.Button (s_c_string)) {
				_isServer = !_isServer;
				Update();
			}
		}

		if (!_isServer) {

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Host");
			_hostString = GUILayout.TextField (_hostString);
			GUILayout.EndHorizontal ();
			Update();
		}

		if (!_isConnected) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Port");
			_portNumber = int.TryParse (GUILayout.TextField (_portNumber.ToString ()), out _portNumber) ? _portNumber : 25000;
			GUILayout.EndHorizontal ();
			Update();
		}

		string connectButton = _isConnected ? "Disconnect" : "Connect";

		if (GUILayout.Button (connectButton)) {
			if (_isConnected) {
				CloseConnection ();
			} else {
				StartConnection ();
			}
			Update();
		}

		if (_isServer && _isConnected) {
			SetupTcpServer();
		}


		GUILayout.EndVertical ();
	}
	*/

	private void StartConnection ()
	{
		if (_isServer) {
			Network.InitializeServer (99, _portNumber, true);
			InitWorld ();
			WorldManager.SetIsoControl(true);
		} else {
			Network.Connect (_hostString, _portNumber, "");
		}
		_isConnected = true;
	}
	
	void InitWorld ()
	{
		if (WorldManager) {
			WorldManager.Init ();
			WorldManager.LateInit ();
		}
	}
	
	void OnConnectedToServer ()
	{
		InitWorld ();
	}
	
	
	void OnPlayerDisconnected (NetworkPlayer player)
	{
		Network.RemoveRPCs (player);
		Network.DestroyPlayerObjects (player);
	}
	
	private void CloseConnection ()
	{
		if (_isServer) {
			RemoveTCPServer();
			NetworkView NetView = gameObject.GetComponent<NetworkView> ();
			NetView.RPC ("RpcHostCloseConnection", RPCMode.AllBuffered);

			Network.RemoveRPCsInGroup (0);
			Network.Disconnect ();
		} 
		else {
			WorldManager.ResetPlayer ();
			_isConnected = false;
			Network.Disconnect ();
		}
	}
	
	[RPC]
	void RpcHostCloseConnection(){
		WorldManager.ResetPlayer ();
		_isConnected = false;
	}
	/*
	void SetupTCP(){
		if (myTCP == null) {
			myTCP = (QuantumTcp)gameObject.AddComponent<QuantumTcp> ();
			myTCP.InitListener ();
			myTCP.MessageReceived +=WorldManager.OnMessageReceived;
		}
	}

	void RemoveTCP(){
		if (gameObject.GetComponent<QuantumTcp> ()) {
			gameObject.GetComponent<QuantumTcp> ().closeSocket();
			Destroy (gameObject.GetComponent<QuantumTcp> ());
		}
	}
	*/

	void RemoveTCPServer(){
		if (gameObject.GetComponent<QuantumTcpServer> ()) {
			gameObject.GetComponent<QuantumTcpServer> ().CloseServer();
			Destroy (gameObject.GetComponent<QuantumTcpServer> ());
		}
	}

	void SetupTcpServer()
	{
		if (myTcpServer == null) {
			myTcpServer = (QuantumTcpServer)gameObject.AddComponent<QuantumTcpServer>();
			myTcpServer.InitListener();
			myTcpServer.MessageReceived +=WorldManager.OnMessageReceived;
		}
	}

	public bool getIsServer(){
		return _isServer;
	}

	public void onCommand(string str)
	{
		if(str.Equals("HostMode"))
		{
			MenuConstants.fadeInFadeOut(HostMenu,mainMenu);
			mainMenu.SetActive(false);
		}
		
		if(str.Equals("HostBack"))
		{
			//mainMenu.SetActive(true);
			MenuConstants.fadeInFadeOut(mainMenu,HostMenu);	
			HostMenu.SetActive(false);
		}

		if(str.Equals("HostStart")){
			if(startCam){
				startCam.SetActive(false);
			}
			MenuConstants.slideOut(HostMenu,false);
			_portNumber = HostPortNum;
			_isServer = true;
			_isConnected =true;
			StartConnection();
			SetupTcpServer();

			HostMenu.SetActive(false);
		}

		if(str.Equals("ClientStart")){
			if(startCam){
				startCam.SetActive(false);
			}
			MenuConstants.slideOut(ClientMenu,false);

			_portNumber = HostPortNum;
			_hostString = HostStr;
			_isServer = false;
			_isConnected =true;
			StartConnection();

			ClientMenu.SetActive(false);
		}

		if(str.Equals("Exit"))
		{
			if(_isConnected){
				CloseConnection();
			}
			Application.Quit();
		}

		if(str.Equals("ExitBack")){
			Cardboard.SDK.VRModeEnabled = true;
			MenuConstants.slideOut(ExitMenu,false);
			ExitMenu.SetActive(false);
		}

		if(str.Equals("ClientMode"))
		{
			MenuConstants.fadeInFadeOut(ClientMenu,mainMenu);
			mainMenu.SetActive(false);
			
		}
		if(str.Equals("ClientBack"))
		{
			MenuConstants.fadeInFadeOut(mainMenu,ClientMenu);
			ClientMenu.SetActive(false);
		}

		if(str.Equals("HostPort")){
			HostPortNum = int.Parse(HostPort.GetComponent<InputField>().text);
		}

		if(str.Equals("ClientPort")){
			ClientPortNum = int.Parse(HostPort.GetComponent<InputField>().text);
		}

		if(str.Equals("ClientHost")){
			HostStr = ClientHost.GetComponent<InputField>().text;
		}
	}


	void OnEscapeButton(){
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(startCam.activeSelf){
				return;
			}
			else{
				Cardboard.SDK.VRModeEnabled = false;
				ExitMenu.SetActive(true);
				MenuConstants.slideOut(ExitMenu,true);
			}
		}
	}
}
