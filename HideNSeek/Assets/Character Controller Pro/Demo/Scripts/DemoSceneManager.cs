using UnityEngine;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

public class DemoSceneManager : MonoBehaviour
{
    [Header("Character")]

    [SerializeField]
    CharacterActor characterActor = null;
    

    [Header("Scene references")]

    [SerializeField]
    CharacterReferenceObject[] references = null;

    [Header("UI")]

    [SerializeField]
    Canvas infoCanvas = null;

    [SerializeField]
    bool hideAndConfineCursor = true;

    [Header("Graphics")]


    [SerializeField]
    GameObject graphicsObject = null;

    [Header("Camera")]

    [SerializeField]
    new Camera3D camera = null; 
    
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    Renderer[] graphicsRenderers = null;
    Renderer[] capsuleRenderers = null;

    NormalMovement normalMovement = null;

    void Awake()
    {
        if( characterActor != null )
            normalMovement = characterActor.GetComponentInChildren<NormalMovement>();

        if( normalMovement != null && camera != null )
            normalMovement.FollowExternalReference = camera.cameraMode == Camera3D.CameraMode.FirstPerson;

        if( graphicsObject != null )
            graphicsRenderers = graphicsObject.GetComponentsInChildren<Renderer>( true );

        Cursor.visible = !hideAndConfineCursor;
        Cursor.lockState = hideAndConfineCursor ? CursorLockMode.Locked : CursorLockMode.None;
        
        
    }

    void Update()
    {
        int index = 0;

        for( index = 0 ; index < references.Length ; index++ )
        {        
            if( references[index] == null )
                break;
            
            if( Input.GetKeyDown( KeyCode.Alpha1 + index ) || Input.GetKeyDown( KeyCode.Keypad1 + index ) )
            {
                GoTo( references[index] );
                break;
            }
        }
        
        if( Input.GetKeyDown( KeyCode.Tab ) )
        {
            if( infoCanvas != null )
                infoCanvas.enabled = !infoCanvas.enabled;
        }


        if( Input.GetKeyDown( KeyCode.V ) )
        {
            // If the Camera3D is present, change between First person and Third person mode.
            if( camera != null )
            {
                camera.ToggleCameraMode();

                if( normalMovement != null )
                    normalMovement.FollowExternalReference = camera.cameraMode == Camera3D.CameraMode.FirstPerson;
                
            }

            
        }
        
    }

    

    void HandleVisualObjects( bool showCapsule )
    {
        if( capsuleRenderers != null )
            for( int i = 0 ; i < capsuleRenderers.Length ; i++ )
                capsuleRenderers[i].enabled = showCapsule;
        
        if( graphicsRenderers != null )
            for( int i = 0 ; i < graphicsRenderers.Length ; i++ )
            {
                SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)graphicsRenderers[i];
                if( skinnedMeshRenderer != null )
                    skinnedMeshRenderer.forceRenderingOff = showCapsule;
                else
                    graphicsRenderers[i].enabled = !showCapsule;
            }
        
        
    }

    void GoTo( CharacterReferenceObject reference )
    {
        if( reference == null )
            return;        
        
        if( characterActor == null )
            return;
        
        characterActor.constraintUpDirection = reference.referenceTransform.up;
        characterActor.Teleport( reference.referenceTransform );

        characterActor.upDirectionReference = reference.verticalAlignmentReference;
        characterActor.upDirectionReferenceMode = VerticalAlignmentSettings.VerticalReferenceMode.Away;
        
    }
}

}
