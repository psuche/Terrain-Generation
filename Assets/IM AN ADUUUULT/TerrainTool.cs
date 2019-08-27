using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TerrainTool : EditorWindow
{
    public string tName = string.Empty;

    private int tWidth = 128;
    private int tLength = 128;
    private int tHeight = 17;
    private bool autoUpdate = false;
    private GameObject tMterial;

    public static bool tUseOffset = false;
    public static float tOffsetX = 0;
    public static float tOffsetY = 0;

    public static float tScale = 16;

    public bool tt = false;

    private string path = "Assets/TerrainData/";
    private string TerrainToolTag = "TerrainToolGenerated";

    [MenuItem("Window/TerrainTool")]
    public static void ShowWindow()
    {
        GetWindow<TerrainTool>();
    }

    private void OnSelectionChange()
    {
        if (Selection.activeGameObject != null)
        {
            if (Selection.activeGameObject.tag == TerrainToolTag)
            {
                tName = Selection.activeGameObject.name;
            }
            else
            {
                tName = string.Empty;
            }
        }
        else
        {
            tName = string.Empty;
        }
        Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        autoUpdate = EditorGUILayout.Toggle("Auto Update", autoUpdate);

        EditorGUILayout.Space();

        tName = EditorGUILayout.TextField("Terrain Name:", tName);

        EditorGUILayout.Space();

        tWidth = EditorGUILayout.IntField("Terrain Width", tWidth);
        tLength = EditorGUILayout.IntField("Terrain Lenght", tLength);
        tHeight = EditorGUILayout.IntField("Terrain Height", tHeight);
        tScale = EditorGUILayout.FloatField("Scale", tScale);

        //EditorGUI.ObjectField(new Rect(10, 10, 50, 50), tMterial, System.Type.GetType("GameObject"), true);

        EditorGUILayout.Space();

        tUseOffset = EditorGUILayout.Toggle("Use Offset", tUseOffset);

        //EditorGUILayout.ToggleGroupScope toggle = new EditorGUILayout.ToggleGroupScope("test", tt); //("", tUseOffset);

        EditorGUILayout.BeginToggleGroup("Use Offset", tUseOffset);

        tOffsetX = EditorGUILayout.FloatField("Terrain Offset X", tOffsetX);
        tOffsetY = EditorGUILayout.FloatField("Terrain Offset Y", tOffsetY);

        EditorGUILayout.EndToggleGroup();


        //GUILayout.Label("Terrain data path:");
        //path = EditorGUILayout.TextField("Assets/", path);

        EditorGUILayout.Space();

        if (GUILayout.Button("Tool me"))
        {
            ValidatePath();
            CreateUpdateTerrain();
            path = string.Empty;
        }

    }

    private void Update()
    {
        if (autoUpdate)
        {
            ValidatePath();
            CreateUpdateTerrain();
            path = string.Empty;
        }
    }

    private void ValidatePath()
    {
        //if (path == string.Empty) path = "TerrainData/";

        string pathToCheck = Application.dataPath + "/" + path;
        if (Directory.Exists(pathToCheck) == false)
        {
            Directory.CreateDirectory(pathToCheck);
        }
    }

    private void CreateUpdateTerrain()
    {
        string terrainName = string.IsNullOrEmpty(tName) ? "Terrain" : tName;

        //if (!string.IsNullOrEmpty(path))
        //{            
        //    if (path[path.Length - 1] != '/')
        //    {
        //        path = path + '/';
        //    }
        //}



        TerrainData td = new TerrainData();
        td.heightmapResolution = tWidth + 1;
        td.size = new Vector3(tWidth, tHeight, tLength);
        

        if (tUseOffset)
        {
            td.SetHeights(0, 0, TerrainGenerator.GenerteTestNoiseMap(td.size));
        }
        else
        {
            td.SetHeights(0, 0, TerrainGenerator.GenerteNoiseMap(td.size));
        }

        td.name = terrainName;

        td.baseMapResolution = tWidth + 1;
        td.alphamapResolution = tWidth + 1;
        td.SetDetailResolution(0, 32);

        var exists = GameObject.FindGameObjectsWithTag(TerrainToolTag).Where(g => g.name == terrainName).FirstOrDefault();

        if (exists == null)
        {

            GameObject _terrain = Terrain.CreateTerrainGameObject(td);
            _terrain.name = terrainName;
            _terrain.tag = TerrainToolTag;

            AssetDatabase.CreateAsset(td, "Assets/TerrainData/" + terrainName + ".asset");

        }
        else
        {
            Terrain _t = exists.GetComponent<Terrain>();
            _t.terrainData = td;
        }
    }

    



}
