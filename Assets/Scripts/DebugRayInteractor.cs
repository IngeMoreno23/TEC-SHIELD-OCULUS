using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

public class DebugRayInteractor : MonoBehaviour
{
    private XRBaseInteractable interactable;

    private void Awake()
    {
        // Get the XRSimpleInteractable component
        interactable = GetComponent<XRBaseInteractable>();

        if (interactable == null)
        {
            Debug.LogError("No XRBaseInteractable component found on this GameObject.");
            return;
        }

        // Subscribe to interaction events
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        interactable.selectEntered.AddListener(OnSelectEnter);
        interactable.selectExited.AddListener(OnSelectExit);
        interactable.activated.AddListener(OnActivated);
        interactable.deactivated.AddListener(OnDeactivated);
        interactable.focusEntered.AddListener(OnFocusEnter);
        interactable.focusExited.AddListener(OnFocusExit);
    }

    private void OnDestroy()
    {
        if (interactable == null) return;

        // Unsubscribe from interaction events
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
        interactable.selectEntered.RemoveListener(OnSelectEnter);
        interactable.selectExited.RemoveListener(OnSelectExit);
        interactable.activated.RemoveListener(OnActivated);
        interactable.deactivated.RemoveListener(OnDeactivated);
        interactable.focusEntered.RemoveListener(OnFocusEnter);
        interactable.focusExited.RemoveListener(OnFocusExit);
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        Debug.Log("Hover Entered");
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        Debug.Log("Hover Exited");
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("Select Entered");
    }

    private void OnSelectExit(SelectExitEventArgs args)
    {
        Debug.Log("Select Exited");
    }

    private void OnFocusEnter(FocusEnterEventArgs args)
    {
        Debug.Log("Focus Entered");
    }

    private void OnFocusExit(FocusExitEventArgs args)
    {
        Debug.Log("Focus Exited");
    }

    private void OnActivated(ActivateEventArgs args)
    {
        Debug.Log("Activated");
    }

    private void OnDeactivated(DeactivateEventArgs args)
    {
        Debug.Log("Deactivated");
    }

}