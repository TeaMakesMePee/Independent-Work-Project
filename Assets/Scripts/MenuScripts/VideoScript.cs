using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoScript : MonoBehaviour
{
    public List<VideoClip> vClips;
    public void PlayVideo(GameData.Division theDiv)
    {
        switch (theDiv)
        {
            case GameData.Division.P_Tank:
                GetComponent<VideoPlayer>().clip = vClips[0];
                break;
            case GameData.Division.P_Damage:
                GetComponent<VideoPlayer>().clip = vClips[1];
                break;
            case GameData.Division.P_Flank:
                GetComponent<VideoPlayer>().clip = vClips[2];
                break;
        }
        GetComponent<VideoPlayer>().Play();
    }

    public void StopVideo()
    {
        GetComponent<VideoPlayer>().Stop();
    }
}
