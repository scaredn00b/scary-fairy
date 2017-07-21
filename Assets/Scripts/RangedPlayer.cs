﻿using System.Collections;
using UnityEngine;

public class RangedPlayer : Player
{
    public Rigidbody2D arrow;
    private float timeToTravel = 1f;
    private bool firstAbility = false;

    // Use this for initialization
    private void Start()
    {
        base.Start();
        this.baseDamage = 10;
        this._hitpoints = 100;
        this.attackCD = 1f;
        animator = GetComponent<Animator>();
        particles = GetComponentInChildren<ParticleSystem>();
        particleSettings = particles.main;
        particles.Stop();
    }

    //An IEnumerator works similar to a function in this case (Coroutine), but you can pause with a yield
    //This function generates an arrow and then checks which way it should fly depending on the direction the player is facing
    protected override IEnumerator Attack()
    {
        CheckForEnchantment();
        isAttacking = true;
        isOnCoolDown[0] = true;
        yield return new WaitForSeconds(attackCD); //Waiting for the cooldown
        isOnCoolDown[0] = false;
    }

    protected override IEnumerator FirstAbility()
    {
        firstAbility = true;
        isOnCoolDown[1] = true;
        yield return new WaitForSeconds(5f);
        isOnCoolDown[1] = false;
    }

     // This function gets called from the animator(see animations events) to check if it is a normal attack or not
    private void GenArrows(int currentDir)
    {
        if(firstAbility) {
            multiShot(currentDir, 3, 15);
        }
        else
        {
            multiShot(currentDir, 1, 0);
        }
    }

    /*This function generates a amount of arrow depending of which angle the player is facing
     * parameter currentDir = facing direction, parameter count = how much arrows should spawn, degree = how much should those arrows be rotated per spawn
     */
    private void multiShot(int currentDir, int count, float degree)
    {
        isAttacking = false;
        firstAbility = false;
        int arrowCount = count;
        Rigidbody2D [] arrows = new Rigidbody2D[arrowCount];
        float degrees = degree;
        for (int i = 0; i < arrowCount; i++)
        {
            arrows[i] = arrow.GetComponent<Arrow>().createArrow(rb2D.position, transform.rotation, timeToTravel);
            Quaternion velocityAngle = Quaternion.Euler(0, 0, (i - arrowCount/2) * degrees);
            if (currentDir == 1)
            {
                Quaternion facingAngle = Quaternion.Euler(0, 0, 90 + (i - arrowCount/2) * degrees);
                arrows[i].transform.SetPositionAndRotation(transform.position, facingAngle);
                arrows[i].velocity = velocityAngle * transform.up * 10f;
            }
            if(currentDir == 2)
            {
                Quaternion facingAngle = Quaternion.Euler(0, 0, (i - arrowCount / 2) * degrees);
                arrows[i].transform.SetPositionAndRotation(transform.position, facingAngle);
                arrows[i].velocity = velocityAngle * transform.right * 10f;
            }
            if(currentDir == 3)
            {
                Quaternion facingAngle = Quaternion.Euler(0, 0, -90 + (i - arrowCount / 2) * degrees);
                arrows[i].transform.SetPositionAndRotation(transform.position, facingAngle);
                arrows[i].velocity = velocityAngle * -transform.up * 10f;
            }
            if(currentDir == 4)
            {
                Quaternion facingAngle = Quaternion.Euler(180, 0, 180 + (i - arrowCount / 2) * -degrees);
                arrows[i].transform.SetPositionAndRotation(transform.position, facingAngle);
                arrows[i].velocity = velocityAngle * -transform.right * 10f;
            }
        }
    }
}
