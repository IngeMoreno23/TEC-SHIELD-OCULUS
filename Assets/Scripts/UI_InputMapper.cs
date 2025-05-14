using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
public class UI_InputMapper : MonoBehaviour
{
    UIDocument m_UIDocument;
    InputSystem_Actions m_InputSystemActions;
    [SerializeField]
    private XRRayInteractor m_XRRayInteractor;
    [SerializeField]
    private GameObject m_UIPanelObject;
    private void OnEnable()
    {
        m_UIDocument = GetComponent<UIDocument>();
        m_InputSystemActions = new InputSystem_Actions();
        m_InputSystemActions.Enable();

        if (m_UIPanelObject == null)
        {
            m_UIPanelObject = gameObject;
        }
        m_UIDocument.panelSettings.SetScreenToPanelSpaceFunction(
            (Vector2 screenPosition) =>
            {
                var invalidPosition = new Vector2(float.NaN, float.NaN);
                if (m_XRRayInteractor == null)
                {
                    Debug.LogWarning("XRRayInteractor no asignado.");
                    return invalidPosition;
                }
                Vector3 origin = m_XRRayInteractor.rayOriginTransform.position;
                Vector3 direction = m_XRRayInteractor.rayOriginTransform.forward;
                Ray interactorRay = new Ray(origin, direction);
                // Debug.DrawRay(origin, direction * 100, Color.magenta);
                if (!Physics.Raycast(interactorRay, out RaycastHit hit, 100f, LayerMask.GetMask("UI")))
                {
                    // Debug.Log("Invalid position");
                    return invalidPosition;
                }
                //Debug.Log("Hit: " + hit.collider.gameObject.name);
                // IMPORTANTE: Verificar que el objeto golpeado es realmente ESTE panel de UI
                // Comprobamos si el objeto golpeado es este panel o un hijo de este panel
                if (hit.collider.gameObject != m_UIPanelObject && !IsChildOf(hit.collider.gameObject, m_UIPanelObject))
                {
                    // El rayo golpeó otro panel de UI, no este
                    return invalidPosition;
                }
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.y = 1 - pixelUV.y;
                pixelUV.x *= this.m_UIDocument.panelSettings.targetTexture.width;
                pixelUV.y *= this.m_UIDocument.panelSettings.targetTexture.height;
                var cursor = this.m_UIDocument.rootVisualElement.Q<VisualElement>("cursor");
                if (cursor != null)
                {
                    cursor.style.left = pixelUV.x / this.m_UIDocument.panelSettings.targetTexture.width * this.m_UIDocument.rootVisualElement.resolvedStyle.width;
                    cursor.style.top = pixelUV.y / this.m_UIDocument.panelSettings.targetTexture.height * this.m_UIDocument.rootVisualElement.resolvedStyle.height;
                }
                return pixelUV;
            }
        );
    }

    private bool IsChildOf(GameObject child, GameObject parent)
    {
        Transform childTransform = child.transform;
        while (childTransform != null)
        {
            if (childTransform.gameObject == parent)
                return true;
            childTransform = childTransform.parent;
        }
        return false;
    }

    private void OnDisable()
    {
        m_InputSystemActions.Disable();
    }
}

