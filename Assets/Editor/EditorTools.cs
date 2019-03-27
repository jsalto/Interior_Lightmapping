using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class EditorArmariosTools : EditorWindow
{
    [MenuItem("Edit/Deselect All &d", false, -101)]
    static void Deselect()
    {
        Selection.activeGameObject = null;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [MenuItem("Edit/Run _F5")] // shortcut key F5 to Play (and exit playmode also)
    static void PlayGame()
    {
        //if(!Application.isPlaying) {
        //    EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false);
        //}
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
}