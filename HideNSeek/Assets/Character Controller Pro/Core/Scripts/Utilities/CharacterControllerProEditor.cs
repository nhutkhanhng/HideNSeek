#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;

namespace Lightbug.CharacterControllerPro.Core
{

class CCPAssetPostprocessor : AssetPostprocessor
{
    public const string RootFolder = "Assets/Character Controller Pro";

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths )
    {
        
        foreach( string importedAsset in importedAssets )
        {            
            if( importedAsset.Equals( RootFolder ) )
            {
                WelcomeWindow window = EditorWindow.GetWindow<WelcomeWindow>( true , "Welcome" );
            }
        }
    }
}

public class CharacterControllerProEditor : Editor
{
    [MenuItem( "Character Controller Pro/Welcome" )]
    public static void WelcomeMessage()
    {
        WelcomeWindow window = EditorWindow.GetWindow<WelcomeWindow>( true , "Welcome" ); 
    }

    [MenuItem( "Character Controller Pro/Documentation" )]
    public static void Documentation()
    {
        Application.OpenURL("https://lightbug14.gitbook.io/ccp/" );
    }

    [MenuItem( "Character Controller Pro/API Reference" )]
    public static void APIReference()
    {
        Application.OpenURL( "https://lightbug14.github.io/lightbug-web/character-controller-pro/Documentation/html/index.html" );
    }

    [MenuItem( "Character Controller Pro/About" )]
    public static void About()
    {
        AboutWindow window = EditorWindow.GetWindow<AboutWindow>( true , "About" );
    }
    
}

public abstract class CharacterControllerProWindow : EditorWindow
{
    protected GUIStyle subtitleStyle = new GUIStyle();
    protected GUIStyle descriptionStyle = new GUIStyle();
    
    protected Texture bannerTexture = null;

    protected virtual void OnEnable()
    {        
        subtitleStyle.fontSize = 18;
        subtitleStyle.alignment = TextAnchor.MiddleCenter;
        subtitleStyle.padding.top = 4;
        subtitleStyle.padding.bottom = 4;
        subtitleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        descriptionStyle.fontSize = 15;
        descriptionStyle.wordWrap = true;
        descriptionStyle.padding.left = 10;
        descriptionStyle.padding.right = 10;
        descriptionStyle.padding.top = 4;
        descriptionStyle.padding.bottom = 4;
        descriptionStyle.richText = true;
        descriptionStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        bannerTexture = Resources.Load<Texture>( "Banner" );

    }
}

public class AboutWindow : CharacterControllerProWindow
{
    const float Width = 200f;
    const float Height = 100f;

    protected override void OnEnable()
    {
        this.position = new Rect( (Screen.width - Width ) / 2f , (Screen.height - Height ) / 2f , Width , Height );
        this.maxSize = this.minSize = this.position.size;
        this.titleContent = new GUIContent("About");
    }

    void OnGUI()
    {
        EditorGUILayout.SelectableLabel( "Version: 1.2.2" , GUILayout.Height(15f) );
        EditorGUILayout.SelectableLabel( "Author : Juan Sálice (Lightbug)" , GUILayout.Height(15f) );
        EditorGUILayout.SelectableLabel( "Email : lightbug14@gmail.com" , GUILayout.Height(15f) );
    }
}


public class WelcomeWindow : CharacterControllerProWindow
{

    protected override void OnEnable()
    {
        base.OnEnable();
    
        this.position = new Rect( 10f , 10f , 566f , 850f );
        this.maxSize = this.minSize = this.position.size;

    }

    void OnGUI()
    {
        GUILayout.Box( bannerTexture , GUILayout.Width(560f));

        GUILayout.Space(20f);

        GUILayout.Label(
        "Hi, welcome to <b>Character Controller Pro</b>."
        , descriptionStyle );

        GUILayout.Space(20f);

        GUILayout.BeginVertical( EditorStyles.helpBox );

        GUILayout.Label("<b>Important</b>" , subtitleStyle );        

        
        GUILayout.Label(
        "The demo scenes included in this package require you to modify some settings in your project (inputs and tags). " + 
        "<b>This is required only for demo purposes, the asset by itself (core + implementation) does not require any previous setup in order to work properly.</b>" , descriptionStyle );
        GUILayout.Label(
        "Inputs: By default the asset uses Unity's input manager, this is why some specific axis must be defined. If this is ignored a message should appear on console." , descriptionStyle );
        GUILayout.Label(
        "Tags: Some specifics tags have been defined, all of them are related to the \"Demo\" (materials and abilities)." , descriptionStyle );

        GUILayout.Label("<b>Demo Setup</b>" , subtitleStyle );

        GUILayout.Label(
        "1. Open the <b>Input manager settings</b>.\n" + 
        "2. Load <b>Preset_Inputs.preset</b>.\n" + 
        "3. Open the <b>Tags and Layers settings</b>.\n" +
        "4. Load <b>Preset_TagsAndLayers</b>.\n" , descriptionStyle );

        
        GUILayout.Label( "For more information about the setup, please visit the section \"Setting up the project\": "
        , descriptionStyle );

        if( GUILayout.Button( "Setting up the project" , EditorStyles.miniButton ) )
        {
            Application.OpenURL( "https://lightbug14.gitbook.io/ccp/package/setting-up-the-project" );
        }
        GUILayout.EndVertical();

        GUILayout.Label("You can open this window by using the top menu: \n<i>Character Controller Pro/Welcome</i>" , descriptionStyle );
        
    }

}

}

#endif
