using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;

public class NodeGraph {

	// initializes nodegraph and fills it with nodes and relationships between them

	public Dictionary<string, Node> BuildNodeGraph() {
		// set nodes
		Dictionary<string, Node> nodes = new Dictionary<string, Node>() {
			{ "Aaron", new Node("Aaron") },
			{ "Betty", new Node("Betty") },
			{ "Brian", new Node("Brian") },
			{ "Catherine", new Node("Catherine") },
			{ "Carson", new Node("Carson") },
			{ "Darian", new Node("Darian") },
			{ "Derek", new Node("Derek") }
		};

		// set node connections
		nodes["Aaron"].SetLink(nodes["Betty"]);
		nodes["Aaron"].SetLink(nodes["Darian"]);
		nodes["Aaron"].SetLink(nodes["Brian"]);
		nodes["Betty"].SetLink(nodes["Catherine"]);
		nodes["Betty"].SetLink(nodes["Darian"]);
		nodes["Brian"].SetLink(nodes["Carson"]);
		nodes["Brian"].SetLink(nodes["Derek"]);

		return nodes;
	}


	// creates node gameobjects and itializes their ui

	public void RenderNodeGraph(GameObject nodePrefab, Dictionary<string, Node> nodes) {
		GameObject container = new GameObject();
		container.transform.SetParent(Scene.instance.transform);
		container.name = "Nodes";

		foreach(KeyValuePair<string, Node> entry in nodes) {
			Node node = entry.Value;

			// instantiate node prefab and initialize his ui
			GameObject go = (GameObject)GameObject.Instantiate(nodePrefab);
			node.ui = go.GetComponent<NodeUi>();
			node.ui.Init(container, node);

			node.ui.OnClick += () => {
				Scene.instance.ClickOnNode(node);
			};

			node.ui.OnDragging += () => {
				Scene.instance.DragOnNode(node);
			};
		}
	}


	// Recursevily traverses all nodes in the nodegraph
	// Not used for anything at the moment

	public void Traverse(Node root) {
		Debug.Log(root.name);
		
		for (int i = 0; i < root.Links.Count; i++) {
			Traverse(root.Links[i]);
		}
	}


	// path finding based on recursive a-star

	public List<Node> SearchPath (Node from, Node to) {
		List<Node> visited = new List<Node>();
		List<Node> path = AstarRecursive(from, to, visited);
		
		if (path == null) {
			Debug.LogError("No path was found from " + from.name + " to " + to.name);
		} else {
			path.Reverse();
			LogPath(path);
		}
			
		return path;
	}


	// a-star single iteration

	public List<Node> AstarRecursive(Node from, Node to, List<Node> visited){
		// escape if current node is already visited
		if (visited.Contains(from)){
			return null;
		}
		
		visited.Add(from);

		// end recursiveness if current node if the goal
		if (from == to){
			List<Node> TmpL = new List<Node>();
			TmpL.Add(from);

			return TmpL;
		}

		// generate a temp list of not-visited friends
		List<Node> tmp = new List<Node>();
		foreach (Node link in from.Links) {
			if (!visited.Contains(link)) {
				tmp.Add(link);
			}
		}

		// we then can order the list by any desired conditions, 
		// in this case distance
		tmp.Sort(delegate(Node n1, Node n2) {
			Vector2 p1 = new Vector2(n1.X, n1.Y);
			Vector2 p2 = new Vector2(to.X, to.Y);
			float dist1 = (p1 - p2).sqrMagnitude;

			Vector2 p3 = new Vector2(n2.X, n2.Y);
			Vector2 p4 = new Vector2(to.X, to.Y);
			float dist2 = (p3 - p4).sqrMagnitude;

			// sort by dist in descending order
			int a = dist1.CompareTo(dist2);

			return a;
		});

		
		if (tmp != null){
			foreach (Node ww in tmp){
				List<Node> TmpR =  new List<Node>();

				TmpR = AstarRecursive(ww, to, visited);
				if (TmpR != null) {
					TmpR.Add(from);
					return TmpR;
				}
			}
		}

		return null;
	}


	// logs the path if any was found

	public void LogPath (List<Node> path) {
		if (path == null) { return; }

		string str = "";
		for (int i = 0; i < path.Count; i++) {

			str += path[i].name; 
			if (i < path.Count - 1) { str += " -> "; }
		}

		Debug.Log ("Path [ " + str + " ]");
	}

}
