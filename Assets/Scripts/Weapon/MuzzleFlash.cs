using UnityEngine;
using System.Collections;

public class MuzzleFlash : MonoBehaviour 
{
	[Header("Values:")]
	[SerializeField] GameObject flashHolder;
	[SerializeField] Sprite[] flashSprites;
	[SerializeField] SpriteRenderer[] spriteRenderers;
	[SerializeField] float flashTime;

	void Start() => Deactivate();

    public void Activate() {
		flashHolder.SetActive (true);
		int flashSpriteIndex = Random.Range (0, flashSprites.Length);
		for (int i =0; i < spriteRenderers.Length; i ++) {
			spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
		}
		Invoke ("Deactivate", flashTime);
	}
	void Deactivate() => flashHolder.SetActive(false);
}
