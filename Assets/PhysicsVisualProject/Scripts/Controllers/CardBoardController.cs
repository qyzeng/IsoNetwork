using UnityEngine;
using System.Collections;

namespace WP.Controller
{
	public class CardBoardController : BaseController<CardBoardController>
	{
		private static CardboardControl cardboardCon;
		private static CardboardControlMagnet cardboardMagnet;
		public bool IsMoving = false ;
		public float moveSpeed = 0.4f;
		private bool isVRMode =true;
		float horizontal =0f;
		float vertical =0f;

		void Start(){
			cardboardCon = GameObject.Find("QuantumWorld").GetComponent<QuantumWorldManager>().currentCamControl.GetComponent<CardboardControl>();
			cardboardMagnet = GameObject.Find("QuantumWorld").GetComponent<QuantumWorldManager>().currentCamControl.GetComponent<CardboardControlMagnet>();

			if(cardboardCon ){
				cardboardCon.magnet.OnDown += CardboardDown;  // When the magnet goes down
				cardboardCon.magnet.OnUp += CardboardUp;      // When the magnet comes back u
			}
		}

		public void CardboardDown(object sender) {
			SetIsMoving(true);
		}
		
		public void CardboardUp(object sender) {
			SetIsMoving(false);
		}

		void SetIsMoving(bool moveState){
			IsMoving = moveState;
		}

		private Vector2 GetInput()
		{
			Vector2 input = new Vector2
			{
				x = Input.GetAxis("Horizontal")*moveSpeed,
				y = Input.GetAxis("Vertical")*moveSpeed
			};
			// If GetAxis are empty, try alternate input methods.
			if (Mathf.Abs(input.x) + Mathf.Abs(input.y) < 2 * float.Epsilon)
			{
				//if (cardboardMagnet.IsHeld()) //IsMoving is the flag for forward movement. This is the bool that would be toggled by a click of the Google cardboard magnet

				if(IsMoving)
				{
					input = new Vector2(0, 1)*moveSpeed; // go straight forward by setting positive Vertical
				}
			}
			return input;
		}

		private void ReadCardBoardData()
		{
			//var rot = Cardboard.SDK.HeadPose.Orientation;
			var rot = Camera.main.transform;
			float camhorizontal = rot.eulerAngles.y;
			float camvertical = rot.eulerAngles.x;
			_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_HORIZONTAL, camhorizontal));
			_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_VERTICAL, camvertical));
		}

		private void ReadRawInput ()
		{
			CameraMouseControl();
			if(Camera.main){
				CommandFiredEventArgs playerRotate = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_ROTATION, Camera.main.transform.rotation);
				_commandList.Add (playerRotate);
			}

			horizontal = GetInput().x;
			vertical = GetInput().y;

			CommandFiredEventArgs playerHorizontal = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_HORIZONTAL, horizontal);
			CommandFiredEventArgs playerVertical = CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.PLAYER_VERTICAL, vertical);
			_commandList.Add (playerHorizontal);
			_commandList.Add (playerVertical);

		}

		private void CameraMouseControl(){
			float camhorizontal = 0f;
			if (Input.GetKey (KeyCode.Q)) {
				camhorizontal = -1f;
			} else if (Input.GetKey (KeyCode.E)) {
				camhorizontal = 1f;
			}
			float camvertical = 0f;
			if (Input.GetMouseButton (1)) {
				camhorizontal = Input.GetAxis ("Mouse X")*5f;
				camvertical = Input.GetAxis ("Mouse Y")*5f;
			}
			_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_HORIZONTAL, camhorizontal));
			_commandList.Add (CommandFiredEventArgs.GenerateArgs ((ushort)COMMAND_TYPE.CAMERA_VERTICAL, camvertical));
		}

		private void Update ()
		{
			ReadRawInput ();
			DispatchCommands ();

			if(Input.GetKeyDown(KeyCode.R)){
				isVRMode = !isVRMode;
				Cardboard.SDK.VRModeEnabled = isVRMode;
			}
		}

		protected override void OnDestroy(){
			base.OnDestroy();
			if(cardboardCon){
				cardboardCon.magnet.OnDown -= CardboardDown;
				cardboardCon.magnet.OnUp -= CardboardUp;
			}
		}
	}
}