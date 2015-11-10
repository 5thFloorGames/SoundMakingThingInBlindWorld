﻿using UnityEngine;
using System.Collections;

public class JosieController : MonoBehaviour {
	public float movementSpeed = 10;
	public float turningSpeed = 60;
	private AudioSource sound;

	void Start(){
		sound = gameObject.GetComponent<AudioSource> ();
	}

	void Update() {
		float horizontal = Input.GetAxis("Horizontal") * turningSpeed * Time.deltaTime;
		transform.Rotate(0, horizontal, 0);
		
		float vertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
		transform.Translate(0, 0, vertical);

		if (Input.GetButton ("Sound")) {
			sound.PlayOneShot(sound.clip);
		}
	}
}