using UnityEngine;
using System.Collections; 
using System; 
using System.IO; 
using System.Net.Sockets;

public class QuantumTcp : MonoBehaviour {

	public delegate void MessageReceivedHandler (string message);
	public event MessageReceivedHandler MessageReceived;

	internal Boolean socketReady = false;
	
	TcpListener mListener;
	TcpClient mClient;
	NetworkStream mClientStream;
	StreamWriter mClientWriter;
	StreamReader mClientReader;
	String Host = "localhost";
	int ClientPort = 51111;
	int ServerPort = 52111;

	System.Collections.Generic.List<Socket> mSocketList = new System.Collections.Generic.List<Socket> ();

	string SocketMessage;

	void SetHost(string host){
		Host = host;
	}

	void SetClientPort(int mclientport){
		ClientPort = mclientport;
	}

	void SetServerPort(int mserverport){
		ServerPort = mserverport;
	}

	public void InitListener(){
		mListener = new TcpListener (System.Net.IPAddress.Loopback, ServerPort);
		mListener.Start ();
		mClient = mListener.AcceptTcpClient ();
	}

	// Use this for initialization
	void Start () {
		//InitListener ();
	}
	
	// Update is called once per frame
	void Update () {

		if (mListener != null) {
			if (mListener.Pending ()) {
				Socket socket = mListener.AcceptSocket ();
				mSocketList.Add (socket);
			}
			
			Socket[] currentSocketList = mSocketList.ToArray ();
			
			foreach (Socket socket in currentSocketList) {
				if (!socket.Connected) {
					socket.Disconnect (true);
					socket.Close ();
					mSocketList.Remove (socket);
				}
			}
		}
		
		SocketMessage = readSocket ();
		if (!string.IsNullOrEmpty (SocketMessage)) {
			if (MessageReceived != null) {
				MessageReceived (SocketMessage);
			}
		}
	
	}

	public void setupSocket ()
	{
		try {
			mClient = new TcpClient (Host, ClientPort);
			mClientStream = mClient.GetStream (); 
			mClientStream.ReadTimeout = 1;
			mClientWriter = new StreamWriter (mClientStream); 
			mClientReader = new StreamReader (mClientStream); 
			socketReady = true;
		} catch (Exception e) {
			Debug.Log ("Socket error: " + e); 
		}
	}
	
	public void writeSocket (string theLine)
	{ 
		if (!socketReady) 
			return;
		String foo = theLine + "\r\n";
		mClientWriter.Write (foo);
		mClientWriter.Flush ();
	} 
	
	public String readSocket ()
	{
		if (!socketReady) 
			return "";
		try {
			return mClientReader.ReadLine ();
			//return mClientReader.ReadToEnd (); // use ReadToEnd if data is in json format;
		} catch (Exception e) {
			return "";
		}
	}
	
	public void closeSocket ()
	{
		if (!socketReady)
			return; 
		mClientWriter.Close ();
		mClientReader.Close ();
		mClient.Close ();
		socketReady = false;
	}

	void OnStart()
	{
		mListener.Start ();
	}

	void OnDestroy ()
	{
		mListener.Stop ();
	}
	
	void OnApplicationQuit ()
	{
		mListener.Stop ();
	}
}