﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObj
{
    public float maxVerticalDistance = 8.0f;
    public float maxHorizontalDistance = 12.0f;
    public float currentDistance;
    private GameObject[] players = new GameObject[2];
    private Vector2 horizontalMovement;
    private Vector2 verticalMovement;
    public string axisVertical;
    public string axisHorizontal;
    private Vector2 lastMove;
    protected int currentDir; // Current facing direction north(1), east(2), south(3), west(4)

    // Use this for initialization
    void Start()
    {
        base.Start();
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
    }

     protected override void Update ()
    {
        animator.SetFloat("MoveX", Input.GetAxisRaw(axisHorizontal));
        animator.SetFloat("MoveY", Input.GetAxisRaw(axisVertical));
        animator.SetBool("PlayerMoving", isMoving);
        animator.SetFloat("LastMoveX", lastMove.x);
        animator.SetFloat("LastMoveY", lastMove.y);
        animator.SetBool("PlayerAttack", isAttacking);
        animator.SetBool("IceEnchantment", iceEnchantment);
        animator.SetBool("FireEnchantment", fireEnchantment);
    }

    protected override Vector2 Move()
    {
        isMoving = false;
        horizontalMovement = new Vector2(0, 0);
        verticalMovement = new Vector2(0, 0);
        float axisH = Input.GetAxisRaw(axisHorizontal);
        float axisV = Input.GetAxisRaw(axisVertical);
        float nextDistance;

        if (axisH > 0.5f || axisH < -0.5f)
        {
            currentDistance = rb2D.position.x - CameraControl.camPosition.x;
            nextDistance = Vector2.Distance(rb2D.position + new Vector2(axisH * moveSpeed * Time.deltaTime, 0),
                CameraControl.camPosition);

            if (Mathf.Abs(currentDistance) < maxHorizontalDistance || (currentDistance > 0 && axisH < 0) || (currentDistance < 0 && axisH > 0))
            {
                horizontalMovement = new Vector2(axisH * moveSpeed * Time.deltaTime, 0);
                isMoving = true;
                lastMove = new Vector2(axisH, 0);
                if (axisH > 0)
                {
                    currentDir = 2;
                }
                else
                {
                    currentDir = 4;
                }
            }
        }

        if (axisV > 0.5f || axisV < -0.5f)
        {
            currentDistance = rb2D.position.y - CameraControl.camPosition.y;
            //movement possible if player below distance threshold or moving towards other player
            if (Mathf.Abs(currentDistance) < maxVerticalDistance || (currentDistance > 0 && axisV < 0) || (currentDistance < 0 && axisV > 0))
            {
                verticalMovement = new Vector2(0, axisV * moveSpeed * Time.deltaTime);
                isMoving = true;
                lastMove = new Vector2(0, axisV);
                if (axisV > 0)
                {
                    currentDir = 1;
                }
                else
                {
                    currentDir = 3;
                }
            }
        }
        newPos = rb2D.position + horizontalMovement + verticalMovement;
        Debug.DrawLine(rb2D.position, rb2D.position + lastMove);
        return newPos;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttacking == true && other.CompareTag("CasualEnemy"))
        {
            CasualEnemy ce = other.GetComponent<CasualEnemy>();
            ce.applyDamage(_damage);
            print("Enemy attacked");
            if (iceEnchantment)
            {
                print("IceEnchanted Attack");
            }
            if (fireEnchantment)
            {
                print("FireEnchanted Attack");
            }
            if (!iceEnchantment && !fireEnchantment)
            {
                print("normal Attack");
            }
        }
    }
}
