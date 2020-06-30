using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioSource[] clips;
  


    public void play(int clip)
    {
        clips[clip].Play();
    }


}
