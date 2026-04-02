using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
public class TextButtons : MonoBehaviour , IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler,IPointerDownHandler
{
    public bool useTextHover;

    public Color textColorEnter;
    public Color textColorPressed;
    TextMeshProUGUI text;
    SoundButtons audioButtons;
    private void Awake()
    {
        if (useTextHover)
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
            text.material = new Material(text.material);
        }
        audioButtons = FindObjectOfType<SoundButtons>();
    }
    void OnEnable()
    {
        if(useTextHover)
            text.color = Color.white;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(useTextHover)
            text.color = textColorEnter;
        audioButtons.onEnterSound();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(useTextHover)
            text.color = textColorEnter;
        audioButtons.onClickSound();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(useTextHover)
            text.color = textColorPressed;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(useTextHover)
            text.color = Color.white;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(TextButtons))]
public class TextButtonsEditor : Editor
{
    SerializedProperty m_useTextHover;
    SerializedProperty m_textColorEnter;
    SerializedProperty m_textColorPressed;
    void OnEnable()
    {
        m_useTextHover = serializedObject.FindProperty("useTextHover");
        m_textColorEnter = serializedObject.FindProperty("textColorEnter");
        m_textColorPressed = serializedObject.FindProperty("textColorPressed");
    }
    public override void OnInspectorGUI()
    {
        TextButtons textButtons = (TextButtons)target;
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_useTextHover,new GUIContent("Use Text Hover"));
        if (textButtons.useTextHover)
        {
            EditorGUILayout.PropertyField(m_textColorEnter,new GUIContent("Text Color Enter"));
            EditorGUILayout.PropertyField(m_textColorPressed,new GUIContent("Text Color Pressed"));
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif