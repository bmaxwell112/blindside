﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float minPos, maxPos, speed, stikeDistance, coolDownRateInSeconds, walkDistance;
    public int escapeNumber, damage, hp;
    public GameObject number, controlPanel, teleport;
    public AudioClip punchHit, punchMiss, punchButton;
    public AudioClip[] hurtSound;
    [HideInInspector]
    public int buttonPressed = 0;

    private bool left, coolDown, youShallNotPass, win;    
    private LevelManager lvl;
    private BarricadeManager BarricadeReference;
    private SpriteRenderer sprite;
    private Animator anim, button;
    private AudioSource audioSource;
    private ControlPanel cPanel;
    private float volume;


    private void Start()
    {
        GameManager.volume = PlayerPrefsManager.GetSFXVolume();
        lvl = FindObjectOfType<LevelManager>();
        BarricadeReference = GameObject.FindWithTag("Barricade").GetComponent<BarricadeManager>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        button = controlPanel.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = GameManager.volume;
        cPanel = FindObjectOfType<ControlPanel>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "enemy")
        youShallNotPass = true;
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "enemy")
        {
            youShallNotPass = false;
        }
    }

    // Update is called once per frame
    void Update () {
        InputCheck();        
    }

    void InputCheck()
    {
        if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x >= minPos)
        {
            transform.position += new Vector3(-speed, 0, 0) * Time.deltaTime;
            left = true;
            sprite.flipX = true;
            anim.SetInteger("animState", 1);
        }
        if (Input.GetKey(KeyCode.RightArrow) && transform.position.x <= maxPos)
        {
            if (!youShallNotPass)
            {
                transform.position += new Vector3(speed, 0, 0) * Time.deltaTime;
            }
            left = false;
            sprite.flipX = false;
            anim.SetInteger("animState", 1);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !left && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            AttackOrBarricade();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && left && transform.position.x <= minPos + 0.5)
        {
            if (cPanel.active)
            {
                ControlPanelPressed();
            }
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            anim.SetInteger("animState", 0);
        }
        if (win)
        {
            WinFunction();
        }
    }

    private void ControlPanelPressed()
    {
        if (!coolDown && !win)
        {
            buttonPressed++;
            PlayAudio(punchButton);
            SpawnNumber(buttonPressed, transform.position);
            if (buttonPressed >= escapeNumber)
            {
                Instantiate(teleport);
                GetComponent<BoxCollider2D>().enabled = false;
                win = true;
            }
            button.SetTrigger("press");
            coolDown = true;
            anim.SetInteger("animState", 2);
            Invoke("AttackCoolDown", coolDownRateInSeconds);
        }
    }

    void WinFunction()
    {
        sprite.color -= new Color(0, 0, 0, 0.75f) * Time.deltaTime;
        if (sprite.color.a <= 0)
        {
            lvl.LoadLevel("03a Win");
        }
    }

    void AttackOrBarricade()
    {
        if(!coolDown)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), (Vector2.right/30), stikeDistance);
            if (hit)
            {      
                if(hit.collider.gameObject.CompareTag("enemy"))
                {
                    print("hit enemy");
                    PlayAudio(punchHit);
                    hit.collider.gameObject.GetComponent<EnemyAI>().DamageEnemy(damage);
                    SpawnNumber(damage, hit.collider.gameObject.transform.position);                    
                }                        

                if(hit.collider.gameObject.tag == "Barricade")
                {
                    print("build barricade");
                    BarricadeReference.BuildBarricade();
                }
            }
            else
            {
                PlayAudio(punchMiss);
                youShallNotPass = false;
            }
            coolDown = true;
            anim.SetInteger("animState", 2);
            Invoke("AttackCoolDown", coolDownRateInSeconds);
        }
    }

    public void DamagePlayer(int damage)
    {
        hp -= damage;        
        if (hp <= 0)
        {
            lvl.LoadLevel("03b Lose");
        }
        else
        {
            int rand = UnityEngine.Random.Range(0, 5);
            PlayAudio(hurtSound[rand]);
            sprite.color = Color.red;
            Invoke("ColorReset", 0.25f);
        }
    }

    void SpawnNumber(int value, Vector3 spawnPos)
    {
        GameObject numClone = Instantiate(number, transform.position + (Vector3.up*3), Quaternion.identity) as GameObject;
        numClone.GetComponent<NumberDisplay>().SetNumber(value);
        numClone.transform.position = spawnPos + (Vector3.up * 2);
    }

    void AttackCoolDown()
    {
        coolDown = false;
        anim.SetInteger("animState", 0);
    }
    void ColorReset()
    {
        sprite.color = Color.white;
    }
    void PlayAudio(AudioClip thisClip)
    {
        audioSource.clip = thisClip;
        audioSource.Play();
    }   
}
