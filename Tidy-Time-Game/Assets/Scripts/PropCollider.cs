/*
This class detects if a player is inside of a prop collider and allows them to press the respective button
Attached to: Prop Collider Objects
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropCollider : MonoBehaviour
{
    private Button interactButton; // Reference to the button component
    private bool isPlayerInCollider = false;

    // Start is called before the first frame update
    private void Start()
    {
        // Get button from child component
        interactButton = GetComponentInChildren<Button>();
        // Disable the button initially
        if (interactButton != null)
        {
            interactButton.interactable = false;
        }
    }
    // Update is called once per frame
    private void Update()
    {
        // If player is inside the collider and presses "Enter", press the button
        if (isPlayerInCollider && Input.GetKeyDown(KeyCode.Return))
        {
            interactButton.onClick.Invoke();
        }
    }

    // If inside collider, enable button
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (interactButton != null)
            {
                interactButton.interactable = true;
            }
            isPlayerInCollider = true;
        }
    }
    // If outside collider, disable button
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (interactButton != null)
            {
                interactButton.interactable = false;
            }
            isPlayerInCollider = false;
        }
    }
}