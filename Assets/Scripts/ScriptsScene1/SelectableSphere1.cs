using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable), typeof(SplineAnimate))]
public class SelectableSphere1 : MonoBehaviour
{
    [SerializeField]
    private XRSimpleInteractable m_Interactable;

    [SerializeField]
    private SplineAnimate m_SplineAnimate;

    public event Action<int, bool> OnSphereSelected;

    private bool m_IsSelected = false;

    public bool IsSelected
    {
        get => m_IsSelected;
        private set => m_IsSelected = value;
    }

    [SerializeField]
    private int m_Id = 0;
    public int Id
    {
        get => m_Id;
        set => m_Id = value;
    }

    [Header("Shaders OnHover Outline")]
    [SerializeField]
    private MeshRenderer m_MeshRenderer;

    [SerializeField]
    private Material m_LitMaterial = null;
    [SerializeField]
    private Material m_OutlineMaterial = null;

    private List<Material> m_BaseMaterials = new List<Material>();
    private List<Material> m_HoverMaterials = new List<Material>();

    [Header("Selection Color")]
    [SerializeField]
    private Color m_SelectionColor;


    private void Awake()
    {
        m_Interactable = GetComponent<XRSimpleInteractable>();
        if (m_Interactable == null)
        {
            Debug.LogError("XRSimpleInteractable component is missing.");
            return;
        }
        m_SplineAnimate = GetComponent<SplineAnimate>();
        if (m_SplineAnimate == null)
        {
            Debug.LogError("SplineAnimate component is missing.");
            return;
        }

        m_Interactable.activated.AddListener(OnActivated);
        m_Interactable.hoverEntered.AddListener(OnHoverEnter);
        m_Interactable.hoverExited.AddListener(OnHoverExit);

        m_MeshRenderer = GetComponent<MeshRenderer>();
        if (m_MeshRenderer == null)
        {
            Debug.LogError("No MeshRenderer component found on this GameObject.");
            return;
        }

        // Instantiate new materials to avoid modifying the original ones
        m_LitMaterial = new Material(m_LitMaterial);
        m_BaseMaterials.Add(m_LitMaterial);
        m_HoverMaterials.Add(m_LitMaterial);
        m_HoverMaterials.Add(m_OutlineMaterial);
    }

    private void OnDestroy()
    {
        if (m_Interactable == null) return;

        m_Interactable.activated.RemoveListener(OnActivated);
        m_Interactable.hoverEntered.RemoveListener(OnHoverEnter);
        m_Interactable.hoverExited.RemoveListener(OnHoverExit);
    }

    private void OnActivated(ActivateEventArgs args)
    {
        ToggleSelection();
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        SetHover(true);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        SetHover(false);
    }

    public void StartSpline()
    {
        m_SplineAnimate.Play();
    }

    public void StopSpline()
    {
        m_SplineAnimate.Pause();
    }

    public void ToggleSelection()
    {
        m_IsSelected = !m_IsSelected;
        SetActive(m_IsSelected);
    }

    public void CanBeSelected(bool canBeSelected)
    {
        if (canBeSelected)
        {
            m_Interactable.enabled = true;
        }
        else
        {
            m_Interactable.enabled = false;
            // Disable selection
            SetActive(false);
            SetHover(false);
        }
    }

    void SetActive(bool enable)
    {
        m_IsSelected = enable;
        ColorSelection(m_IsSelected);
        OnSphereSelected?.Invoke(m_Id, m_IsSelected);
    }

    void SetHover(bool enable)
    {
        if (enable)
        {
            m_MeshRenderer.SetMaterials(m_HoverMaterials);
        }
        else
        {
            m_MeshRenderer.SetMaterials(m_BaseMaterials);
        }
    }

    public void ColorSelection(bool enable)
    {
        if (enable)
        {
            m_LitMaterial.color = m_SelectionColor;
        }
        else
        {
            m_LitMaterial.color = Color.gray;
        }
    }
}
