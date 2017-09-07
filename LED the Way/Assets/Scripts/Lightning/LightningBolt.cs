using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBolt : MonoBehaviour {

	public List<GameObject> ActiveLineObjects;
	public List<GameObject> InactiveLineObjects;

	public GameObject LinePrefab;

	public float Alpha { get; set; }
	public float FadeOutRate { get;set; }
	public Color Tint { get; set; }
	public bool IsComplete { get { return Alpha <= 0; } }

	public Vector2 Start {
		get {
			return ActiveLineObjects [0].GetComponent<Line> ().startPoint;
		}
	}

	public Vector2 End {
		get {
			return ActiveLineObjects [ActiveLineObjects.Count - 1]
				.GetComponent<Line> ().endPoint;
		}
	}

	public void Initialize(int maxSegments) {
		ActiveLineObjects = new List<GameObject> ();
		InactiveLineObjects = new List<GameObject> ();

		for (int i = 0; i < maxSegments; i++) {
			GameObject lineObj = (GameObject)GameObject.Instantiate (LinePrefab);
			lineObj.transform.parent = transform;
			lineObj.SetActive (false);
			InactiveLineObjects.Add (lineObj);
		}
	}

	public void ActivateBolt(Vector2 source, Vector2 dest,
		Color color, float thickness) {
		Tint = color;
		Alpha = 1.5f;
		FadeOutRate = 0.03f;

		// Prevent getting a 0 magnitude
		if (Vector2.Distance (dest, source) <= 0) {
			Vector2 adjust = Random.insideUnitCircle;
			if (adjust.magnitude <= 0)
				adjust.x += .1f;
			dest += adjust;
		}

		// Slope is dy/dx
		Vector2 slope = dest - source;
		// Perpendicular to the slope
		Vector2 normal = (new Vector2 (slope.y, -slope.x)).normalized;

		float boltDistance = slope.magnitude;
		List<float> linePositions = new List<float> ();
		linePositions.Add (0);

		for (int i = 0; i < boltDistance / 4; i++) {
			linePositions.Add (Random.Range (.25f, .75f));
		}

		linePositions.Sort ();

		const float Sway = 120;
		const float Jaggedness = 1 / Sway;
		// How far will the segments deviate from the slope,
		// turns into a straight line if spread is 0
		float spread = 1f;

		Vector2 previousPoint = source;
		float previousDisplacement = 0;

		for (int i = 1; i < linePositions.Count; i++) {
			int inactiveCount = InactiveLineObjects.Count;
			if (inactiveCount <= 0)
				break;

			float pos = linePositions [i]; // From 0 to 1
			float scale = (boltDistance * Jaggedness) * // jaggedness taking percentage of distance
			              (pos - linePositions [i - 1]); // bigger distance from prev. point, bigger scale
			// Ensure the bolt goes to the destination point
			float envelope = pos > 0.95f ? 20 * (1 - pos) : spread; // Deviates more for the last few points
			float displacement = Random.Range(-Sway, Sway);
			float displacementDiff = (displacement - previousDisplacement);
			displacement -= displacementDiff * (1 - scale); // bigger distance, less displacemenet
			displacement *= envelope;

			Vector2 point = source + // starting point
			                (pos * slope) + // which position on the start-end line
							(displacement * normal); // deviation from the line;
			
			activateLine (previousPoint, point, thickness);
			previousPoint = point;
			previousDisplacement = displacement;
		}

		// Connect the last segment
		activateLine (previousPoint, dest, thickness);
	}

	public void DeactivateAllSegments() {
		for (int i = ActiveLineObjects.Count - 1; i >= 0; i--) {
			GameObject line = ActiveLineObjects [i];
			line.SetActive (false);

			// Return all segments to inactive pool
			ActiveLineObjects.RemoveAt (i);
			InactiveLineObjects.Add (line);
		}
	}

	void activateLine(Vector2 A, Vector2 B, float thickness) {
		int inactiveCount = InactiveLineObjects.Count;
		if (inactiveCount <= 0)
			return; // Our pool is currently empty!
		GameObject lineObj = InactiveLineObjects[inactiveCount - 1];
		lineObj.SetActive (true);

		Line lineComponent = lineObj.GetComponent<Line> ();
		lineComponent.SetColor (Color.white);
		lineComponent.startPoint = A;
		lineComponent.endPoint = B;
		lineComponent.Thickness = thickness;

		// Take it from our pool, and make it alive!
		InactiveLineObjects.RemoveAt (inactiveCount - 1);
		ActiveLineObjects.Add (lineObj);
	}

	// If bolt is visible, draw it by drawing each active
	// line segment in corresponding alpha value
	public void Draw() {
		if (Alpha <= 0)
			return;

		foreach (GameObject lineObj in ActiveLineObjects) {
			Line lineComponent = lineObj.GetComponent<Line> ();
			lineComponent.SetColor (Tint * (Alpha * 0.6f));
			lineComponent.Draw ();
		}
	}

	public void UpdateBolt() {
		Alpha -= FadeOutRate;
	}

	// Returns the point where the bolt is at a given fraction
	// of the bolt (from 0 - start of the bolt, to 1 - end of the bolt
	public Vector2 GetPoint(float position) {
		Vector2 start = Start;
		Vector2 end = End;
		float length = Vector2.Distance (start, end);
		// Normalized direction from bolt start to bolt end
		Vector2 unitDirection = (end - start) / length;

		position *= length; // Lies somewhere on the imaginary line
							// between the beggining of the first line and
							// the end of the last line

		// Find a line - its projection (from bolt start to line end)
		// has to be at or further from position
		Line line = ActiveLineObjects.Find (
			            x => Vector2.Dot (x.GetComponent<Line> ().endPoint
			            - start, unitDirection) >= position).GetComponent<Line> ();
		// Line start projected onto direction line
		float lineStartPos = Vector2.Dot (line.startPoint - start, unitDirection);
		// Line end projected onto direction line
		// Because of predicate condition, line end lies at or further from
		// position
		float lineEndPos = Vector2.Dot (line.endPoint - start, unitDirection);
		// Factor between 0 and 1
		float linePos = (position - lineStartPos) / (lineEndPos - lineStartPos);

		// Return a point which lies on the selected line
		return Vector2.Lerp (line.startPoint, line.endPoint, linePos);
	}
}
