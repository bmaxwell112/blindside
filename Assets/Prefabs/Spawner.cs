﻿using System.Collections; using System.Collections.Generic; using UnityEngine;  public class Spawner : MonoBehaviour {      public GameObject[] enemyPrefab;     public int spawnRate;      // Use this for initialization     void Start() {         Invoke("SpawnEnemies", spawnRate);     } 	 	// Update is called once per frame 	void Update () { 		 	}      void SpawnEnemies()     {         int rand = Random.Range(0, 3);         GameObject attacker = Instantiate(enemyPrefab[rand], Vector3.zero, Quaternion.identity, transform) as GameObject;         attacker.transform.parent = transform;         attacker.transform.position = transform.position;         Invoke("SpawnEnemies", spawnRate);     }  } 