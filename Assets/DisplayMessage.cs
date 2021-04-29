using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMessage : MonoBehaviour
{
    [SerializeField] private Animator emojiAnimation;
    [SerializeField] private AudioSource emojiSound;

    public void ShowEmojis(string message,AudioClip clip)
    {
        emojiSound.playOnAwake=false;
        this.gameObject.SetActive(true);
        emojiAnimation.Play(message);
        emojiSound.clip = clip;
        emojiSound.Play();
        StartCoroutine(stopEmoji(clip.length));
    } 

    IEnumerator stopEmoji(float length)
    {
        yield return new WaitForSeconds(1.5f);
        this.gameObject.SetActive(false);
    } 
}
