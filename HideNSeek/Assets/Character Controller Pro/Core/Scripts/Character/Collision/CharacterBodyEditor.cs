using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Lightbug.CharacterControllerPro.Core
{

    [CustomEditor(typeof(CharacterBody), true), CanEditMultipleObjects]
    public class CharacterBodyEditor : Editor
    {
        const float ButtonHeight = 20f;
        SerializedProperty bodySize = null;
        SerializedProperty mass = null;

        CharacterBody monoBehaviour;

        void OnEnable()
        {
            monoBehaviour = (CharacterBody)target;

            bodySize = serializedObject.FindProperty("bodySize");
            mass = serializedObject.FindProperty("mass");
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CustomUtilities.DrawMonoBehaviourField<CharacterBody>((CharacterBody)target);

            if (monoBehaviour.transform.localScale != Vector3.one)
            {
                GUI.color = Color.red;
                GUILayout.BeginVertical("Box");
            }

            CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray);
            DrawSize();

            CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray);
            EditorGUILayout.PropertyField(mass);

            GUILayout.Space(10f);

            if (monoBehaviour.transform.localScale != Vector3.one)
                EditorGUILayout.HelpBox("Transform local scale is not <1,1,1>!", MessageType.Warning);

            if (monoBehaviour.transform.localScale != Vector3.one)
            {
                GUILayout.EndVertical();
            }

            GUI.color = Color.white;

            serializedObject.ApplyModifiedProperties();
        }

        void DrawSize()
        {
            float width = EditorGUILayout.FloatField("Width", bodySize.vector2Value.x);

            if (width < 0f)
                width = 0f;

            float height = EditorGUILayout.FloatField("Height", bodySize.vector2Value.y);

            if (height < width)
                height = width;

            bodySize.vector2Value = new Vector2(width, height);

        }

        CapsuleBoundsHandle capsuleHandle = new CapsuleBoundsHandle();

        void OnSceneGUI()
        {
            if (monoBehaviour == null)
                return;

            Handles.matrix = monoBehaviour.transform.localToWorldMatrix;

            capsuleHandle.radius = bodySize.vector2Value.x / 2f;
            capsuleHandle.height = bodySize.vector2Value.y;

            capsuleHandle.center = Vector3.up * capsuleHandle.height / 2f;

            capsuleHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y | PrimitiveBoundsHandle.Axes.Z;


            capsuleHandle.heightAxis = CapsuleBoundsHandle.HeightAxis.Y;
            capsuleHandle.DrawHandle();

            Handles.matrix = Matrix4x4.identity;

        }

        // protected Bounds OnHandleChanged( PrimitiveBoundsHandle.HandleDirection handle, Bounds boundsOnClick, Bounds newBounds )
        // {

        // }

    }

}

#endif
