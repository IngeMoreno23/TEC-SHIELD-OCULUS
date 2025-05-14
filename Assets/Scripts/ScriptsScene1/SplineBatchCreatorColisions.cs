#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;

public class SplineBatchCreatorColisions : EditorWindow
{
    [Header("Settings")]
    private GameObject m_Containter = null;
    private int m_splineCount = 8;
    private int m_knotCount = 50;

    private Vector2 m_XRange = new Vector2(-5f, 5f);
    private Vector2 m_YRange = new Vector2(-3f, 3f);
    private Vector2 m_ZRange = new Vector2(-0.5f, 0.5f);

    // Spatial Distribution Settings
    private bool m_useSpatialControl = true;
    private float m_minDistanceBetweenKnots = 0.75f;
    private int m_maxAttemptsPerKnot = 30;
    private int m_gridDensity = 8;
    private bool m_showAdvancedSettings = false;
    private float m_crossingMinDistance = 1.0f;
    private int m_maxCrossingsAllowed = 3;

    // For tracking spatial distribution
    private List<Vector3>[,,] m_spatialGrid;
    private List<Vector3> m_allKnotPositions = new List<Vector3>();

    [MenuItem("Tools/Spline Batch Creator Colisions")]
    public static void ShowWindow()
    {
        GetWindow<SplineBatchCreatorColisions>("Spline Batch Creator Colisions");
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

        EditorGUILayout.Space();
        GUILayout.Label("Control de Distribución Espacial", EditorStyles.boldLabel);
        m_useSpatialControl = EditorGUILayout.Toggle("Usar Control Espacial", m_useSpatialControl);

        if (m_useSpatialControl)
        {
            m_minDistanceBetweenKnots = EditorGUILayout.Slider("Distancia Mínima Entre Nudos", m_minDistanceBetweenKnots, 0.1f, 3.0f);

            m_showAdvancedSettings = EditorGUILayout.Foldout(m_showAdvancedSettings, "Configuración Avanzada");
            if (m_showAdvancedSettings)
            {
                EditorGUI.indentLevel++;
                m_gridDensity = EditorGUILayout.IntSlider("Densidad de la Cuadrícula", m_gridDensity, 2, 15);
                m_maxAttemptsPerKnot = EditorGUILayout.IntSlider("Intentos Máximos por Nudo", m_maxAttemptsPerKnot, 5, 50);
                m_crossingMinDistance = EditorGUILayout.Slider("Distancia Mínima en Cruces", m_crossingMinDistance, 0.1f, 2.0f);
                m_maxCrossingsAllowed = EditorGUILayout.IntSlider("Cruces Máximos Permitidos", m_maxCrossingsAllowed, 1, 10);
                EditorGUI.indentLevel--;
            }
        }

        GUI.enabled = Validation();

        EditorGUILayout.Space();
        if (GUILayout.Button("Crear Splines en la Escena"))
        {
            CreateSplines(m_splineCount);
        }
    }

    private void CreateSplines(int count)
    {
        InitializeSpatialGrid();

        for (int i = 0; i < count; i++)
        {
            GameObject splineObj = new GameObject("Spline" + (i));
            var container = splineObj.AddComponent<SplineContainer>();
            var spline = new Spline();

            List<Vector3> knotPositions = new List<Vector3>();

            for (int j = 0; j < m_knotCount; j++)
            {
                Vector3 position;
                if (m_useSpatialControl)
                {
                    position = GenerateSpatiallyDistributedPosition(knotPositions, j);
                }
                else
                {
                    position = new Vector3(
                        Random.Range(m_XRange.x, m_XRange.y),
                        Random.Range(m_YRange.x, m_YRange.y),
                        Random.Range(m_ZRange.x, m_ZRange.y));
                }

                knotPositions.Add(position);
                spline.Add(new BezierKnot(position));

                // Registrar posición para el control espacial global
                if (m_useSpatialControl)
                {
                    AddPositionToSpatialGrid(position);
                }
            }

            spline.SetTangentMode(TangentMode.AutoSmooth);
            container.Spline = spline;
            container.transform.SetParent(m_Containter.transform);
            container.transform.localPosition = Vector3.zero;

            Undo.RegisterCreatedObjectUndo(splineObj, "Crear Spline");
        }

        Debug.Log($"{count} Splines creados con distribución espacial controlada.");
    }

    private void InitializeSpatialGrid()
    {
        m_allKnotPositions.Clear();

        m_spatialGrid = new List<Vector3>[m_gridDensity, m_gridDensity, m_gridDensity];

        for (int x = 0; x < m_gridDensity; x++)
        {
            for (int y = 0; y < m_gridDensity; y++)
            {
                for (int z = 0; z < m_gridDensity; z++)
                {
                    m_spatialGrid[x, y, z] = new List<Vector3>();
                }
            }
        }
    }

    private Vector3 GenerateSpatiallyDistributedPosition(List<Vector3> currentSplinePositions, int knotIndex)
    {
        // Para el primer nudo de un spline, podemos ser más aleatorios
        if (knotIndex == 0)
        {
            for (int attempt = 0; attempt < m_maxAttemptsPerKnot; attempt++)
            {
                Vector3 position = new Vector3(
                    Random.Range(m_XRange.x, m_XRange.y),
                    Random.Range(m_YRange.x, m_YRange.y),
                    Random.Range(m_ZRange.x, m_ZRange.y));

                if (IsPositionValid(position, null))
                {
                    return position;
                }
            }
        }
        // Para los demás nudos, intentamos mantener una cierta continuidad
        else
        {
            Vector3 previousPosition = currentSplinePositions[knotIndex - 1];

            // Dirección de movimiento si hay un nudo anterior
            Vector3 direction = (knotIndex > 1) ?
                (previousPosition - currentSplinePositions[knotIndex - 2]).normalized :
                Random.onUnitSphere;

            float stepSize = m_minDistanceBetweenKnots * 0.8f;

            for (int attempt = 0; attempt < m_maxAttemptsPerKnot; attempt++)
            {
                // Añadimos algo de aleatoriedad a la dirección pero manteniendo la tendencia
                Vector3 randomizedDirection = (direction + Random.insideUnitSphere * 0.3f).normalized;

                // Distancia aleatoria pero manteniendo una separación mínima
                float distance = Random.Range(stepSize, stepSize * 2.0f);

                Vector3 candidatePosition = previousPosition + randomizedDirection * distance;

                // Nos aseguramos de que el punto esté dentro de los límites
                candidatePosition.x = Mathf.Clamp(candidatePosition.x, m_XRange.x, m_XRange.y);
                candidatePosition.y = Mathf.Clamp(candidatePosition.y, m_YRange.x, m_YRange.y);
                candidatePosition.z = Mathf.Clamp(candidatePosition.z, m_ZRange.x, m_ZRange.y);

                if (IsPositionValid(candidatePosition, currentSplinePositions))
                {
                    return candidatePosition;
                }

                // Si no encontramos una posición válida, aumentamos la aleatoriedad
                stepSize *= 1.1f;
            }
        }

        // Si después de muchos intentos no encontramos una posición válida, usamos una aleatoria
        // pero aumentando ligeramente los límites para tener más opciones
        return new Vector3(
            Random.Range(m_XRange.x * 1.1f, m_XRange.y * 1.1f),
            Random.Range(m_YRange.x * 1.1f, m_YRange.y * 1.1f),
            Random.Range(m_ZRange.x * 1.1f, m_ZRange.y * 1.1f));
    }

    private bool IsPositionValid(Vector3 position, List<Vector3> currentSpline)
    {
        // Verificar si está dentro de los límites
        if (position.x < m_XRange.x || position.x > m_XRange.y ||
            position.y < m_YRange.x || position.y > m_YRange.y ||
            position.z < m_ZRange.x || position.z > m_ZRange.y)
        {
            return false;
        }

        // Verificar la distancia mínima con otros nudos ya existentes
        foreach (Vector3 existingPos in m_allKnotPositions)
        {
            if (Vector3.Distance(position, existingPos) < m_minDistanceBetweenKnots)
            {
                return false;
            }
        }

        // Si estamos generando un spline, verificamos que no tenga demasiados cruces
        if (currentSpline != null && currentSpline.Count > 1)
        {
            int crossingsCount = 0;
            Vector3 previousKnot = currentSpline[currentSpline.Count - 1];

            // Verificar cruces potenciales con otros splines
            for (int i = 1; i < m_allKnotPositions.Count; i++)
            {
                if (i % m_knotCount == 0) continue; // Saltar el primer nudo de cada spline

                Vector3 otherKnot = m_allKnotPositions[i];
                Vector3 otherPreviousKnot = m_allKnotPositions[i - 1];

                // Calcular la distancia mínima entre los dos segmentos
                float distance = MinimumDistanceBetweenSegments(
                    previousKnot, position,
                    otherPreviousKnot, otherKnot);

                if (distance < m_crossingMinDistance)
                {
                    crossingsCount++;
                    if (crossingsCount > m_maxCrossingsAllowed)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private void AddPositionToSpatialGrid(Vector3 position)
    {
        m_allKnotPositions.Add(position);

        // Calcular índices de la cuadrícula
        int gridX = Mathf.FloorToInt((position.x - m_XRange.x) / (m_XRange.y - m_XRange.x) * (m_gridDensity - 1));
        int gridY = Mathf.FloorToInt((position.y - m_YRange.x) / (m_YRange.y - m_YRange.x) * (m_gridDensity - 1));
        int gridZ = Mathf.FloorToInt((position.z - m_ZRange.x) / (m_ZRange.y - m_ZRange.x) * (m_gridDensity - 1));

        // Asegurarse de que estamos dentro de los límites
        gridX = Mathf.Clamp(gridX, 0, m_gridDensity - 1);
        gridY = Mathf.Clamp(gridY, 0, m_gridDensity - 1);
        gridZ = Mathf.Clamp(gridZ, 0, m_gridDensity - 1);

        // Añadir la posición a la celda correspondiente
        m_spatialGrid[gridX, gridY, gridZ].Add(position);
    }

    // Calcula la distancia mínima entre dos segmentos de línea
    private float MinimumDistanceBetweenSegments(Vector3 s1p0, Vector3 s1p1, Vector3 s2p0, Vector3 s2p1)
    {
        Vector3 u = s1p1 - s1p0;
        Vector3 v = s2p1 - s2p0;
        Vector3 w = s1p0 - s2p0;

        float a = Vector3.Dot(u, u);
        float b = Vector3.Dot(u, v);
        float c = Vector3.Dot(v, v);
        float d = Vector3.Dot(u, w);
        float e = Vector3.Dot(v, w);

        float D = a * c - b * b;
        float sc, tc;

        // Caso paralelo o casi paralelo
        if (D < 0.0001f)
        {
            sc = 0.0f;
            tc = (b > c ? d / b : e / c);
        }
        else
        {
            sc = (b * e - c * d) / D;
            tc = (a * e - b * d) / D;
        }

        // Clamp sc y tc al rango [0, 1]
        sc = Mathf.Clamp01(sc);
        tc = Mathf.Clamp01(tc);

        // Calcular los puntos más cercanos
        Vector3 dP = w + (sc * u) - (tc * v);

        return dP.magnitude;
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