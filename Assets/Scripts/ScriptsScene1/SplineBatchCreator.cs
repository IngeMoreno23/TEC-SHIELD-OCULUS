#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;

public class SplineBatchCreator : EditorWindow
{
    [Header("Settings")]

    private GameObject m_Containter = null;

    private int m_splineCount = 8;
    private int m_knotCount = 50;

    private Vector2 m_XRange = new Vector2(-5f, 5f);
    private Vector2 m_YRange = new Vector2(-3f, 3f);
    private Vector2 m_ZRange = new Vector2(-0.5f, 0.5f);


    [MenuItem("Tools/Spline Batch Creator")]
    public static void ShowWindow()
    {
        GetWindow<SplineBatchCreator>("Spline Batch Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Crear múltiples Splines", EditorStyles.boldLabel);

        m_Containter = EditorGUILayout.ObjectField("Contenedor (Padre de los Splines generados)", m_Containter, typeof(GameObject), true) as GameObject;

        m_splineCount = EditorGUILayout.IntField("Cantidad de Splines", m_splineCount);
        m_knotCount = EditorGUILayout.IntField("Cantidad de Nudos por Spline", m_knotCount);

        m_XRange = EditorGUILayout.Vector2Field("Rango en X", m_XRange);
        m_YRange = EditorGUILayout.Vector2Field("Rango en Y", m_YRange);
        m_ZRange = EditorGUILayout.Vector2Field("Rango en Z", m_ZRange);

        GUI.enabled = Validation();

        if (GUILayout.Button("Crear Splines en la Escena"))
        {
            CreateSplines(m_splineCount);
        }
    }

    private void CreateSplines(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject splineObj = new GameObject("Spline" + (i));
            var container = splineObj.AddComponent<SplineContainer>();

            // Opcional: agregar 2 puntos por spline para que no estén vacíos
            var spline = new Spline();

            for (int j = 0; j < m_knotCount; j++)
            {
                Vector3 position = new Vector3(
                    Random.Range(m_XRange.x, m_XRange.y),
                    Random.Range(m_YRange.x, m_YRange.y),
                    Random.Range(m_ZRange.x, m_ZRange.y));
                spline.Add(new BezierKnot(position));
            }

            spline.SetTangentMode(TangentMode.AutoSmooth);

            container.Spline = spline;

            container.transform.SetParent(m_Containter.transform);
            container.transform.localPosition = Vector3.zero;

            Undo.RegisterCreatedObjectUndo(splineObj, "Crear Spline");
        }

        Debug.Log($"{count} Splines creados.");
    }

    private bool Validation()
    {
        bool state = true;

        if (m_Containter == null)
        {
            state = false;
            EditorGUILayout.HelpBox("El contenedor no puede ser nulo.", MessageType.Error);
        }

        if (m_splineCount <= 0)
        {
            state = false;
            EditorGUILayout.HelpBox("La cantidad de splines debe ser mayor a 0.", MessageType.Error);
        }

        if (m_knotCount <= 0)
        {
            state = false;
            EditorGUILayout.HelpBox("La cantidad de nudos por spline debe ser mayor a 0.", MessageType.Error);
        }

        if (m_XRange.x >= m_XRange.y)
        {
            state = false;
            EditorGUILayout.HelpBox("El rango en X no es válido.", MessageType.Error);
        }

        if (m_YRange.x >= m_YRange.y)
        {
            state = false;
            EditorGUILayout.HelpBox("El rango en Y no es válido.", MessageType.Error);
        }

        if (m_ZRange.x >= m_ZRange.y)
        {
            state = false;
            EditorGUILayout.HelpBox("El rango en Z no es válido.", MessageType.Error);
        }

        if (m_Containter != null && m_Containter.transform.childCount > 0)
        {
            state = false;
            EditorGUILayout.HelpBox("El contenedor no puede tener hijos.", MessageType.Error);
        }

        return state;
    }
}
#endif