using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundController : MonoBehaviour
{
    public static SoundController instance { get; private set; }
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip m_click, m_dice, m_move, m_ladder, m_snake;


    private void Awake()
    {
        instance = this;
    }


    public void PlayClick()
    {
        sfxSource.PlayOneShot(m_click);
    }
    public void PlayDice()
    {
        sfxSource.PlayOneShot(m_dice);
    }
    public void PlayMove()
    {
        sfxSource.PlayOneShot(m_move);
    }
    public void PlayLadder()
    {
        sfxSource.PlayOneShot(m_ladder);
    }
    public void PlaySnake()
    {        
        sfxSource.PlayOneShot(m_snake);
    }

    
}

