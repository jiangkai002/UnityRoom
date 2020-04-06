using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class GetRoomModel : EditorWindow
{
    List<string> jsonPaths = new List<string>();
    List<RoomBoundary> roomBoundaries = new List<RoomBoundary>();
    Dictionary<string, string> jsonContext = new Dictionary<string, string>();
    string jsonContent = "";
    GUIStyle style1Button;
    GetRoomModel()
    {
        this.titleContent = new GUIContent("By  宋   V1.1");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("工具/生成房间模型")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GetRoomModel));
        Rect re = new Rect(0f, 0f, 900f, 600f);
        EditorWindow.GetWindowWithRect(typeof(GetRoomModel), re);

    }

    void OnGUI()
    {
        style1Button = EditorStyles.miniButton;
        style1Button.fontSize = 14;
        /////////////////// 第一块、选择路径得到json
        List<string> lastJsonPath = jsonPaths;
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("单个选择", style1Button))
        {
            jsonPaths.Clear();
            List<string> filters = new List<string> { "JSON", "json" };
            jsonPaths.Add(EditorUtility.OpenFilePanelWithFilters("Open File Dialog", Application.dataPath, filters.ToArray()));
            roomBoundaries = GetJsonContent(jsonPaths[0]);
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("批量选择", style1Button))
        {
            jsonPaths.Clear();
            string folderPath = EditorUtility.OpenFolderPanel("Open Folder Dialog", Application.dataPath, "房间边界");
            foreach (var aa in new DirectoryInfo(folderPath).GetFiles())
            {
                if (aa.Extension == ".json")
                    jsonPaths.Add(aa.FullName);
            }
            jsonContext = GetJsonContent(jsonPaths);
        }
        EditorGUILayout.EndHorizontal();

        //提示出所选内容的正确错误
        for (int i = 0; i < jsonPaths.Count; i++)
        {
            if (i % 2 == 0)
            {
                EditorGUILayout.BeginHorizontal();
                string flag = "";
                if (jsonContext.ContainsKey(jsonPaths[i])) flag = "[√]";
                else flag = "[X]";
                EditorGUILayout.LabelField(flag + new FileInfo(jsonPaths[i]).Name, EditorStyles.boldLabel);
                if (i < jsonPaths.Count - 1)
                {
                    if (jsonContext.ContainsKey(jsonPaths[i + 1])) flag = "[√]";
                    else flag = "[X]";
                    EditorGUILayout.LabelField(flag + new FileInfo(jsonPaths[i + 1]).Name, EditorStyles.boldLabel);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        //////
        if (GUILayout.Button("在场景中生成房间面片", style1Button))
        {
            //Meshs
            GenerateRoomMeshs(jsonContext.Values.ToList());
        }
        if (GUILayout.Button("在场景中生成房间体块", style1Button))
        {
            //Meshs
            List<RoomBoundary> roomBoundaries = GetJsonContent(jsonPaths.FirstOrDefault());

            Draw.CreateRoom(roomBoundaries);
            GenerateRoomModels(jsonContext.Values.ToList());
        }
    }

    Dictionary<string, string> GetJsonContent(List<string> jsonPaths)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        foreach (var filePath in jsonPaths)
        {
            using (StreamReader reader = File.OpenText(filePath))
            {
                string str = reader.ReadToEnd();
                reader.Close();
                if (IsRoomJson(str))
                {
                    result.Add(filePath, str);
                }
            }
        }
        return result;
    }

     List<RoomBoundary> GetJsonContent(string jsonPath)
    {
        List<RoomBoundary> roomBoundary = new List<RoomBoundary>();
        using (StreamReader reader = File.OpenText(jsonPath))
        {
            string str = reader.ReadToEnd();
            reader.Close();
            if (IsRoomJson(str))
            {
                return JsonConvert.DeserializeObject<List<RoomBoundary>>(str);
            }
        }
        return null;
    }

    /// <summary>
    /// 判断是不是一个合格的房间边界Json
    /// </summary>
    bool IsRoomJson(string context)
    {
        if (context.Length > 3)
            return true;
        return false;
    }

    //根据生成Mesh
    List<RoomModel> GenerateRoomMeshs(List<string> jsons)
    {
        return null;
    }

    //根据生成Model
    List<RoomModel> GenerateRoomModels(List<string> jsons)
    {
        return null;
    }
    List<RoomModel> GenerateRoomModels(List<RoomBoundary> roomBoundary)
    {
        GenerateRooms.CreateRoom(roomBoundary);
        return null;
    }
}

