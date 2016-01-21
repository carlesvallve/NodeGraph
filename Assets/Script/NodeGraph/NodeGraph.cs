using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NodeGraph {


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
			nodes["Aaron"].SetFriend(nodes["Betty"]);
			nodes["Aaron"].SetFriend(nodes["Brian"]);
			nodes["Betty"].SetFriend(nodes["Catherine"]);
			nodes["Betty"].SetFriend(nodes["Darian"]);
			nodes["Brian"].SetFriend(nodes["Carson"]);
			nodes["Brian"].SetFriend(nodes["Derek"]);

			return nodes;
		}


		// creates node gameobjects and itializes their ui

		public void RenderNodeGraph(GameObject nodePrefab, Dictionary<string, Node> nodes) {
			foreach(KeyValuePair<string, Node> entry in nodes) {
				Node node = entry.Value;

				// instantiate node prefab and initialize his ui
				GameObject go = (GameObject)GameObject.Instantiate(nodePrefab);
				node.ui = go.GetComponent<NodeUi>();
				node.ui.Init(node);
			}
		}


		// not used for anything. Recursevily traverses all nodes in the nodegraph

		public void Traverse(Node root) {
			Debug.Log(root.name);
			
			for (int i = 0; i < root.Friends.Count; i++) {
				Traverse(root.Friends[i]);
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
			foreach (Node friend in from.Friends) {
				if (!visited.Contains(friend)) {
					tmp.Add(friend);
				}
			}

			// we then can order the list by any desired conditions, 
			// in this case distance
			tmp = tmp.OrderBy(t => {
				Vector2 p1 = new Vector2(t.X, t.Y);
				Vector2 p2 = new Vector2(to.X, to.Y);
				return (p1 - p2).sqrMagnitude;
			}).ToList();

			// This is how it would look if we could use Linq...
			/*List<Node> tmp =  nx.Friends
				// if a friend is not visited
				.Where (c => !visited.Contains(c) ).ToList(); 

				// we can order here by several conditions, in this case distance         
				.OrderBy(t => {
					Vector2 p1 = new Vector2(t.X, t.Y);
					Vector2 p2 = new Vector2(goal.X, goal.Y);
					return (p1 - p2).sqrMagnitude;
				}).ToList();*/
			
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


		// prints the path if any was found

		public void LogPath (List<Node> path) {
			if (path == null) { return; }

			string str = "";
			for (int i = 0; i < path.Count; i++) {

				str += path[i].name; 
				if (i < path.Count - 1) { str += " -> "; }
			}

			Debug.Log ("Path [" + str + "]");
		}

	}
