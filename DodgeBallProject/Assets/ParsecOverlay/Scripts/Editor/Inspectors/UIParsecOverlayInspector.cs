using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ParsecOverlay
{

    [CustomEditor(typeof(UIParsecOverlay))]
    public class UIParsecOverlayInspector : Editor
    {
        public string[] EXCLUDED_PROPERTIES = new string[]
        {
            "m_Script",
            "rewiredPlayers"
        };

        private ReorderableList _rewiredPlayersReorderableList;

        private void OnEnable()
        {
            _rewiredPlayersReorderableList =
                new ReorderableList(serializedObject, serializedObject.FindProperty("rewiredPlayers"));

            _rewiredPlayersReorderableList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) => {
                    SerializedProperty property =
                        _rewiredPlayersReorderableList.serializedProperty.GetArrayElementAtIndex(index);
                    property.stringValue = EditorGUI.TextField(rect, property.stringValue);
                };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, EXCLUDED_PROPERTIES);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Rewired Mapping", EditorStyles.boldLabel);
            _rewiredPlayersReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

    }

}
