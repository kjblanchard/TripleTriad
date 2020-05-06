/* 
/// Copyright (c) 2015 Sirawat Pitaksarit, Exceed7 Experiments LP 
/// http://www.exceed7.com/introloop
*/

using UnityEditor;
using UnityEngine;

namespace E7.Introloop
{
    [CustomPropertyDrawer(typeof(IntroloopSettings))]
    public class IntroloopSettingsDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + 2) * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect line = position;
            line.height = EditorGUIUtility.singleLineHeight;
            // line.width -= 8;
            // line.y += 4;
            // line.x += 4;

            // GUI.Box(position, "", EditorStyles.helpBox);
            var mixer = property.FindPropertyRelative("routeToMixerGroup");
            var log = property.FindPropertyRelative("logInformation");

            EditorGUI.PropertyField(line, mixer);
            line.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(line, log);
        }
    }
}