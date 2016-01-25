using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Node {
	
	public string name { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public Color color { get; set; }
	public NodeUi ui { get; set; }

	private List<Node> LinksList = new List<Node>();
	public List<Node> Links {
		get { return LinksList; }
	}


	public Node (string name) {
		this.name = name;

		int d = 350;
		this.X = Random.Range(-d, d);
		this.Y = Random.Range(-d, d);
		this.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
	}


	public void SetLink (Node p) {
		// escape if we are already linked
		if (Links.Contains(p)) {
			return;
		}

		// link me to given node
		LinksList.Add(p);

		// links are always bidirectional
		p.SetLink(this); 
	}
	
}
