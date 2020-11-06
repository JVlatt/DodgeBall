using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    public List<AnimationClip> animations;
    private bool _launched = false;
    private float timerAnim;
    private int animIndex;
    private Animator _anim;    
    void Start()
    {
        _anim = GetComponent<Animator>();
        animIndex = Random.Range(0, animations.Count);
        timerAnim = Random.Range(0, animations[animIndex].length) / animations[animIndex].length;
        _anim.Play("Base Layer." + animations[animIndex].name, 0, timerAnim);
    }
}
