﻿using UnityEngine;
using System.Collections;

public class HiroController : MonoBehaviour {
	private float speed = 2.0f;
	private bool rotating = false;
	private bool moving = false;
	private bool falling = false;
	private float rotSpeed = 175.0f;
	private Vector3 target;
	private AudioSource sound;
	private float rotation = 0.0f;
	private Quaternion qTo = Quaternion.identity;
	private int wallMask = 0;
	private AudioClip[] forwardJumps;
	private AudioClip[] backwardJumps;
	private AudioClip[] turnRight;
	private AudioClip[] turnLeft;
	private AudioClip[] bumps;
	private AudioClip[] fallingSounds;
	private AudioClip[] creaks;
	private AudioClip[] forwardHollow;
	private AudioClip[] backwardHollow;

	[SerializeField]
	private float basicSoundVolume = 0.4f;
	[SerializeField]
	private float spawnVolume = 0.5f;
	[SerializeField]
	private float clickVolume = 0.5f;
	[SerializeField]
	private float creakVolume = 0.5f;
	
	private AudioClip spawn;
	private AudioClip click;
	private Animator animator;
	
	private bool onHollow = false;

	void Awake(){
		//lightSource = GetComponentInChildren<Light> ();
		sound = GetComponentInChildren<AudioSource> ();
		forwardJumps = Resources.LoadAll<AudioClip>("Audio/Hiro/Forward");
		backwardJumps = Resources.LoadAll<AudioClip>("Audio/Hiro/Backward");
		turnRight = Resources.LoadAll<AudioClip>("Audio/Hiro/TurnRight");
		turnLeft = Resources.LoadAll<AudioClip>("Audio/Hiro/TurnLeft");
		bumps = Resources.LoadAll<AudioClip>("Audio/Hiro/Bump");
		fallingSounds = Resources.LoadAll<AudioClip>("Audio/Hiro/Falling");
		creaks = Resources.LoadAll<AudioClip>("Audio/Hiro/Creak");
		forwardHollow = Resources.LoadAll<AudioClip>("Audio/Hiro/ForwardHollow");
		backwardHollow = Resources.LoadAll<AudioClip>("Audio/Hiro/BackwardHollow");
		spawn = Resources.Load<AudioClip>("Audio/Actions/start");
		click = Resources.Load<AudioClip>("Audio/Actions/Click/Click");
		animator = GetComponent<Animator> ();
	}
	
	// Use this for initialization
	void Start () {
		wallMask |= 1 << LayerMask.NameToLayer ("Wall");
		SpawnSound ();
	}
	
	private void PlayRandomSound(AudioClip[] clips){
		PlayRandomSound(clips,basicSoundVolume);
	}
	
	private void PlayRandomSound(AudioClip[] clips, float volume){
		sound.PlayOneShot (clips[Random.Range(0,clips.Length)], volume);
	}
	
	public void SpawnSound(){
		sound.PlayOneShot (spawn, spawnVolume);
	}
	
	public void Click(){
		sound.PlayOneShot (click, clickVolume);
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < 0.45 && !falling) {
			falling = true;
			PlayRandomSound(fallingSounds, 0.8f);
		}
		
		if (transform.position.y > 0.45 && falling) {
			falling = false;
		}
		
		if (rotating) {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, qTo, rotSpeed * Time.deltaTime);
			if(Quaternion.Angle(transform.rotation, qTo) < 0.1){
				rotating = false;
			}
		}
		if (moving) {
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, target, step);
			if(transform.position == target){
				moving = false;
				onHollow = checkHollow(transform.position);
			}
		}
		
		if (!rotating && !moving) {
			if (Input.GetButtonDown ("TurnRight")) {
				animator.SetTrigger("TurnRight");
				rotation += 90f;
				qTo = Quaternion.Euler(0.0f, rotation, 0.0f);
				Rotate (rotSpeed);
				if(onHollow){
					PlayRandomSound(creaks, creakVolume);
				}
				PlayRandomSound(turnRight);
			} else if (Input.GetButtonDown ("TurnLeft")) {
				animator.SetTrigger("TurnLeft");
				rotation -= 90f;
				qTo = Quaternion.Euler(0.0f, rotation, 0.0f);
				Rotate (-rotSpeed);
				if(onHollow){
					PlayRandomSound(creaks, creakVolume);
				}
				PlayRandomSound(turnLeft);
			}

			if(!moving){
				if (Input.GetAxis ("HiroMovementMac") < 0 || Input.GetAxis("HiroMovementPC") > 0.5) {
					bool endHollow = checkHollow(transform.position + transform.forward);
					if(Physics.Raycast (transform.position + transform.up, transform.forward, 1.25f, wallMask)){
						PlayBump();
					} else {
						setTarget(transform.position + transform.forward);
						animator.SetTrigger("Jump");
						if(onHollow){
							PlayRandomSound(creaks, creakVolume);
						}
						if(endHollow){
							PlayRandomSound(forwardHollow);
						} else {
							PlayRandomSound(forwardJumps);
						}
					}
				}
				if (Input.GetAxis ("HiroMovementMac") > 0 || Input.GetAxis("HiroMovementPC") < -0.5) {
					bool endHollow = checkHollow(transform.position + transform.forward * (-1));
					
					if(Physics.Raycast (transform.position + transform.up, transform.forward * (-1), 1.25f, wallMask)){
						// make this play only once
						PlayBump();
					} else {
						setTarget(transform.position + transform.forward * (-1));
						animator.SetTrigger("JumpBack");
						if(onHollow){
							PlayRandomSound(creaks, creakVolume);
						}
						if(endHollow){
							PlayRandomSound(backwardHollow);
						} else {
							PlayRandomSound(backwardJumps);
						}
					}
				}
			}
		}
	}
	
	private bool checkHollow(Vector3 position){
		RaycastHit hit;
		Debug.DrawRay(position + transform.up * (0.5f), transform.up * (-1),Color.green, 1.5f);
		Physics.Raycast (position + transform.up * (0.5f), transform.up * (-1), out hit);
		return hit.transform.tag == "Hollow";
	}
	
	void PlayBump(){
		if (!sound.isPlaying) {
			sound.clip = bumps[Random.Range(0,bumps.Length)];
			sound.Play();
		}
	}
	
	void Rotate(float speed) {
		rotating = true;
	}
	
	void setTarget(Vector3 transform){
		target = transform;
		moving = true;
	}
	
	public void reset(){
		rotating = false;
		moving = false;
		onHollow = false;
	}
}