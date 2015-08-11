using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class jsondata{
	public string cmd;
	public int num_vertices;
	public int num_triangles;
	public float isovalue;
	public string list_vertices;
	public string list_triangles; 
	public string list_phases;
	public List<string> ingredients = new List<string>();
	
	public jsondata() { }
}
