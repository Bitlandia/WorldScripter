
using UnityEngine;
using System.Collections;

public class AudioHandlerFPS : MonoBehaviour
{

    public AudioSource myAudio;

    // Use this for initialization
    public void Run(string url)
    {
        WWW web = new WWW(url);
        UnityEngine.AudioClip www = WWWAudioExtensions.GetAudioClip(web); //Get the audio file
        myAudio = gameObject.AddComponent<AudioSource>();
        //Set a bunch of audo variables
        myAudio.playOnAwake = true;
        myAudio.minDistance = 1;
        myAudio.dopplerLevel = 0;
        myAudio.maxDistance = 50;
        myAudio.rolloffMode = AudioRolloffMode.Custom;
        myAudio.spatialBlend = 1f;
        myAudio.clip = www;
        myAudio.loop = false;
        Debug.Log(url);
        Debug.Log(myAudio.clip.loadState.ToString());
    }

    // Update is called once per frame
    void Update()
    {

    }
}