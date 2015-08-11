using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class QuantumTcpServer : MonoBehaviour {

	public delegate void MessageReceivedHandler (string message);
	public event MessageReceivedHandler MessageReceived;
	
	internal Boolean ServerReady = false;
	
	TcpListener mListener;
	TcpClient mClient;
	NetworkStream mClientStream;
	StreamWriter mClientWriter;
	StreamReader mClientReader;

	int ServerPort = 53111;
	string mMessage;
	
	void SetServerPort(int mserverport){
		ServerPort = mserverport;
	}

	public void InitListener(){
		mListener = new TcpListener (System.Net.IPAddress.Loopback, ServerPort);
		mListener.Start ();
		ServerReady = true;

	}

	public void WriteToClient (string theLine)
	{ 
		if (!ServerReady) 
			return;
		String foo = theLine + "\r\n";
		mClientWriter.Write (foo);
		mClientWriter.Flush ();
	} 
	
	public String ReadFromClient ()
	{
		if (!ServerReady) 
			return "";
		try {
			return mClientReader.ReadLine ();
			//return mClientReader.ReadToEnd (); // use ReadToEnd if data is in json format;
		} catch (Exception e) {
			return "";
		}
	}
	
	public void CloseServer ()
	{
		if (!ServerReady)
			return; 
		//mClientWriter.Close ();
		//mClientReader.Close ();
		//mClient.Close ();
		mListener.Stop ();
		ServerReady = false;
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

	// Use this for initialization
	void Start () {
		//InitListener ();
	}
	
	// Update is called once per frame
	void Update () {

		if (mListener != null) {
			if (mListener.Pending()){
				mClient = mListener.AcceptTcpClient();
				mClientStream = mClient.GetStream();
				if (mClientStream!=null){
					mClientReader = new StreamReader (mClientStream);
					mClientWriter = new StreamWriter (mClientStream);
					mMessage = ReadFromClient();
					//Debug.Log(mMessage);

					if (!string.IsNullOrEmpty (mMessage)) {
						if (MessageReceived != null) {
							MessageReceived (mMessage);
							//Debug.Log(mMessage+"received");
						}
					}
				}
			}
		}
	}
}