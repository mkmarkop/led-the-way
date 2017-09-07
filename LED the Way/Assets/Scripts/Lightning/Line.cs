using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

	public Vector2 startPoint;

	public Vector2 endPoint;

	public float Thickness;

	public GameObject StartPart, MidPart, EndPart;

	// Pixels per unit
	public const float PPU = 100f;

	// Units per pixel
	public const float UPP = 1f / PPU;

	public Line(Vector2 start, Vector2 end, float thickness) {
		startPoint = start;
		endPoint = end;
		Thickness = thickness;
	}

	public void SetColor(Color color) {
		StartPart.GetComponent<SpriteRenderer> ().color = color;
		MidPart.GetComponent<SpriteRenderer> ().color = color;
		EndPart.GetComponent<SpriteRenderer> ().color = color;
	}

	public void Draw() {
		SpriteRenderer startRenderer = StartPart.GetComponent<SpriteRenderer> ();
		SpriteRenderer midRenderer = MidPart.GetComponent<SpriteRenderer> ();
		SpriteRenderer endRenderer = EndPart.GetComponent<SpriteRenderer> ();

		Vector2 direction = endPoint - startPoint;
		float rotation = Mathf.Atan2 (direction.y, direction.x) *
		                 Mathf.Rad2Deg;

		MidPart.transform.localScale = new Vector3 (
			PPU * (direction.magnitude / midRenderer.sprite.rect.width),
			Thickness,
			MidPart.transform.localScale.z);

		StartPart.transform.localScale = new Vector3 (
			StartPart.transform.localScale.x,
			Thickness,
			StartPart.transform.localScale.z);

		EndPart.transform.localScale = new Vector3 (
			EndPart.transform.localScale.x,
			Thickness,
			EndPart.transform.localScale.z);

		MidPart.transform.rotation = Quaternion.Euler (
			new Vector3 (0, 0, rotation));
		StartPart.transform.rotation = Quaternion.Euler (
			new Vector3 (0, 0, rotation));
		EndPart.transform.rotation = Quaternion.Euler (
			new Vector3 (0, 0, rotation));

		MidPart.transform.position = new Vector3 (
			startPoint.x, startPoint.y, MidPart.transform.position.z);
		StartPart.transform.position = new Vector3 (
			startPoint.x, startPoint.y, StartPart.transform.position.z);
		EndPart.transform.position = new Vector3 (
			startPoint.x, startPoint.y, EndPart.transform.position.z);

		rotation *= Mathf.Deg2Rad;

		// Calculate half-length of each part, in pixels
		// So far, each part is centered in startPoint

		// We have to move this part by half of its length so
		// its left point is at startPoint
		float midPartHalfLength = MidPart.transform.localScale.x *
		                           midRenderer.sprite.rect.width / 2f;
		float startPartHalfLength = StartPart.transform.localScale.x *
		                             startRenderer.sprite.rect.width / 2f;
		float endPartHalfLength = EndPart.transform.localScale.x *
		                           endRenderer.sprite.rect.width / 2f;
		
		MidPart.transform.position += new Vector3 (
			UPP * Mathf.Cos (rotation) * midPartHalfLength,
			UPP * Mathf.Sin (rotation) * midPartHalfLength,
			0);

		StartPart.transform.position -= new Vector3 (
			UPP * Mathf.Cos (rotation) * startPartHalfLength,
			UPP * Mathf.Sin (rotation) * startPartHalfLength,
			0);

		// Move the end part by the whole length of the middle part
		EndPart.transform.position += new Vector3 (
			UPP * Mathf.Cos (rotation) * midPartHalfLength * 2,
			UPP * Mathf.Sin (rotation) * midPartHalfLength * 2,
			0);
		EndPart.transform.position += new Vector3 (
			UPP * Mathf.Cos (rotation) * endPartHalfLength,
			UPP * Mathf.Sin (rotation) * endPartHalfLength,
			0);
	}
}
