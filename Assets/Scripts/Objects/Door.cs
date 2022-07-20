using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorType
{
    key,
    enemy,
    button
}

public class Door : Interactable
{
    [Header("Door variables")]
    public DoorType thisDoorType;
    public bool open = false;
    public Inventory playerInventory;
    public SpriteRenderer doorSprite;
    public BoxCollider2D physicsColliders;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(playerInRange)
            {
                //Check Key
                if(playerInRange && thisDoorType == DoorType.key)
                {
                    //If player have key
                    if(playerInventory.numberOfKeys > 0)
                    {
                        //Remove player key
                        playerInventory.numberOfKeys--;
                        //Open
                        Open();
                    }
                }
            }
        }
    }

    public void Open()
    {
        //Remove door
        doorSprite.enabled = false;
        //setbool open = true
        open = true;
        //remove collider
        physicsColliders.enabled = false;
    }

    public void Close()
    {

    }
}
