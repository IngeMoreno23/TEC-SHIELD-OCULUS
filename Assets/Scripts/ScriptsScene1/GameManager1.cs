using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;
using System.Collections;

public class GameManager1 : MonoBehaviour
{
    [Header("Camera Fade")]
    [SerializeField]
    private FadeCamera m_FadeCamera;

    [Header("GameDataSO")]
    [SerializeField]
    private UIGame1Data m_GameDataSO;

    [Header("Game Manager Settings")]
    [SerializeField]
    bool m_AbleToStart = false;
    public bool AbleToStart => m_AbleToStart; // Indicates if the game manager is able to start the game

    [SerializeField]
    GameObject m_SpherePrefab = null;

    [Header("Spheres Settings")]
    [SerializeField]
    [Range(1, 10)]
    int m_SphereCount = 8;

    [SerializeField]
    [Range(1, 10)]
    int m_CorrectSphereCount = 4;

    [Header("Simulation Settings")]
    [SerializeField]
    [Tooltip("Time in seconds to wait before starting the simulation")]
    [Range(1, 5)]
    int m_SimulationDelay = 5;

    [SerializeField]
    [Tooltip("Time in seconds to simulate the splines")]
    [Range(5, 20)]
    int m_SimulationTime = 10;

    [SerializeField]
    [Tooltip("Repetitions of the simulation")]
    [Range(1, 5)]
    int m_SimulationRepetitions = 3;

    int m_CurrentIteration = 0; // Current iteration of the simulation
    int m_SelectedSphereCount = 0; // Number of spheres currently being selected by the user in-game

    bool IsSelectionCompleted => m_SelectedSphereCount >= m_CorrectSphereCount; // Number of spheres in the simulation

    bool m_SaveData = false;

    struct SelectionData
    {   // The correct sphere count is determined if is under the number of spheres that are correct in the simulation
        public int BallId;  // ID of the selected ball
        public float SelectionDeltaTime;    // Time taken to select the ball
        public int PauseIndex;  // Index of the pause in the simulation
        public int ActionOrder; // Order of the action taken by the user for each Pause Index
        public bool IsSelected; // Indicates if the ball was selected or deselected
    }
    [Header("Stadistic Results")]
    [SerializeField]
    List<SelectionData> allSelections;

    [Header("Containers Settings")]
    [SerializeField]
    GameObject m_GameManagerContainer = null;

    [SerializeField]
    GameObject m_SphereContainer = null;

    [SerializeField]
    GameObject m_SplineContainer = null;

    [SerializeField]
    List<GameObject> m_Spheres = new List<GameObject>();

    [SerializeField]
    List<SplineContainer> m_splineContainers = new List<SplineContainer>();

    [Header("List for current selected spheres")]
    [SerializeField]
    List<int> m_CurrentSelectedSpheres = new List<int>();

    private void Awake()
    {
        ValidateData();
        if (!m_AbleToStart)
        {
            Debug.LogError("Game Manager is not able to start. Check the settings.");
            return;
        }

        for (int i = 0; i < m_SphereCount; i++)
        {
            GameObject sphere = Instantiate(m_SpherePrefab, new Vector3(0, 0, 0), Quaternion.identity, m_SphereContainer.transform);
            SelectableSphere1 selectableSphere = sphere.GetComponent<SelectableSphere1>();
            if (selectableSphere != null)
            {
                selectableSphere.Id = i;
                selectableSphere.OnSphereSelected += OnSphereSelected;
                if (i < m_CorrectSphereCount)
                {
                    m_CurrentSelectedSpheres.Add(i);
                }
            }
            else
            {
                Debug.LogError("SelectableSphere1 component is missing on the sphere prefab.");
            }
            SplineAnimate splineAnimate = sphere.GetComponent<SplineAnimate>();
            if (splineAnimate != null)
            {
                if (i < m_splineContainers.Count)
                {
                    SplineContainer splineContainer = m_splineContainers[i];
                    splineAnimate.Container = splineContainer;

                    Spline spline = splineContainer.Spline;
                    if (spline.Count > 0)
                    {
                        BezierKnot firstKnot = spline[0];
                        sphere.transform.position = splineContainer.transform.TransformPoint(firstKnot.Position);
                    }
                }
            }
            else
            {
                Debug.LogError("SplineAnimate component is missing on the sphere prefab.");
            }

            m_Spheres.Add(sphere);
        }
    }

    void Start()
    {
        m_FadeCamera.RedoFade();
        m_GameDataSO.maxIterations = m_SimulationRepetitions; // Set the maximum iterations for the game data
        m_GameDataSO.currentIteration = 0;
        m_GameDataSO.maxSelectedSpheres = m_CorrectSphereCount; // Set the maximum selected spheres for the game data
        m_GameDataSO.currentSelectedSpheres = 0;
    }


    private void Update()
    {
        m_GameDataSO.currentIteration = m_CurrentIteration;
        m_GameDataSO.currentSelectedSpheres = m_SelectedSphereCount;
    }
    private void OnDestroy()
    {
        foreach (GameObject sphere in m_Spheres)
        {
            if (sphere == null) continue;
            SelectableSphere1 selectableSphere = sphere.GetComponent<SelectableSphere1>();
            if (selectableSphere != null)
            {
                selectableSphere.OnSphereSelected -= OnSphereSelected;
            }
            Destroy(sphere);
        }
    }

    private void OnValidate()
    {
        ValidateData();
    }

    // Validates and creates the set of spheres and spline containers, does not create the splines nor vinculates them to the spheres. Refer to SplineBatchCreator.cs and Awake()
    private void ValidateData()
    {
        m_AbleToStart = false;

        #region Values in range and prefabs
        if (m_SpherePrefab == null)
        {
            Debug.LogError("Sphere Prefab is not assigned.");
            return;
        }

        if (m_SphereCount <= 0)
        {
            Debug.LogError("Sphere Count must be greater than 0.");
            return;
        }

        if (m_CorrectSphereCount <= 0)
        {
            Debug.LogError("Correct Sphere Count must be greater than 0.");
            return;
        }
        if (m_CorrectSphereCount > m_SphereCount)
        {
            Debug.LogError("Correct Sphere Count must be less than Sphere Count.");
            return;
        }

        if (m_SimulationDelay <= 0)
        {
            Debug.LogError("Simulation Delay must be greater than 0.");
            return;
        }

        if (m_SimulationTime <= 0)
        {
            Debug.LogError("Simulation Time must be greater than 0.");
            return;
        }

        if (m_SimulationRepetitions <= 0)
        {
            Debug.LogError("Simulation Repetitions must be greater than 0.");
            return;
        }

        if (m_GameDataSO == null)
        {
            Debug.LogError("GameDataSO is not assigned.");
            return;
        }
        m_GameDataSO.instructionsText = "";
        #endregion

        #region Containers and filling containers
        if (m_GameManagerContainer == null)
        {
            Debug.LogError("Game Manager Container is not assigned.");
            return;
        }
        if (m_SphereContainer == null)
        {
            Debug.LogError("Sphere Container is not assigned.");
            return;
        }
        if (m_SplineContainer == null)
        {
            Debug.LogError("Spline Container is not assigned.");
            return;
        }

        if (m_SplineContainer.transform.childCount != m_SphereCount)
        {
            Debug.LogError("Spline Container has different count as Sphere Count.");
            return;
        }

        if (m_Spheres.Count > 0)
        {
            foreach (GameObject sphere in m_Spheres)
            {
                Destroy(sphere);
            }
            m_Spheres.Clear();
        }

        if (m_splineContainers.Count > 0)
        {
            m_splineContainers.Clear();
        }

        for (int i = 0; i < m_SplineContainer.transform.childCount; i++)
        {
            SplineContainer splineContainer = m_SplineContainer.transform.GetChild(i).GetComponent<SplineContainer>();
            if (splineContainer != null)
            {
                m_splineContainers.Add(splineContainer);
            }
        }
        #endregion

        m_AbleToStart = true;
    }

    public IEnumerator StartGame()
    {
        if (!m_AbleToStart)
        {
            Debug.LogError("Game Manager is not able to start. Check the settings.");
            yield break;
        }
        m_AbleToStart = false; // Prevents the game from starting again

        m_FadeCamera.RedoFade();

        // Disabling the hover and selection of spheres

        m_GameManagerContainer.SetActive(true); // Enables the view of spheres in-game
        SetSpheresCanBeSelected(false);

        yield return new WaitForSeconds(1f);

        // Show Correct Spheres
        for (int i = 0; i < m_Spheres.Count; i++)
        {
            SelectableSphere1 selectableSphere = m_Spheres[i].GetComponent<SelectableSphere1>();
            if (selectableSphere != null)
            {
                if (m_CurrentSelectedSpheres.Contains(i))
                {
                    selectableSphere.ColorSelection(true);

                }
            }
        }

        for (int i = 0; i < m_SimulationDelay; i++)
        {
            m_GameDataSO.instructionsText = $"Iniciando en {m_SimulationDelay - i}";
            yield return new WaitForSeconds(1f);
        }

        // Hide Correct Spheres
        for (int i = 0; i < m_Spheres.Count; i++)
        {
            SelectableSphere1 selectableSphere = m_Spheres[i].GetComponent<SelectableSphere1>();
            if (selectableSphere != null)
            {
                if (m_CurrentSelectedSpheres.Contains(i))
                {
                    selectableSphere.ColorSelection(false);
                }
            }
        }
        m_CurrentSelectedSpheres.Clear();

        m_GameDataSO.instructionsText = "¡Iniciando simulación!";
        yield return new WaitForSeconds(1f);


        yield return GameSplinesLogic();

        Debug.Log("ACABAMOS");
    }

    private IEnumerator GameSplinesLogic()
    {
        // Repeat the simulation for the specified number of repetitions
        for (int i = 0; i < m_SimulationRepetitions; i++)
        {
            m_SelectedSphereCount = 0;
            m_CurrentIteration = i + 1;

            //  Call StartSplines to start the splines
            m_GameDataSO.instructionsText = "";
            StartSplines();
            //  Wait for the simulation time before stopping the splines
            yield return new WaitForSeconds(m_SimulationTime);
            //  Call StopSplines to stop the splines after the simulation time
            PauseSplines();
            m_GameDataSO.instructionsText = "¡Selecciona las esferas correctas!";
            SetSpheresCanBeSelected(true);
            m_SaveData = true;
            //  Wait for the user to select the spheres
            yield return new WaitUntil(() => IsSelectionCompleted);
            //  Reset the spheres checking SelectableSphere1.IsSelected 

            m_SaveData = false;
            SetSpheresCanBeSelected(false);

            yield return ShowCurrentSelections(1f);
            m_CurrentSelectedSpheres.Clear();

            yield return new WaitForSeconds(1f);
        }
        m_GameDataSO.instructionsText = "¡Fin!";
    }

    private IEnumerator ShowCurrentSelections(float time)
    {
        for (int i = 0; i < m_Spheres.Count; i++)
        {
            SelectableSphere1 selectableSphere = m_Spheres[i].GetComponent<SelectableSphere1>();
            if (selectableSphere != null)
            {
                if (m_CurrentSelectedSpheres.Contains(i))
                {
                    selectableSphere.ColorSelection(true);

                }
            }
        }
        yield return new WaitForSeconds(time);

        for (int i = 0; i < m_Spheres.Count; i++)
        {
            SelectableSphere1 selectableSphere = m_Spheres[i].GetComponent<SelectableSphere1>();
            if (selectableSphere != null)
            {
                if (m_CurrentSelectedSpheres.Contains(i))
                {
                    selectableSphere.ColorSelection(false);
                }
            }
        }
    }

    private void OnSphereSelected(int id, bool selectionState)
    {
        if (!m_SaveData) return;

        if (selectionState)
        {
            m_SelectedSphereCount++;
            m_CurrentSelectedSpheres.Add(id);
        }
        else
        {
            m_SelectedSphereCount--;
            m_CurrentSelectedSpheres.Remove(id);
        }

        //Debug.Log(m_SelectedSphereCount);
        //if (selectionState)
        //{
        //    string selectionType = id < m_CorrectSphereCount ? "Correct" : "Incorrect";
        //    Debug.Log($"Sphere {id} selected! {selectionType}");
        //}
        //else
        //{
        //    Debug.Log($"Sphere {id} deselected!");
        //}
        //Debug.Log($"{id} {selectionState}");
    }

    public void StartSplines()
    {

        for (int i = 0; i < m_Spheres.Count; i++)
        {
            SelectableSphere1 selectableSphere = m_Spheres[i].GetComponent<SelectableSphere1>();
            if (selectableSphere != null)
            {
                selectableSphere.StartSpline();
            }
        }
    }
    public void PauseSplines()
    {
        for (int i = 0; i < m_Spheres.Count; i++)
        {
            SelectableSphere1 selectableSphere = m_Spheres[i].GetComponent<SelectableSphere1>();
            if (selectableSphere != null)
            {
                selectableSphere.StopSpline();
            }
        }
    }

    void SetSpheresCanBeSelected(bool canBeSelected)
    {
        foreach (var sphere in m_Spheres)
        {
            var selectableSphere = sphere.GetComponent<SelectableSphere1>();
            if (selectableSphere != null)
            {
                selectableSphere.CanBeSelected(canBeSelected);
            }
        }
    }
}