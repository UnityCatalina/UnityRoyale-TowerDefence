using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomGUIShader : ShaderGUI
{
    MaterialEditor editor;
    MaterialProperty[] properties;
    int index = 0;
    bool has = false;
    public override void OnGUI(MaterialEditor editor, MaterialProperty[] properties)
    {
        this.properties = properties;
        this.editor = editor;
        DoGUI();
    }
    GUIContent MakeLabel(string name, string tooltip)
    {
        GUIContent uIContent = new GUIContent();
        uIContent.text = name;
        uIContent.tooltip = tooltip;
        return uIContent;
    }
    MaterialProperty FindProperty(string name)
    {
        return FindProperty(name, properties);
    }
    void DoGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Bleding Mode");
        GUILayout.BeginVertical();
#pragma warning disable CS0618 // Type or member is obsolete
        editor.SetFloat("_Index", EditorGUILayout.Popup((int)editor.GetFloat("_Index", out has), new string[] {"Tranditional transparency","Premultiplied transparency","Additive","Soft Additive", "Multiplicative","2x Multiplicative" }, EditorStyles.popup));
#pragma warning restore CS0618 // Type or member is obsolete


        switch ((int)editor.GetFloat("_Index",out has))
        {
            case 0:
                {
                    editor.SetFloat("_Src", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    editor.SetFloat("_Des", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                }
                break;
            case 1:
                {
                    editor.SetFloat("_Src", (int)UnityEngine.Rendering.BlendMode.One);
                    editor.SetFloat("_Des", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                }
                break;
            case 2:
                {
                    editor.SetFloat("_Src", (int)UnityEngine.Rendering.BlendMode.One);
                    editor.SetFloat("_Des", (int)UnityEngine.Rendering.BlendMode.One);
                }
                break;
            case 3:
                {
                    editor.SetFloat("_Src", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
                    editor.SetFloat("_Des", (int)UnityEngine.Rendering.BlendMode.One);
                }
                break;
            case 4:
                {
                    editor.SetFloat("_Src", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    editor.SetFloat("_Des", (int)UnityEngine.Rendering.BlendMode.Zero);
                }
                break;
            case 5:
                {
                    editor.SetFloat("_Src", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    editor.SetFloat("_Des", (int)UnityEngine.Rendering.BlendMode.SrcColor);
                }
                break;
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.Label("Main Tex", EditorStyles.boldLabel);



        // Main Tex 
        GUILayout.BeginVertical();

        editor.TexturePropertySingleLine(MakeLabel("Albedo", "jeje"), FindProperty("_MainTex"));
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        editor.TextureScaleOffsetProperty(FindProperty("_MainTex"));
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        GUILayout.BeginVertical();
        editor.ShaderProperty(FindProperty("_SpeedMainU"), "Speed U");
        editor.ShaderProperty(FindProperty("_SpeedMainV"), "Speed V");
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();

        editor.TexturePropertySingleLine(MakeLabel("Alpha Map", "jeje"), FindProperty("_AlphaMap1"));
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        editor.TextureScaleOffsetProperty(FindProperty("_AlphaMap1"));
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();



        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        GUILayout.BeginVertical();
        editor.ShaderProperty(FindProperty("_SpeedMainAlphaU"), "Speed U");
        editor.ShaderProperty(FindProperty("_SpeedMainAlphaV"), "Speed V");
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        // Sub Tex

        GUILayout.BeginVertical();
        GUILayout.Space(20);
        GUILayout.Label("Sub Tex", EditorStyles.boldLabel);

        editor.TexturePropertySingleLine(MakeLabel("Albedo", "jeje"), FindProperty("_SubTex"));
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        editor.TextureScaleOffsetProperty(FindProperty("_SubTex"));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        GUILayout.BeginVertical();
        editor.ShaderProperty(FindProperty("_SpeedSubU"), "Speed U");
        editor.ShaderProperty(FindProperty("_SpeedSubV"), "Speed V");
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();



        GUILayout.BeginVertical();
        GUILayout.Space(0);
        editor.TexturePropertySingleLine(MakeLabel("Alpha Map", "jeje"), FindProperty("_AlphaMap2"));
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        editor.TextureScaleOffsetProperty(FindProperty("_AlphaMap2"));
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        GUILayout.BeginVertical();
        editor.ShaderProperty(FindProperty("_SpeedSubAlphaU"), "Speed U");
        editor.ShaderProperty(FindProperty("_SpeedSubAlphaV"), "Speed V");
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        // Blend Chanel
        GUILayout.BeginVertical();
        GUILayout.Space(20);
        GUILayout.Label("Blend Tex", EditorStyles.boldLabel);
        GUILayout.BeginVertical();
      
        editor.TexturePropertySingleLine(MakeLabel("Blend Map", "hihi"), FindProperty("_BlendMap"));
        editor.TextureScaleOffsetProperty(FindProperty("_BlendMap"));
    

        GUILayout.BeginVertical();
        editor.ShaderProperty(FindProperty("_SpeedBlendU"), "Speed U");
        editor.ShaderProperty(FindProperty("_SpeedBlendV"), "Speed V");
        GUILayout.EndVertical();



        editor.DefaultShaderProperty(FindProperty("_Blend"), "Blend");

    }
}
