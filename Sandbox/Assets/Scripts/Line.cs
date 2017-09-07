using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

	public Vector2 startPoint;

	public Vector2 endPoint;

	public float Thickness;

	public GameObject StartCapChild, LineChild, EndCapChild;

	public Line(Vector2 startPoint, Vector2 endPoint, float thickness) {
		this.startPoint = startPoint;
		this.endPoint = endPoint;
		Thickness = thickness;
	}

	public void SetColor(Color color) {
		StartCapChild.GetComponent<SpriteRenderer> ().color = color;
		LineChild.GetComponent<SpriteRenderer> ().color = color;
		EndCapChild.GetComponent<SpriteRenderer> ().color = color;
	}

	public void Draw() {
		SpriteRenderer startSprite = LineChild.GetComponent<SpriteRenderer> ();
		SpriteRenderer lineSprite = LineChild.GetComponent<SpriteRenderer> ();
		SpriteRenderer endSprite = LineChild.GetComponent<SpriteRenderer> ();

		Vector2 difference = endPoint - startPoint;
		float rotation = Mathf.Atan2 (difference.y, difference.x) *
		                 Mathf.Rad2Deg;

		LineChild.transform.localScale = new Vector3 (
			100 * (difference.magnitude / lineSprite.sprite.rect.width),
			Thickness,
			LineChild.transform.localScale.z);

		StartCapChild.transform.localScale	= new Vector3 (
			StartCapChild.transform.localScale.x,
			Thickness,
			StartCapChild.transform.localScale.z);

		EndCapChild.transform.localScale = new Vector3 (
			EndCapChild.transform.localScale.x,
			Thickness,
			EndCapChild.transform.localScale.z);

		LineChild.transform.rotation = Quaternion.Euler (
			new Vector3 (0, 0, rotation));
		StartCapChild.transform.rotation = Quaternion.Euler (
			new Vector3 (0, 0, rotation));
		EndCapChild.transform.rotation = Quaternion.Euler (
			new Vector3 (0, 0, rotation));

		LineChild.transform.position = new Vector3 (
			startPoint.x, startPoint.y, LineChild.transform.position.z);
		StartCapChild.transform.position = new Vector3 (
			startPoint.x, startPoint.y, StartCapChild.transform.position.z);
		EndCapChild.transform.position = new Vector3 (
			startPoint.x, startPoint.y, EndCapChild.transform.position.z);

		rotation *= Mathf.Deg2Rad;

		float lineChildWorldAdjust = LineChild.transform.localScale.x *
		                             lineSprite.sprite.rect.width / 2f;
		float startCapWorldAdjust = StartCapChild.transform.localScale.x *
		                            startSprite.sprite.rect.width / 2f;
		float endCapWorldAdjust = EndCapChild.transform.localScale.x *
		                          endSprite.sprite.rect.width / 2f;

		// 1px sprite width = .01f world position
		LineChild.transform.position += new Vector3 (
			.01f * Mathf.Cos (rotation) * lineChildWorldAdjust,
			.01f * Mathf.Sin (rotation) * lineChildWorldAdjust,
			0);

		StartCapChild.transform.position -= new Vector3 (
			.01f * Mathf.Cos (rotation) * startCapWorldAdjust,
			.01f * Mathf.Sin (rotation) * startCapWorldAdjust,
			0);

		EndCapChild.transform.position += new Vector3 (
			.01f * Mathf.Cos (rotation) * lineChildWorldAdjust * 2,
			.01f * Mathf.Sin (rotation) * lineChildWorldAdjust * 2,
			0);
		EndCapChild.transform.position += new Vector3 (
			.01f * Mathf.Cos (rotation) * endCapWorldAdjust,
			.01f * Mathf.Sin (rotation) * endCapWorldAdjust,
			0);
	}
}
