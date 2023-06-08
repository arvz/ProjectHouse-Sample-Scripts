using System;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class DataEditorWindow : OdinMenuEditorWindow
{
    const string EQUIPMENTS_DATA_PATH = "Resources/GameData/Equipments/";
    const string ITEMS_DATA_PATH = "Resources/GameData/Items/";

    const string EQUIPMENTS = "Equipments";
    const string ITEMS = "Items";

    OdinMenuTree _tree;
    OdinMenuItem _equipmentsMenuItem;
    OdinMenuItem _itemsMenuItem;

    [MenuItem("Tools/Game Data Editor")]
    public static void OpenWindow()
    {
        GetWindow<DataEditorWindow>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        _tree = new OdinMenuTree();
        Undo.undoRedoPerformed += OnUndoRedoPerformed;

        _equipmentsMenuItem = new OdinMenuItem(_tree, EQUIPMENTS, null);
        _itemsMenuItem = new OdinMenuItem(_tree, ITEMS, null);

        _tree.Selection.SupportsMultiSelect = false;

        _tree.AddAllAssetsAtPath(EQUIPMENTS, EQUIPMENTS_DATA_PATH, typeof(EquipmentItemData));
        _tree.AddMenuItemAtPath("", _equipmentsMenuItem);

        _tree.AddAllAssetsAtPath(ITEMS, ITEMS_DATA_PATH, typeof(ItemData));
        _tree.AddMenuItemAtPath("", _itemsMenuItem);

        return _tree;
    }

    protected override void OnBeginDrawEditors()
    {
        if (MenuTree == null) return;

        OdinMenuTreeSelection selected = MenuTree.Selection;
        DrawTopToolbar(selected);
    }

    protected override void DrawMenu()
    {
        base.DrawMenu();
        GUILayout.FlexibleSpace();

        SirenixEditorGUI.BeginHorizontalToolbar();

        if (SirenixEditorGUI.ToolbarButton("Rebuild"))
        {
            ForceMenuTreeRebuild();
        }

        SirenixEditorGUI.EndHorizontalToolbar();
    }

    protected override void OnEndDrawEditors()
    {
        GUILayout.FlexibleSpace();
        SirenixEditorGUI.BeginHorizontalToolbar(35, 5);
        GUI.backgroundColor = Color.red;
        SirenixEditorGUI.EndHorizontalToolbar();
    }

    protected override void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnGUI();

        if (EditorGUI.EndChangeCheck())
        {
        }
    }

    void DrawTopToolbar(OdinMenuTreeSelection selected)
    {
        SirenixEditorGUI.BeginHorizontalToolbar();

        GUILayout.FlexibleSpace();

        if (selected.SelectedValue != null)
        {
            var asset = (UnityEngine.Object)selected.SelectedValue;

            if (SirenixEditorGUI.ToolbarButton("Select Data"))
            {
                EditorGUIUtility.PingObject(asset);
                Selection.SetActiveObjectWithContext(asset, asset);
            }

            if (SirenixEditorGUI.ToolbarButton("Delete Data"))
            {
                var path = AssetDatabase.GetAssetOrScenePath(asset);
                AssetDatabase.DeleteAsset(path);
            }
        }


        SirenixEditorGUI.EndHorizontalToolbar();
    }

    void OnUndoRedoPerformed()
    {
    }
}