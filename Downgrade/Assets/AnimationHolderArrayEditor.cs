/*using UnityEditorInternal;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(AnimationHolder))]
public class AnimationHolderArrayEditor : Editor
{
    private AnimationHolder holder;

    public override void OnInspectorGUI()
    {
        holder = (AnimationHolder)target;

        /*if (GUILayout.Button("Open Animation Editor"))
        {
            AnimationEditorWindow.ShowWindow(holder);
        }

        DrawDefaultInspector();
    }
}

public class AnimationEditorWindow : EditorWindow
{
    private AnimationHolder holder;
    private SerializedObject serializedHolder;
    private ReorderableList clipsList;
    private Vector2 scrollPos;
    private int tab;

    public static void ShowWindow(AnimationHolder holder)
    {
        AnimationEditorWindow window = GetWindow<AnimationEditorWindow>("Animation Editor");
        window.holder = holder;
        window.serializedHolder = new SerializedObject(holder);
        window.InitializeClipsList();
    }

    private void OnGUI()
    {
        if (holder == null)
        {
            EditorGUILayout.LabelField("No AnimationHolder selected.");
            return;
        }

        serializedHolder.Update();

        GUILayout.BeginHorizontal();
        if (GUILayout.Toggle(tab == 0, "Animation Clips", EditorStyles.toolbarButton)) tab = 0;
        if (GUILayout.Toggle(tab == 1, "Animation Events", EditorStyles.toolbarButton)) tab = 1;
        GUILayout.EndHorizontal();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (tab == 0)
        {
            clipsList.DoLayoutList();
        }
        else if (tab == 1)
        {
            DrawAnimationEventsTab();
        }

        EditorGUILayout.EndScrollView();

        serializedHolder.ApplyModifiedProperties();
    }

    private void DrawAnimationClipsTab()
    {
        EditorGUILayout.LabelField("Animation Clips", EditorStyles.boldLabel);

        SerializedProperty clips = serializedHolder.FindProperty("clips");
        if (clips != null)
        {
            for (int i = 0; i < clips.arraySize; i++)
            {
                SerializedProperty clip = clips.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(clip, GUIContent.none);
                if (GUILayout.Button("X", GUILayout.Width(20))) // Botón para eliminar el clip
                {
                    clips.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No clips found", MessageType.Warning);
        }
    }


    private void DrawAnimationEventsTab()
    {
        EditorGUILayout.LabelField("Animation Custom Events", EditorStyles.boldLabel);

        SerializedProperty animations = serializedHolder.FindProperty("animations");
        if (animations != null)
        {
            for (int i = 0; i < animations.arraySize; i++)
            {
                SerializedProperty animation = animations.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(animation.FindPropertyRelative("animClip"), new GUIContent("Animation Clip"));

                SerializedProperty events = animation.FindPropertyRelative("invokableEvents");
                for (int j = 0; j < events.arraySize; j++)
                {
                    SerializedProperty eventData = events.GetArrayElementAtIndex(j);
                    EditorGUILayout.PropertyField(eventData.FindPropertyRelative("time"), new GUIContent($"Event {j + 1} time of Activation"));
                    EditorGUILayout.PropertyField(eventData.FindPropertyRelative("events"), new GUIContent("Events"));
                }

                if (GUILayout.Button("Add Event"))
                {
                    events.InsertArrayElementAtIndex(events.arraySize);
                }

                if (events.arraySize > 0 && GUILayout.Button("Remove Last Event"))
                {
                    events.DeleteArrayElementAtIndex(events.arraySize - 1);
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No animations found", MessageType.Warning);
        }
    }

    private void InitializeClipsList()
    {
        clipsList = new ReorderableList(serializedHolder, serializedHolder.FindProperty("clips"), true, true, true, true);
        clipsList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Animation Clips");
        };
        clipsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = clipsList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };
        clipsList.onChangedCallback = (ReorderableList list) =>
        {
            serializedHolder.ApplyModifiedProperties();
        };
    }
}*/
