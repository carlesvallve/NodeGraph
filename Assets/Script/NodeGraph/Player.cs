using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public class Player : MonoBehaviour {

	private Node currentNode;
	public Node CurrentNode { 
		get {
			return currentNode;
		} 
		set {
			currentNode = value;
			//SetPosition(value.X, value.Y);
			text.text = value.name;
		}
	}

	private TextMesh text;
	private Tween pathTween;


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


	void Update () {
		if (currentNode != null) {
			SetPosition(currentNode.X, currentNode.Y);
		}
	}


	public void FollowPath (List<Node> path) {
		// remove first node of the path since we are already in it
		path.RemoveAt(0);

		// get waypoints
		Vector3[] waypoints = new Vector3[path.Count];
		for (int i = 0; i < path.Count; i++) {
			waypoints[i] = new Vector3(path[i].X, path[i].Y);
		}

		// kill previous path tween
		if (pathTween != null && pathTween.IsActive()) {
			pathTween.Kill(false);
		}

		// tween along waypoints
		float duration = waypoints.Length * 0.5f;
		pathTween = transform.DOLocalPath(waypoints, duration, PathType.Linear, PathMode.Ignore, 10)
		.SetEase(Ease.Linear)
		.OnWaypointChange((int index) => {
			CurrentNode = path[index];
		});
	}


	/*private Vector3[] GetStepWaypoints (StageUi stageUi1, StageUi stageUi2, int direction) {
		Vector3 startPos = new Vector3(stageUi1.Stage.X * MapUi.tileSize, stageUi1.Stage.Y * MapUi.tileSize, 0);
		MapPathPoint[] arr = stageUi1.GetPathToMapPoint(stageUi2).ToArray();

		if (direction == -1) { 
			startPos = new Vector3(stageUi2.Stage.X * MapUi.tileSize, stageUi2.Stage.Y * MapUi.tileSize, 0); 
			System.Array.Reverse(arr); 
		}

		Vector3[] waypoints = new Vector3[arr.Length + 1];
		for (int i = 0; i < arr.Length; i++) {
			waypoints[i] = startPos + arr[i].transform.localPosition;
		}

		waypoints[arr.Length] = new Vector3(stageUi2.Stage.X * MapUi.tileSize, stageUi2.Stage.Y * MapUi.tileSize, 0);

		return waypoints;
	}*/

}
