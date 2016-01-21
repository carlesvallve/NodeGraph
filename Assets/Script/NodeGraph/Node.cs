using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Node {
	
	public string name { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public Color color { get; set; }
	public NodeUi ui { get; set; }

	private List<Node> FriendsList = new List<Node>();
	public List<Node> Friends {
		get { return FriendsList; }
	}


	public Node (string name) {
		this.name = name;

		int d = 350;
		this.X = Random.Range(-d, d);
		this.Y = Random.Range(-d, d);
		this.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
	}


	public void SetFriend (Node p) {
		// escape if we are already friends
		if (IsFriendOf(p)) {
			return;
		}

		// add new friend to friend list
		FriendsList.Add(p);

		// friends are always corresponded
		p.SetFriend(this); 
	}


	public bool IsFriendOf(Node p) {
		foreach (Node friend in Friends) {
			if (p == friend) { return true; }
		}

		return false;
	}

	
	
}
