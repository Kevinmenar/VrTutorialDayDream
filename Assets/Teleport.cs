using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
	public GameObject ScreenCover;
	private Camera _camera;
	private Renderer _coverRenderer; // our material that contains our color
	private float _fadeInTime; // time to fade in our screen cover
	private float _fadeOutTime; // time to fade out our screen cover
	private float _timeInDark; // time we wait before we fade back in
	private float _time; // keep track of time
	private Boolean _isFading;

	Vector3 currentTargetPos;

	void Start () {
		_camera = Camera.main;
		_coverRenderer = ScreenCover.GetComponent<Renderer>();
		_fadeInTime = 0.2f;
		_fadeOutTime = 0.2f;
		_timeInDark = 0.4f;
		_time = 0f;
		_isFading = false;
		 
		// set our starting color to be be 0 or transparent
		SetCoverAlpha(0f);

	}
		
	void Update ()
	{
		// Only shoot when we're not in the middle of teleporting
		if (GvrControllerInput.ClickButtonDown && !_isFading)
		{
			Shoot();
		}
	}

		
	private void Shoot()
	{
		// shoot a raycast from the center of our screen
		Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit; // output variable to get what we collided against
		if (Physics.Raycast(ray, out hit))
		{
			if (hit.transform != null)
			{
				// set our location to the point we hit
				StartCoroutine(FadeIn(hit));
			}
		}
	}
	// Coroutine to cover the player's screen
	private IEnumerator FadeIn(RaycastHit hit)
	{
		_isFading = true;
		_time = 0f;
		while (_time < _fadeInTime)
		{   
			Fade(0, 1, _fadeInTime); // 1 is opaque, 0 is transparent
			yield return null; // wait until the next frame
		}
		// now that the screen is covered, set our location to the point we hit
		print("Current position X is " + hit.point.x);
		print("Current position Z is " + hit.point.z);

		Vector3 newLocation = new Vector3(hit.point.x, 1, hit.point.z);
		transform.position = newLocation;

		print("Current transform.position X is " + transform.position.x);
		print("Current transform.position Z is " + transform.position.z);

		yield return new WaitForSeconds(_timeInDark); // wait in the dark
		StartCoroutine(FadeOut()); // start fading away the cover
	}

	// Coroutine to remove the cover from the player's screen
	private IEnumerator FadeOut()
	{
		_time = 0f;
		while (_time < _fadeOutTime)
		{
			Fade(1, 0, _fadeOutTime); // 1 is opaque, 0 is transparent
			yield return null; // wait until the next frame
		}
		_isFading = false;
	}

	// Helper function to change the alpha of our screen cover
	private void Fade(float start, float end, float fadeTime)
	{
		_time += Time.deltaTime;
		float currentAlpha = Mathf.Lerp(start, end, _time / fadeTime);
		SetCoverAlpha(currentAlpha);
	}

	// Helper function to change the alpha of our cover material. We have to
	// change the material directly, we can't hold a reference to the color variable
	private void SetCoverAlpha(float alpha)
	{
		Color color = _coverRenderer.material.color;
		color.a = alpha;
		_coverRenderer.material.color = color;
	}

}
