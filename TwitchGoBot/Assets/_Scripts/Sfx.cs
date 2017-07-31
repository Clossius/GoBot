using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Sfx : MonoBehaviour {

	public AudioSource audioSource;
	public Toggle usingSound;
	public AudioClip click01;
	public AudioClip click02; 
	public AudioClip click03; 
	public AudioClip click04;
	public AudioClip click05;
	public AudioClip cancel;

	void PlaySound(AudioClip clip) {
		if(usingSound.isOn) {
			audioSource.clip = clip;
			audioSource.Play();
		}
	}

	public void LoginButtonClicked() {
		PlaySound(click01);
	}

	public void ButtonHoverEvent() {
		PlaySound(click02);
	}

	public void ButtonClicked() {
		PlaySound(click03);
	}

	public void SubmitTextEvent() {
		PlaySound(click04);
	}

	public void OnChatTextEdit() {
		PlaySound(click05);
	}

	public void CancelSound() {
		PlaySound(cancel);
	}

}
