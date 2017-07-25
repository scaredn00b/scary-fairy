﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class AIMove : AIBase, IObserver {

    private Transform target;

    public float updateRate = 0.5f;

    public float updatePlayerRate = 3f;

    public Path path;

    public bool canMove = true;

    public bool canSearch = true;

    public float speed = 300f;

    public ForceMode2D forceMode;

    [HideInInspector]
    public bool pathIsEnded = false;

    public float nextWaypointDistance = 0.5f;

    private int currentWaypoint = 0;

    private GameObject[] targets = new GameObject[2];



    private void Init()
    {
        Subject.AddObserver(this);
        getPlayers();
        StartCoroutine(RepeatTrySearchPlayer());
        StartCoroutine(RepeatTrySearchPath());
    }

    public void OnNotify(string gameEvent)
    {
        switch (gameEvent)
        {
            case "Player changed class":
                    getPlayers();
                    SearchPlayer();
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        Subject.RemoveObserver(this);
        seeker.CancelCurrentPathRequest();
    }

    private void Start()
    {
        Init();
    }

    public void getPlayers()
    {
        targets = GameObject.FindGameObjectsWithTag("Player");
    }

    private IEnumerator RepeatTrySearchPath()
    {
        while (true)
        {
            yield return new WaitForSeconds(TrySearchPath());
        }
    }

    private IEnumerator RepeatTrySearchPlayer()
    {
        while (true) yield return new WaitForSeconds(TrySearchPlayer());
    }

    private float TrySearchPath()
    {
        if (canSearch && target != null && this.gameObject.activeInHierarchy)
        {
            UpdatePath();
        }
        return updateRate;
    }

    private float TrySearchPlayer()
    {
        if (canSearch && this.gameObject.activeInHierarchy)
        {
            getPlayers();
            SearchPlayer();
        }
        return updatePlayerRate;
    }

    private void SearchPlayer()
    {
        float closestDistance = float.MaxValue;
        int targetIndex = -1;
        for (int i = 0; i < targets.Length; i++)
        {
            if(target == null)
            {
                getPlayers();
            }
            float distanceToCurrPlayer = Math.Abs(Vector3.Distance(transform.position, targets[i].transform.position));
            if (closestDistance > distanceToCurrPlayer)
            {
                closestDistance = distanceToCurrPlayer;
                targetIndex = i;
            }
        }
        target = targets[targetIndex].transform;
    }

    //Gets called if a path is available
    private void OnPathComplete(Path p)
    {
        Debug.Log("We got a path. Error?" + p.error);
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void UpdatePath()
    {
        seeker.StartPath(transform.position, target.position, OnPathComplete); //Calcs a path to the target
    }

    protected override void MovementUpdate(float deltaTime)
    {
        if(target == null)
        {
            return;
        }

        if(path == null)
        {
            return;
        }

        if (!canMove)
        {
            return;
        }

        if(currentWaypoint >= path.vectorPath.Count)
        {
            if (pathIsEnded)
            {
                return;
            }
            Debug.Log("End of path reached");
            pathIsEnded = true;
            return;
        }
        pathIsEnded = false;

        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;
        rigid2D.AddForce(dir, forceMode); //This script uses addForce to move objects -> high speed & linear drag need to be set

        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if(dist < nextWaypointDistance) //Have we reached our next waypoint?
        {
            currentWaypoint++;
        }
    }

    public Vector3 getDirection()
    {
        if (canMove)
        {
            Vector3 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            return direction;
        }
        else return Vector3.zero;
    }

    public void hitByIceEnchantment()
    {
        StartCoroutine(applyIceSlow());
    }

    protected IEnumerator applyIceSlow()
    {
        speed = speed * 0.5f;
        yield return new WaitForSeconds(1f);
        speed = speed / 0.5f;
    }
}
