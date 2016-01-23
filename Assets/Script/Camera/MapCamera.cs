using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityCamera = UnityEngine.Camera;

using DG.Tweening;

//namespace Zap.Map.Camera {

	public class MapCamera : MonoBehaviour {

		public static bool active = true;

		// accessor to refer to the map unity camera 
		public static UnityCamera currentCamera { get; set; }

		// camera bounds
		public Bounds bounds { get; set; }

		// resolution
		private int resHeight = 1024;
		private int pixelsPerUnit = 1;
		
		// zoom
		private float zoomSpeed = 50f;
		private float minZoom = 10;
		private float maxZoom = 1024 / 2;
		private float pinchLength = 0;
		private bool zooming;

		// panning
		private float panSpeed = 1f;
		private bool panSpeedRelativeToZoom = false;
		private Vector2 lastPoint;

		// interpolation
		private float defaultInterpolationTime = 0.35f;
		Tween moveTween;
		Tween zoomTween;


		public void Init (UnityCamera currentCamera, Bounds bounds) {
			MapCamera.currentCamera = currentCamera;
			this.bounds = bounds;

			maxZoom = ((resHeight / 2) / pixelsPerUnit);
			currentCamera.orthographicSize = maxZoom;
		}


		void Update () {
			if (!active) { 
				return; 
			}

			// apply pan
			if (!zooming) {
				SetPanInputEditor();
				SetPanInputDevice();
			}
			
			// apply zoom
			SetZoomInputEditor();
			SetZoomInputDevice();
		}


		// =========================================
		// Move And Zoom methods
		// =========================================

		private void KillActiveTweens () {
			if (moveTween != null && moveTween.IsActive()) {
				moveTween.Kill(false);
			}

			if (zoomTween != null && zoomTween.IsActive()) {
				zoomTween.Kill(false);
			}
		}

		public void SetPosition (float x, float y) {
			currentCamera.transform.position = new Vector3(
				x - bounds.extents.x, y - bounds.extents.y, currentCamera.transform.position.z
			);
			ConstrainToBounds();
		}


		public void SetZoom (float zoom) {
			currentCamera.orthographicSize = zoom;
			ConstrainToBounds();
		}


		public void SetPositionAndZoom (float x, float y, float zoom) {
			// get camera goal position
			Vector3 pos = new Vector3(x - bounds.extents.x, y - bounds.extents.y, currentCamera.transform.position.z);
			pos = ConstrainToBounds(pos, zoom, false);

			currentCamera.orthographicSize = zoom;
			currentCamera.transform.position = pos;

			ConstrainToBounds();
		}


		public void InterpolatePosition (float x, float y, float duration = -1) {
			if (duration < 0) { duration = defaultInterpolationTime; }
			
			// get camera goal position
			Vector3 pos = new Vector3(x - bounds.extents.x, y - bounds.extents.y, currentCamera.transform.position.z);
			pos = ConstrainToBounds(pos, currentCamera.orthographicSize, false);

			KillActiveTweens();
			moveTween = currentCamera.transform.DOLocalMove(pos, duration, false).SetEase(Ease.InOutQuad)
				.OnUpdate(() => ConstrainToBounds(currentCamera.transform.position, currentCamera.orthographicSize, true));
		}


		public void InterpolateZoom (float zoom, float duration = -1) {
			if (duration < 0) { duration = defaultInterpolationTime; }
			
			// get camera goal position
			Vector3 pos = new Vector3(
				currentCamera.transform.position.x - bounds.extents.x, 
				currentCamera.transform.position.y - bounds.extents.y, 
				currentCamera.transform.position.z
			);
			pos = ConstrainToBounds(pos, zoom, false);

			// move camera to final zoom and position
			KillActiveTweens();
			zoomTween = currentCamera.DOOrthoSize(zoom, duration).SetEase(Ease.InOutQuad)
				.OnUpdate(() => ConstrainToBounds(currentCamera.transform.position, currentCamera.orthographicSize, true));
		}


		public void InterpolatePositionAndZoom (float x, float y, float zoom, float duration = -1) {
			if (duration < 0) { duration = defaultInterpolationTime; }


			
			// get camera goal position
			Vector3 pos = new Vector3(x - bounds.extents.x, y - bounds.extents.y, currentCamera.transform.position.z);
			pos = ConstrainToBounds(pos, zoom, false);

			// move camera to final zoom and position
			KillActiveTweens();
			zoomTween = currentCamera.DOOrthoSize(zoom, duration).SetEase(Ease.InOutQuad);
			moveTween = currentCamera.transform.DOLocalMove(pos, duration, false).SetEase(Ease.InOutQuad)
				.OnUpdate(() => ConstrainToBounds(currentCamera.transform.position, zoom, true));
		}


		// =========================================
		// User interaction methods
		// =========================================

		private void PanOrthoCamera (Vector2 delta) {
			// get final delta
			float pixelSizeAdjustment = 1f / pixelsPerUnit;
			delta *= pixelSizeAdjustment;

			// get final offset
			Vector2 offset = Vector2.Scale(delta, currentCamera.transform.localScale);

			// translate camera by offset
			currentCamera.transform.position -= (Vector3)offset;
			ConstrainToBounds();
		}


		private void ZoomOrthoCamera (Vector3 zoomTowards, float amount, float duration = -1) {
			if (duration < 0) { duration = defaultInterpolationTime; }

			// calculate how much we will have to move towards the zoomTowards position
			float multiplier = (1.0f / currentCamera.orthographicSize * amount);

			float zoom = currentCamera.orthographicSize - amount;
			zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

			// interpolate camera zoom
			currentCamera.DOOrthoSize(zoom, duration).SetEase(Ease.OutQuad);
				
			// interpolate camera position
			Vector3 pos = currentCamera.transform.position + (zoomTowards - currentCamera.transform.position) * multiplier; 
			currentCamera.transform.DOLocalMove(pos, duration, false).SetEase(Ease.OutQuad)
					.OnUpdate(() => ConstrainToBounds(currentCamera.transform.position, currentCamera.orthographicSize, true));
		}


		// Pan Input

		[Conditional("UNITY_EDITOR")]
		private void SetPanInputEditor () {
			if (Input.GetMouseButtonDown(0)) {
				lastPoint = Input.mousePosition;
			} else if (Input.GetMouseButton(0)) {
				float speed = panSpeedRelativeToZoom ? (panSpeed * maxZoom / currentCamera.orthographicSize) : panSpeed;
				Vector2 delta = ((Vector2)Input.mousePosition - lastPoint) * speed;
				PanOrthoCamera(delta);
				lastPoint = (Vector2)Input.mousePosition;
			}
		}


		[Conditional("UNITY_IOS"), Conditional("UNITY_ANDROID")]
		private void SetPanInputDevice () {
			if (Input.touchCount != 1) { return; }

			if (Input.GetTouch(0).phase == TouchPhase.Began) {
				lastPoint = Input.GetTouch(0).position;
			} else if (Input.GetTouch(0).phase == TouchPhase.Moved) {
				float speed = panSpeedRelativeToZoom ? (panSpeed * maxZoom / currentCamera.orthographicSize) : panSpeed;
				Vector2 delta = (Input.GetTouch(0).position - lastPoint) * speed;
				PanOrthoCamera(delta);
				lastPoint = Input.GetTouch(0).position;
			}	
		}


		// Zoom Input

		[Conditional("UNITY_EDITOR")]
		private void SetZoomInputEditor () {
			zooming = false;
			if (Input.GetAxis("Mouse ScrollWheel") > 0) {
				// apply scroll forward
				ZoomOrthoCamera(currentCamera.ScreenToWorldPoint((Vector2)Input.mousePosition), 1 * zoomSpeed);
				zooming = true;
			} else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
				// apply scoll back
				ZoomOrthoCamera(currentCamera.ScreenToWorldPoint((Vector2)Input.mousePosition), -1 * zoomSpeed);
				zooming = true;
			}

		}


		[Conditional("UNITY_IOS"), Conditional("UNITY_ANDROID")]
		private void SetZoomInputDevice () {
			// pinch began
			if (Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Began) {
				pinchLength = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
				zooming = true;
			}
			
			// pinch moved
			if (Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)) {
				float deltaLength = (Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) - pinchLength);
				float zoomDistance = Mathf.Clamp(1f / pinchLength * (deltaLength), -1.5f, 1.5f);
				pinchLength = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

				Vector2 center = (Input.GetTouch(0).position + Input.GetTouch(1).position) / 2f;
				ZoomOrthoCamera(currentCamera.ScreenToWorldPoint(center), zoomDistance * zoomSpeed * 5f);
			}

			// pinch ended (with all fingers)
			if (Input.touchCount == 0) {
				zooming = false;
			}
		}


		// =========================================
		// Constrain to camera bounds
		// =========================================

		private void ConstrainToBounds () {
			ConstrainToBounds(currentCamera.transform.position, 0, true);
		}


		private Vector3 ConstrainToBounds (Vector3 pos, float atZoom = 0, bool updatePosition = true) {
			//return pos;

			if (atZoom == 0) { atZoom = currentCamera.orthographicSize; }
			// calculate bg sprite bounds
			float vertExtent = atZoom; //currentCamera.orthographicSize;  
			float horzExtent = vertExtent * Screen.width / Screen.height;

			float leftBound = (float)(horzExtent - bounds.size.x / 2.0f);
			float rightBound = (float)(bounds.size.x / 2.0f - horzExtent);
			float bottomBound = (float)(vertExtent - bounds.size.y / 2.0f);
			float topBound = (float)(bounds.size.y  / 2.0f - vertExtent);

			// limit camera position to bounds
			//Vector3 pos = currentCamera.transform.position;
			pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
			pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);

			// reposition camera
			if (updatePosition) {
				currentCamera.transform.position = pos;
			}
			
			return pos;
		}

	}

//}
