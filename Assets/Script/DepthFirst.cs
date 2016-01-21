using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using System.Collections;


 
namespace DepthFirst {

	public class Person {
		public Person(string name) {
			this.name = name;

			int d = 350;
			this.X = UnityEngine.Random.Range(-d, d);
			this.Y = UnityEngine.Random.Range(-d, d);
			this.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
		}

		public string name { get; set; }
		public List<Person> Friends {
			get {
				return FriendsList;
			}
		}

		public void isFriendOf(Person p) {
			FriendsList.Add(p);
		}

		List<Person> FriendsList = new List<Person>();

		public override string ToString() {
			return name;
		}


		public int X { get; set; }
		public int Y { get; set; }
		public Color color { get; set; }

		public DragAndDrop ui { get; set; }
	}


	public class DepthFirstAlgorithm {

		public GameObject container;

		
		public Person BuildFriendGraph() {

			this.container = new GameObject();
			this.container.name = "Container";

			Person Aaron = new Person("Aaron");
			Person Betty = new Person("Betty");
			Person Brian = new Person("Brian");
			Aaron.isFriendOf(Betty);
			Aaron.isFriendOf(Brian);

			Person Catherine = new Person("Catherine");
			Person Carson = new Person("Carson");
			Person Darian = new Person("Darian");
			Person Derek = new Person("Derek");
			Betty.isFriendOf(Catherine);
			Betty.isFriendOf(Darian);
			Brian.isFriendOf(Carson);
			Brian.isFriendOf(Derek);

			return Aaron;
		}


		public Person Search(Person root, string nameToSearchFor) {
			//Debug.Log (">>> " + nameToSearchFor + " -> " + root.name + " " + (nameToSearchFor == root.name));
			if (nameToSearchFor == root.name)
				return root;

			Person personFound = null;
			for (int i = 0; i < root.Friends.Count; i++) {
				personFound = Search(root.Friends[i], nameToSearchFor);
				if (personFound != null)
					break;
			}
			return personFound;
		}


		public void Traverse(Person root, GameObject personPrefab, GameObject pathPrefab) {
			Debug.Log(root.name);
			Render(root, personPrefab, pathPrefab);
			
			for (int i = 0; i < root.Friends.Count; i++) {
				Traverse(root.Friends[i], personPrefab, pathPrefab);
			}
		}


		private void Render (Person root, GameObject personPrefab, GameObject pathPrefab) {
			// instantiate person prefab
			GameObject go = (GameObject)GameObject.Instantiate(personPrefab);
			go.name = root.name;
			go.transform.SetParent(container.transform);
			go.transform.localPosition = new Vector3(root.X, root.Y, 0);

			// initialize person ui
			root.ui = go.GetComponent<DragAndDrop>();
			root.ui.Init(root);
		}
	}

}


