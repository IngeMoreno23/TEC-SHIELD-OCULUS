#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager1))]
public class GameManager1Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager1 manager = (GameManager1)target;

        if (GUILayout.Button("Correr Spline"))
        {
            manager.StartSplines();
        }
        if (GUILayout.Button("Detener Spline"))
        {
            manager.PauseSplines();
        }

        if (GUILayout.Button("Iniciar Simulación"))
        {
            manager.StartGame();
        }
    }
}
#endif