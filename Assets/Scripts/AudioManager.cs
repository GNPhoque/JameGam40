using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[SerializeField] AudioSource sfx;

	[SerializeField] public AudioSource audioWorkshop;
	[SerializeField] public AudioSource audioShop;
	[SerializeField] public AudioSource audioGraveyard;

	[SerializeField] float fadeInDuration;
	[SerializeField] float FadeOutDuration;
	float currentFadeInDuration;
	float currentFadeOutDuration;

	Coroutine CrossfadeInCoroutine;
	Coroutine CrossfadeOutCoroutine;

	public static AudioManager instance;
	private void Awake()
	{
		if (instance) Destroy(instance.gameObject);

		instance = this;
	}

	public void PlaySfx(AudioClip clip)
	{
		sfx.PlayOneShot(clip);
	}

	public void Crossfade(AudioSource[] fadeIn, AudioSource[] fadeOut)
	{
		if(CrossfadeInCoroutine != null)StopCoroutine(CrossfadeInCoroutine);
		if(CrossfadeOutCoroutine != null)StopCoroutine(CrossfadeOutCoroutine);
		
		CrossfadeInCoroutine = StartCoroutine(CrossfadeIn(fadeIn));
		CrossfadeOutCoroutine = StartCoroutine(CrossfadeOut(fadeOut));
	}

	public void CrossfadeToWorkshop()
	{
		Crossfade(new AudioSource[] { audioWorkshop }, new AudioSource[] { audioShop, audioGraveyard });
	}

	public void CrossfadeToShop()
	{
		Crossfade(new AudioSource[] { audioShop }, new AudioSource[] { audioWorkshop, audioGraveyard });
	}

	public void CrossfadeToGraveyard()
	{
		Crossfade(new AudioSource[] { audioGraveyard }, new AudioSource[] { audioShop, audioWorkshop });
	}

	IEnumerator CrossfadeIn(AudioSource[] sources)
	{
		float volumeIn = 0f;
		currentFadeInDuration = 0f;

		while (volumeIn < 1f)
		{
			currentFadeInDuration += Time.deltaTime;
			volumeIn = currentFadeInDuration / fadeInDuration;
			foreach (var source in sources)
			{
				source.volume = Mathf.Max(source.volume, volumeIn);
				yield return null;
			}
		}
	}

	IEnumerator CrossfadeOut(AudioSource[] sources)
	{
		float volumeOut = 1f;
		currentFadeOutDuration = FadeOutDuration;

		while (volumeOut > 0f)
		{
			currentFadeOutDuration -= Time.deltaTime;
			volumeOut = currentFadeOutDuration / FadeOutDuration;
			foreach (var source in sources)
			{
				source.volume = Mathf.Min(source.volume, volumeOut);
				yield return null;
			}
		}
	}
}
