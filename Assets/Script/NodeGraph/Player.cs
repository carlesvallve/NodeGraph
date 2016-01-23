using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public class Player : MonoBehaviour {

	public delegate void OnArrivedToNodeHandler();
	public event OnArrivedToNodeHandler OnArrivedToNode;

	private Node currentNode;
	public Node CurrentNode { 
		get {
			return currentNode;
		} 
		set {
			currentNode = value;
			text.text = value.name;
		}
	}

	private TextMesh text;
	private Sequence pathSequence;
	private List<Node> newPath;

	public bool moving { get; private set; }


	public void Init (Node initialNode) {
		name = "Player";
		gameObject.layer = LayerMask.NameToLayer("Player");

		Transform t = transform.Find("Text");
		if (t != null) {
			t.GetComponent<MeshRenderer>().sortingOrder = 110;
			text = t.GetComponent<TextMesh>();
		}

		CurrentNode = initialNode;
		SetPosition(CurrentNode.X, CurrentNode.Y);
	}


	public void SetPosition(int x, int y) {
		transform.localPosition = new Vector3(x, y, 0);
	}


	public void SetNewPath(List<Node> path) {
		if (pathSequence != null && pathSequence.IsActive()) {
			newPath = path;
		} else {
			FollowPath(path);
		}
	}


	public void FollowPath (List<Node> path) {
		newPath = null;

		// generate a list of waypoint lines
		List<Vector3[]> waypoints = GenerateWaypoints(path);

		// kill previous sequence
		if (pathSequence != null && pathSequence.IsActive()) {
			pathSequence.Kill(false);
			moving = false;
		}

		// start the movement sequence
		int num = 0;
		pathSequence = DOTween.Sequence()
		.OnStart(() => { moving = true; })
		.OnComplete(() => { moving = false; });

		// iterate along each line to follow
		for (int i = 0; i < waypoints.Count; i++) {
			float duration = waypoints[i].Length * 0.1f;

			pathSequence.Append(
				transform.DOLocalPath(waypoints[i], duration, PathType.Linear, PathMode.Ignore, 10).
				SetEase(Ease.Linear)
				.OnComplete( () => { 
					// arrival to next node
					num++;
					CurrentNode = path[num]; 
					print(CurrentNode.name);

					// emit arrived event
					if (OnArrivedToNode != null) { 
						OnArrivedToNode.Invoke(); 
					}
				})
			)
			.AppendInterval(0.01f);
		}
	}


	private List<Vector3[]> GenerateWaypoints (List<Node> path) {
		// generate a list of paths between nodes
		List<List<Vector3>> points = new List<List<Vector3>>();
		for (int i = 0; i < path.Count - 1; i++) {
			points.Add(new List<Vector3>());

			if (i < path.Count - 1) {
				// add the list of points connecting to next node
				NodePath line = path[i].ui.GetPathToNode(path[i + 1]);
				for (int n = 0; n < line.Count; n++) {	
					points[i].Add(line[n].transform.position);
				}

				// add next node in path
				points[i].Add(new Vector3(path[i + 1].X, path[i + 1].Y, 0));
			}
		}

		// generate the list of waypoint lines
		List<Vector3[]> waypoints = new List<Vector3[]>();
		foreach (List<Vector3> line in points) {
			waypoints.Add(line.ToArray());
		}

		return waypoints;
	}

}
