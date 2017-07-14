﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Npc : MovingObj
{
    private Vector2 startMarker;
    private Vector2 endMarker;
    private float journeyLength;
    private bool isKnockedBack = false;
    protected AIMove AI;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        AI = GetComponent<AIMove>(); //Simple caching of component -> better performance
    }

    protected void FixedUpdate()
    {
        if (isKnockedBack && Math.Abs(Vector2.Distance(startMarker, rb2D.position)) > journeyLength) //IsKnockedBack used for performance improvement, if first argument is false, no more calc is need (distance needs cpu power)
        {
            rb2D.velocity = Vector2.zero; //Stop rigidbody from moving
            isKnockedBack = false;
            AI.canMove = true; //AI can now move again
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected void OnCollisionStay2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("CasualEnemy"))
        {
            other.gameObject.GetComponent<MovingObj>().applyDamage(_damage);
            rb2D.bodyType = RigidbodyType2D.Kinematic; //Set rigidbody to kinematic to prevent player from pushing enemy
            rb2D.velocity = Vector2.zero;
        }
    }

    protected void OnCollisionExit2D(Collision2D collision)
    {
        rb2D.bodyType = RigidbodyType2D.Dynamic; //Set rigidbody dynamic again;
    }



    public void knockBack(Vector2 force, float playerMass)
    {
        startMarker = rb2D.position;
        endMarker = startMarker + force;
        journeyLength = Math.Abs(Vector2.Distance(startMarker, endMarker));
        rb2D.velocity = Vector2.zero;
        rb2D.AddForce(force,ForceMode2D.Impulse); //Add force in direction using an impulse
        rb2D.velocity = rb2D.velocity * playerMass; //If a player is heavier, knockBack further
        AI.canMove = false; //Prevent AI from moving instead
        isKnockedBack = true;
    }
}
