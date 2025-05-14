using Unity.Properties;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "UIGame1Data", menuName = "Scriptable Objects/UIGame1Data")]
public class UIGame1Data : ScriptableObject
{
    public int currentIteration;
    public int maxIterations;

    public int currentSelectedSpheres;
    public int maxSelectedSpheres;

    [CreateProperty]
    public string currentIterationText => $"{currentIteration} / {maxIterations}";
    [CreateProperty]
    public string currentSelectedSphereText => $"{currentSelectedSpheres} / {maxSelectedSpheres}";

    public string instructionsText = "";
}
