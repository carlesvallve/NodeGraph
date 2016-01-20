/*// .Net includes
using System.Collections;
using System.Collections.Generic;

// Unity includes
using UnityEngine;

// Libs
using Zap.Map.Logic;
using Zap.Map.Data;
using Zap.Map.Ui;
using Zap.Map.Camera;

using DG.Tweening;


namespace Zap.Views {

	/// <summary>
	/// Map view to select map and stage in which to play the game
	/// </summary>
	public class MapView : MonoBehaviour {

		// Prefabs to instantiate
		[Tooltip("Prefab that will be used to render the worldmap ui")]
		public WorldMapUi UIWORLDMAP_PREFAB;
		[Tooltip("Prefab that will be used to render the map ui")]
		public MapUi UIMAP_PREFAB;

		// Holds the map ui
		private WorldMapUi worldMapUi;
		private MapUi mapUi;


		// reference to the camera class for this view
		private MapCamera mapCamera;


		void Start() {
			// initialize DOTween
			DOTween.Init();
			DOTween.defaultEaseType = Ease.OutQuad;

			// get camera
			mapCamera = GetComponent<MapCamera>();
			mapCamera.Init(Camera.main);
			
			// Run Test on loading map data and serialize it
			WorldMapData worldMapData = LoadWorldMap();
			WorldMapLogic worldMap = CreateWorldMap(worldMapData);

			LoadMaps(worldMapData, worldMap);

			// render the world map
			CreateWorldMapUi(worldMap);
		}


		// ===============================================
		// Load Data
		// ===============================================


		private WorldMapData LoadWorldMap () {
			TextAsset asset = Resources.Load("Data/MapData/WorldMap") as TextAsset;
			WorldMapData worldMapData = JsonUtility.FromJson<WorldMapData>(asset.text);

			Debug.Log("========= WorldMapData =========");
			Debug.Log(JsonUtility.ToJson(worldMapData));

			return worldMapData;
		}


		private void LoadMaps (WorldMapData worldMapData, WorldMapLogic worldMap) {

			foreach (MapData map in worldMapData.maps) {
				TextAsset asset = Resources.Load("Data/MapData/" + map.id) as TextAsset;
				MapData mapData =  JsonUtility.FromJson<MapData>(asset.text);

				Debug.Log("========= MapData =========");
				Debug.Log(JsonUtility.ToJson(mapData));

				// create a MapLogic class for each MapData class and add it to worldMap
				worldMap.Add(CreateMap(mapData, map.location.x, map.location.y));
			}
		}


		// ===============================================
		// Set Logic
		// ===============================================

		private WorldMapLogic CreateWorldMap (WorldMapData worldMapData) {

			WorldMapLogic worldMap = new WorldMapLogic();
			worldMap.Background = worldMapData.background;
			Debug.Log("Created World Map");

			worldMap.OnMapAdded += (mapAdded) => {
				Debug.Log ("Map Added: " + mapAdded.GetHashCode());
			};

			worldMap.OnMapRemoved += (mapRemoved) => {
				Debug.Log ("Map Removed: " + mapRemoved.GetHashCode());
			};

			worldMap.OnMapChanged += (oldMap, newMap) => {
				int oldHash = 0;
				if (oldMap != null) {
					oldHash = oldMap.GetHashCode();
				}

				int newHash = 0;
				if (newMap != null) {
					newHash = newMap.GetHashCode();
				}

				Debug.Log ("Map Changed: from " + oldHash + " to " + newHash);

				// render new selected map
				CreateMapUi(newMap);
			};

			return worldMap;
		}


		private MapLogic CreateMap (MapData mapData, int x, int y) {

			// create map

			MapLogic map = new MapLogic (mapData.id, x, y, true, true);

			map.Name = mapData.name;
			map.Background = mapData.background;

			Debug.Log("Created Map" + map.Id + " (" + map.Name + ")");

			map.PropertyChanged += (sender, e) => {
				MapLogic mapLogic = sender as MapLogic;

				if (mapLogic == null) {
					return;
				}

				switch (e.PropertyName) {
					case "Visible":
						Debug.Log (mapLogic.Id + "(" + mapLogic.Name + ") Stage Visible Change: " + mapLogic.GetHashCode ());
						break;
					case "Locked":
						Debug.Log (mapLogic.Id + "(" + mapLogic.Name + ") Stage Lock Change: " + mapLogic.GetHashCode ());
						break;
					case "Name":
						Debug.Log (mapLogic.Id + "(" + mapLogic.Name + ") Name Changed: " + mapLogic.GetHashCode ());
						break;
					case "Background":
						Debug.Log (mapLogic.Id + "(" + mapLogic.Name + ") Background Changed: " + mapLogic.GetHashCode ());
						break;
				}
			};

			map.OnStageChanged += (oldStage, newStage) => {
				int oldHash = 0;
				if (oldStage != null) {
					oldHash = oldStage.GetHashCode();
				}
				//Debug.Log ("Map" + map.Id + " (" + map.Name + ") Stage Change: " + oldHash + " to " + newStage.GetHashCode());
			};

			map.OnStageAdded += (stageAdded) => {
				Debug.Log ("Map" + map.Id + " (" + map.Name + ") Stage" + stageAdded.StageNumber + " Added: " + stageAdded.GetHashCode());
			};

			map.OnStageRemoved += (stageRemoved) => {
				Debug.Log ("Map" + map.Id + " (" + map.Name + ") Stage" + stageRemoved.StageNumber + " Removed: " + stageRemoved.GetHashCode());
			};

			// create map stages

			for (int i = 0; i < mapData.stages.Count; i++) {
				StageLogic stage = new StageLogic (mapData.stages[i].id, mapData.stages[i].location.x, mapData.stages[i].location.y, true, true);

				stage.PropertyChanged += (sender, e) => {
					StageLogic stageLogic = sender as StageLogic;

					if (stageLogic == null) {
						return;
					}

					switch (e.PropertyName) {
						case "X":
						case "Y":
							Debug.Log ("Map" + map.Id + " (" + map.Name + ") Stage" + stageLogic.StageNumber + " Location Change: " + stageLogic.GetHashCode());
							break;
						case "Locked":
							Debug.Log ("Map" + map.Id + " (" + map.Name + ") Stage" + stageLogic.StageNumber + " Lock Change: " + stageLogic.GetHashCode());
							break;
						case "Visible":
							Debug.Log ("Map" + map.Id + " (" + map.Name + ") Stage" + stageLogic.StageNumber + " Visible Change: " + stageLogic.GetHashCode());
							break;
					}
				};

				map.Add(stage);
			}

			// select first stage
			map.CurrentStage = map.ReadOnlyStages[0];

			return map;
		}


		// ===============================================
		// Render Ui
		// ===============================================

		private void CreateWorldMapUi(WorldMapLogic worldMap) {
			if (mapUi != null) {
				Destroy(mapUi.gameObject);
			}

			if (worldMapUi != null) {
				Destroy(worldMapUi.gameObject);
			}

			Debug.Log("========= WorldMapUi =========");
			worldMapUi = (WorldMapUi)Instantiate<WorldMapUi>(UIWORLDMAP_PREFAB);
			worldMapUi.gameObject.transform.SetParent(gameObject.transform, false);
			worldMapUi.Init(worldMap, mapCamera);
		}


		private void CreateMapUi(MapLogic map) {
			if (worldMapUi != null) {
				Destroy(worldMapUi.gameObject);
			}

			if (mapUi != null) {
				Destroy(mapUi.gameObject);
			}

			if (map == null) {
				return;
			}

			Debug.Log("========= MapUi =========");
			mapUi = (MapUi)Instantiate<MapUi>(UIMAP_PREFAB);
			mapUi.gameObject.transform.SetParent(gameObject.transform, false);
			mapUi.Init(map, mapCamera);
		}


		// ===============================================
		// Hud Actions
		// ===============================================

		public void ButtonBack () {
			CreateWorldMapUi(worldMapUi.WorldMap);
		}

	}
}
*/