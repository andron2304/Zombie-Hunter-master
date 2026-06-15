using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public bool canMove;
    public bool death;
    Animator anim;
    BoxCollider2D col;
    bool deathSoundPlayed = false;

    public float speed;

    // Use this for initialization
    void Start()
    {
        canMove = true;
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (death)
        {
            col.enabled = false;
            anim.SetTrigger("Death");

            // Play death SFX once when death is triggered
            if (!deathSoundPlayed)
            {
                if (SFXManager.Instance != null)
                {
                    SFXManager.Instance.PlayZombieDeath();
                }
                deathSoundPlayed = true;
            }

            if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Zombie_Death"))
            {
                Destroy(gameObject, 1f);
                death = false;
            }

            canMove = false;//Stop Movement

        }
        if (canMove)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }

    }

}
