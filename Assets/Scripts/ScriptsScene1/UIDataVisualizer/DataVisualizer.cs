using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class DataVisualizer : MonoBehaviour
{
    [SerializeField]
    UIDocument m_UIDocument;
    private VisualElement m_Root;
    private ScrollView m_Container;

    [Header("Data Visualizer")]
    [SerializeField]
    private DataTable m_DataTable = null;

    // Fixed the issue by removing the parentheses from Application.persistentDataPath
    [SerializeField]
    private string m_DataFilePath;

    private void Awake()
    {
        m_DataFilePath = Path.Combine(Application.persistentDataPath, "Simulation1_Data.json");
    }
    void Start()
    {
        m_Root = m_UIDocument.rootVisualElement;
        m_Container = m_Root.Q<ScrollView>("results-scroll-view");
        var button = m_Root.Q<Button>("save-button");
        button?.RegisterCallback<ClickEvent>(OnClickEvent);
    }

    void Update()
    {
    }

    public void OnClickEvent(ClickEvent evt)
    {
        Debug.Log("Click event triggered");
    }



    [ContextMenu("Add Visual Element")]
    public void AddVisualElement()
    {
        if (m_Container == null)
        {
            Debug.LogError("Container is null");
            return;
        }
        // Create a new visual element
        
        foreach (var item in m_DataTable.data)
        {
            // Check if the element does not exist
            VisualElement gameID = m_Container.Q<VisualElement>("game-result-" + item.gameID);
            if (gameID == null)
            {
                VisualElement newElement = new VisualElement();
                newElement.AddToClassList("game-result");
                newElement.name = "game-result-" + item.gameID;
                newElement.Add(new Label("Game ID: " + item.gameID));
                gameID = newElement;
            }
            gameID.Add(new Label("Ball ID: " + item.selectionData.BallId));
            gameID.Add(new Label("Selection Delta Time: " + item.selectionData.SelectionDeltaTime));
            gameID.Add(new Label("Pause Index: " + item.selectionData.PauseIndex));
            gameID.Add(new Label("Action Order: " + item.selectionData.ActionOrder));
            gameID.Add(new Label("Is Selected: " + item.selectionData.IsSelected));
            m_Container.Add(gameID);
        }

    }





    [ContextMenu("Generate Sample Table")]
    public void GenerateSampleTable()
    {
        if (m_DataTable == null)
        {
            m_DataTable = new DataTable();
        }

        // Generate 10 sample data rows
        for (int i = 0; i < 10; i++)
        {
            // Iterations
            List<SelectionData> selectionDataList = new List<SelectionData>();
            for (int j = 0; j < 12; j++)
            {
                int index = j % 4;
                int pauseIndex = j / 4;
                SelectionData selectionData = new SelectionData
                {
                    BallId = j,
                    SelectionDeltaTime = UnityEngine.Random.Range(0.1f, 1.0f),
                    PauseIndex = pauseIndex,
                    ActionOrder = j,
                    IsSelected = true
                };
                selectionDataList.Add(selectionData);
            }
            m_DataTable.AddRows(selectionDataList);
        }
        LogData();
        Debug.Log("Sample table generated");
    }
    public void LogData()
    {
        foreach(DataRow dataRow in m_DataTable.data)
        {
            Debug.Log(dataRow.gameID);
            Debug.Log(dataRow.selectionData.BallId);
            Debug.Log(dataRow.selectionData.SelectionDeltaTime);
            Debug.Log(dataRow.selectionData.PauseIndex);
            Debug.Log(dataRow.selectionData.ActionOrder);
            Debug.Log(dataRow.selectionData.IsSelected);
            Debug.Log("===================================");
        }
    }
    public void SaveData()
    {
        string json = JsonUtility.ToJson(m_DataTable, true);
        File.WriteAllText(m_DataFilePath, json);
        Debug.Log("Data saved to " + m_DataFilePath);
    }

    public void LoadData()
    {
        if (File.Exists(m_DataFilePath))
        {
            string json = File.ReadAllText(m_DataFilePath);
            m_DataTable = JsonUtility.FromJson<DataTable>(json);
            Debug.Log("Datos cargados");
        }
        else
        {
            Debug.LogWarning("No se encontró el archivo de datos.");
        }
    }

    private class DataTable
    {
        int serial_GameID = 0;
        public List<DataRow> data = new List<DataRow>();

        public DataTable()
        {
            this.serial_GameID = 0;
            this.data = new List<DataRow>();
        }

        public void AddRows(List<SelectionData> selectionData)
        {
            foreach (SelectionData sd in selectionData)
            {
                data.Add(new DataRow(serial_GameID, sd));
            }

            this.serial_GameID += 1;
        }
    }

    private class DataRow
    {
        public int gameID = 0;
        public SelectionData selectionData = new SelectionData();

        public DataRow(int gameID, SelectionData selectionData)
        {
            this.gameID = gameID;
            this.selectionData = selectionData;
        }
    }

    struct SelectionData
    {
        public int BallId;
        public float SelectionDeltaTime;
        public int PauseIndex;
        public int ActionOrder;
        public bool IsSelected;
    }
}
