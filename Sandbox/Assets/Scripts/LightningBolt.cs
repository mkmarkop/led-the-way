using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBolt : MonoBehaviour {

	public List<GameObject> ActiveLineObj;
	public List<GameObject> InactiveLineObj;

	public GameObject LinePrefab;

	public float Alpha { get; set; }
	public float FadeOutRate { get; set; }
	public Color Tint { get; set; }
	public Vector2 Start {
		get {
			return ActiveLineObj [0].GetComponent<Line>().startPoint;
		}
	}
	public Vector2 End {
		get {
			return ActiveLineObj [ActiveLineObj.Count - 1].GetComponent<Line> ().startPoint;
		}
	}
	public bool IsComplete { get { return Alpha <= 0; } }

	// How many line segments can the bolt use
	public void Initialize(int maxSegments) {
		ActiveLineObj = new List<GameObject> ();
		InactiveLineObj = new List<GameObject> ();

		for (int i = 0; i < maxSegments; i++) {
			GameObject line = (GameObject)GameObject.Instantiate (LinePrefab);
			line.transform.parent = transform;
			line.SetActive (false);
			InactiveLineObj.Add (line);
		}
	}

	// Create jaged lines
	public void ActivateBolt(Vector2 source, Vector2 dest,
		Color color, float thickness) {
		Tint = color;
		Alpha = 1.5f;
		FadeOutRate = 0.03f;

		if (Vector2.Distance (dest, source) <= 0) {
			Vector2 adjust = Random.insideUnitCircle;
			if (adjust.magnitude <= 0)
				adjust.x += .1f;
			dest += adjust;
		}

		// Slope between two points
		Vector2 slope = dest - source;
		Vector2 normal = (new Vector2 (slope.y, -slope.x)).normalized;

		float distance = slope.magnitude;
		List<float> positions = new List<float> ();
		positions.Add (0);

		// Choose a number of random positions, and scale them
		// between 0 and 1, startPoint and endPoint.
		for (int i = 0; i < distance / 4; i++) {
			positions.Add (Random.Range (.25f, .75f));
		}

		// Sort the positions so we can easily add line
		// segments between them.
		positions.Sort ();

		const float Sway = 120;
		const float Jaggedness = 1 / Sway;

		// Control how far the segments deviate from the slope of our line
		// Spread of 0 => straight line
		float spread = 1f;

		Vector2 prevPoint = source;
		float prevDisplacement = 0;

		for (int i = 1; i < positions.Count; i++) {
			int inactiveCount = InactiveLineObj.Count;
			if (inactiveCount <= 0)
				break;

			float pos = positions [i];
			// Factor to avoid overly sharp angles
			float scale = (distance * Jaggedness) * (pos - positions [i - 1]);
			// Factor to ensure the lightning goes to the destination point
			// Limit the displacement
			float envelope = pos > 0.95f ? 20 * (1 - pos) : spread;
			float displacement = Random.Range (-Sway, Sway);
			displacement -= (displacement - prevDisplacement) * (1 - scale);
			displacement *= envelope;

			Vector2 point = source + (pos * slope) + (displacement * normal);
			activateLine (prevPoint, point, thickness);
			prevPoint = point;
			prevDisplacement = displacement;
		}

		activateLine (prevPoint, dest, thickness);
	}

	/// <summary>
	/// Deactivates all the segments.
	/// </summary>
	public void DeactivateSegments() {
		for (int i = ActiveLineObj.Count - 1; i >= 0; i--) {
			GameObject line = ActiveLineObj [i];
			line.SetActive (false);
			ActiveLineObj.RemoveAt (i);
			InactiveLineObj.Add (line);
		}
	}

	// Pick a line from a pool of inactive objects,
	// set its points and activate it.
	void activateLine(Vector2 A, Vector2 B, float thickness) {
		int inactivateCount = InactiveLineObj.Count;
		if (inactivateCount <= 0) return;
		GameObject line = InactiveLineObj [inactivateCount - 1];
		line.SetActive (true);

		Line lineComponent = line.GetComponent<Line> ();
		lineComponent.SetColor (Color.white);
		lineComponent.startPoint = A;
		lineComponent.endPoint = B;
		lineComponent.Thickness = thickness;
		InactiveLineObj.RemoveAt (inactivateCount - 1);
		ActiveLineObj.Add (line);
	}

	/// <summary>
	/// Update bolt's color on the screen.
	/// </summary>
	public void Draw() {
		if (Alpha <= 0)
			return;

		foreach (GameObject obj in ActiveLineObj) {
			Line lineComponent = obj.GetComponent<Line> ();
			lineComponent.SetColor (Tint * (Alpha * 0.6f));
			lineComponent.Draw ();
		}
	}

	/// <summary>
	/// Make the bolt fade out.
	/// </summary>
	public void UpdateBolt() {
		Alpha -= FadeOutRate;
	}

	// Return the point where the bolt is at
	// Position -> [0, 1]
	public Vector2 GetPoint(float position) {
		Vector2 start = Start;
		float length = Vector2.Distance (start, End);
		Vector2 dir = (End - start) / length;
		position *= length;

		Line line = ActiveLineObj.Find (x =>
			Vector2.Dot (x.GetComponent<Line> ().endPoint -
		            start, dir) >= position).GetComponent<Line> ();
		float lineStart = Vector2.Dot (line.startPoint - start, dir);
		float lineEnd = Vector2.Dot (line.endPoint - start, dir);
		float linePos = (position - lineStart) / (lineEnd - lineStart);

		return Vector2.Lerp (line.startPoint, line.endPoint, linePos);
	}
}
