using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Animation Data", menuName = "Scriptable Objects/Animation Data", order = 1)]
public class AnimationData : ScriptableObject
{
    public static float targetFrameTime = 0.0167f;
    public int frameOfGap;
    public Sprite[] sprites;
    public bool loop;
    public GameManager.soundsNames[] sounds;

    /*[System.Serializable]
    public class FrameEvent
    {
        //which frame of animation the event starts on
        public int frameIndex;
        //number identifier to reference in code
        public string eventName;
    }

    public FrameEvent[] frameEvents;*/
}
