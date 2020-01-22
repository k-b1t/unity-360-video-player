using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR;

// k/bit

public class VideoPlayer360 : MonoBehaviour {

    RenderTexture rendererTexture;
    Material skyboxMaterial;
    public VideoClip videoClip;
    public bool videoStopsWhenUserNotPresent = true;
    public bool useVideoResolution = true;
    public int rendererTextureWidth = 256;
    public int rendererTextureHeight = 256;
    public bool videoLoop = true;
    public bool muteSound = false;
    private bool playerPresent = false;
    VideoPlayer videoPlayer;

    void Start () {
        Application.runInBackground = true;
        // create texture renderrer
        if (useVideoResolution) {
            rendererTexture = new RenderTexture ((int) videoClip.width, (int) videoClip.height, 16, RenderTextureFormat.ARGB32);
        } else {
            rendererTexture = new RenderTexture (rendererTextureWidth, rendererTextureHeight, 16, RenderTextureFormat.ARGB32);
        }
        rendererTexture.Create ();

        // create skybox/panoram material  and set it in render settings
        skyboxMaterial = new Material (Shader.Find ("Skybox/Panoramic"));
        skyboxMaterial.mainTexture = rendererTexture;
        RenderSettings.skybox = skyboxMaterial;

        // create video player
        AudioSource audioSource = GetComponent<AudioSource> ();
        videoPlayer = gameObject.AddComponent<UnityEngine.Video.VideoPlayer> ();
        videoPlayer.playOnAwake = true;
        videoPlayer.isLooping = videoLoop;
        videoPlayer.clip = videoClip;
        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = rendererTexture;
        if (muteSound) {
            videoPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.None;
        } else {
            videoPlayer.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.Direct;
        }
    }

    void Update () {
        if (videoStopsWhenUserNotPresent && XRDevice.isPresent) {
            if (XRDevice.userPresence == UserPresenceState.Present && playerPresent == false) {
                videoPlayer.Play ();
                playerPresent = true;
            }

            if (XRDevice.userPresence == UserPresenceState.NotPresent && playerPresent == true) {
                videoPlayer.Stop ();
                playerPresent = false;
            }
        }
    }
}