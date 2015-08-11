using UnityEngine;
using System.Collections;

public class QuantumMesRec : MonoBehaviour {

	public GameObject mymesh;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public GameObject CreateObjMesh(string message){
		string [] parts;
		GameObject game_obj;
		Mesh thisMesh;
		MeshFilter myMeshFilter;

		parts = message.Split ('|');
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
		}
		
		if (mymesh) {
			game_obj = Instantiate (mymesh) as GameObject;
		} else {
			game_obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
		}
		myMeshFilter = (MeshFilter)game_obj.GetComponent<MeshFilter> ();
		myMeshFilter.mesh = MakeMesh (N_vertices, my_vertices, N_triangles, my_triangles);
		return game_obj;
	}

	public GameObject DrawColor(GameObject gameobject,string message){
		string [] parts;
		GameObject game_obj;
		Mesh thisMesh;
		MeshFilter myMeshFilter;

		parts = message.Split ('|');
		int N_obj = int.Parse (parts [1]);
		float isovalue = float.Parse (parts [2]);
		int N_wphase = int.Parse (parts [3]);
		var my_wphase = new float[N_wphase * 2];
		for (int i=0; i<N_wphase; i++) {
			my_wphase [i] = float.Parse (parts [3 + 1 + i]);
		}
		for (int i=N_wphase; i<N_wphase*2; i++) {
			my_wphase [i] = my_wphase [i - N_wphase];
		}
		game_obj = gameobject;
		myMeshFilter = (MeshFilter)game_obj.GetComponent<MeshFilter> ();
		int verticesLen = myMeshFilter.mesh.vertices.Length;
		myMeshFilter.mesh.colors = ColorVerticesbyPhase (verticesLen, 0.5f, my_wphase);

		//myMeshFilter.mesh.colors = ColorVerticesbyPhase (verticesLen, isovalue, my_wphase);
		return game_obj;
	}

	Mesh MakeDoubleSided (Mesh mesh)
	{
		var vertices = mesh.vertices;
		var uv = mesh.uv;
		var normals = mesh.normals;
		var szV = vertices.Length;
		var newVerts = new Vector3[szV * 2];
		var newUv = new Vector2[szV * 2];
		var newNorms = new Vector3[szV * 2];
		int k;
		for (int j=0; j< szV; j++) {
			// duplicate vertices and uvs:
			newVerts [j] = newVerts [j + szV] = vertices [j];
			newUv [j] = newUv [j + szV] = uv [j];
			// copy the original normals...
			newNorms [j] = normals [j];
			// and revert the new ones
			newNorms [j + szV] = -normals [j];
		}
		var triangles = mesh.triangles;
		var szT = triangles.Length;
		var newTris = new int[szT * 2]; // double the triangles
		
		for (int i=0; i< szT; i+=3) {
			// copy the original triangle
			newTris [i] = triangles [i];
			newTris [i + 1] = triangles [i + 1];
			newTris [i + 2] = triangles [i + 2];
			// save the new reversed triangle
			k = i + szT; 
			newTris [k] = triangles [i] + szV;
			newTris [k + 2] = triangles [i + 1] + szV;
			newTris [k + 1] = triangles [i + 2] + szV;
		}
		mesh.vertices = newVerts;
		mesh.uv = newUv;
		mesh.normals = newNorms;
		mesh.triangles = newTris;// assign triangles last!
		return mesh;
	}
	// make reversed mesh instead of double mesh, because mesh can not be more than 64k
	Mesh MakeReversedMesh (Mesh mesh)
	{
		var vertices = mesh.vertices;
		var uv = mesh.uv;
		var normals = mesh.normals;
		var szV = vertices.Length;
		var szN = normals.Length;
		for (int i=0; i<szN; i++) {
			normals [i] = -mesh.normals [i];
		}
		Mesh newMesh = new Mesh ();
		newMesh.vertices = mesh.vertices;
		newMesh.uv = mesh.uv;
		newMesh.normals = normals;
		newMesh.triangles = mesh.triangles;// assign triangles last!
		return newMesh;
	}

	Mesh MakeMesh (int N_vertices, float[] myvertices, int N_triangles, int[] mytriangles)
	{
		Mesh m = new Mesh ();
		
		var my_vertices = new Vector3[N_vertices];
		var my_uv = new Vector2[N_vertices];
		var my_triangles = mytriangles;
		for (int i = 0; i < N_vertices; i++) {
			my_vertices [i] = new Vector3 (myvertices [i * 3], myvertices [i * 3 + 1], myvertices [i * 3 + 2]);
			my_uv [i] = new Vector2 (my_vertices [i].x, my_vertices [i].z);
		}
		for (int i = 0; i < N_triangles*3; i++) {
			//my_triangles[i]= mytriangles[i]-1; need to be careful count from 0 or 1,gts is from 1, so need a minus
			my_triangles [i] = mytriangles [i];
		}
		m.vertices = my_vertices;
		m.uv = my_uv;
		m.RecalculateBounds ();
		m.triangles = my_triangles;
		m.RecalculateNormals ();
		m = MakeDoubleSided (m);
		m.RecalculateNormals ();
		return m;
	}

	Color[] ColorVerticesbyPhase (int verticesLen, float value, float[]mywphase)
	{
		Color[] mcolors = new Color[verticesLen];
		int ii = 0;
		//float[] arguments = new float[vertices.Length];
		float argument = 0f;
		float saturation = 1.0f;
		float lightness = value;
		float hue = 0f;

		while (ii < verticesLen) {
			//argument = mywphase[ii]*180/Mathf.PI+180;
			if (mywphase[ii]<0)
			{
				argument = mywphase[ii]*180/Mathf.PI+360;
			}
			else argument = mywphase[ii]*180/Mathf.PI;
			if (argument >= 360) hue =argument-360f;
			else hue= argument;
			Vector3 mRGB = HslToRgb(hue, saturation, lightness);
			mcolors [ii].a =1f;
			mcolors [ii].r =mRGB.x;
			mcolors [ii].g =mRGB.y;
			mcolors [ii].b =mRGB.z;
			//mcolors[ii].a = Color.blue.a;
			ii++;
		}
		return mcolors;
	}

	/**
 * Converts an HSL color value to RGB. Conversion formula
 * adapted from http://en.wikipedia.org/wiki/HSL_color_space.
 * Assumes h, s, and l are contained in the set [0, 1] and
 * returns r, g, and b in the set [0, 255].
 *
 * @param   Number  h       The hue
 * @param   Number  s       The saturation
 * @param   Number  l       The lightness
 * @return  Array           The RGB representation
 * */
	
	Vector3 HslToRgb(float h, float s, float l){
		float r1 = 0f;
		float g1 = 0f;
		float b1 = 0f;
		float m = 0f;
		if (s == 0) {
			r1 = l;
			g1 = l;
			b1 = l;
		} else {
			float _h = h / 60f;
			int i = (int)(_h);
			//Debug.Log (i);
			float c = (1f - Mathf.Abs(2*l - 1f)) * s;
			float x = c*(1-Mathf.Abs((h/60f)%2-1f));
			m = l - c / 2f;
			
			switch (i) {
			case 0:
				r1 = c;
				g1 = x;
				b1 = 0f;
				break;
			case 1:
				r1 = x;
				g1 = c;
				b1 = 0f;
				break;
			case 2:
				r1 = 0f;
				g1 = c;
				b1 = x;
				break;
			case 3:
				r1 = 0f;
				g1 = x;
				b1 = c;
				break;
			case 4:
				r1 = x;
				g1 = 0f;
				b1 = c;
				break;
			case 5:
				r1 = c;
				g1 = 0f;
				b1 = x;
				break;
			default:
				break;
			}
		}
		Vector3 mcolor = new Vector3(r1+m, g1+m, b1+m);
		return mcolor;
	}
}
