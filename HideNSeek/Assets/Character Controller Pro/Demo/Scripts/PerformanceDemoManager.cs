﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lightbug.CharacterControllerPro.Demo
{

public class PerformanceDemoManager : MonoBehaviour
{
    [SerializeField]
    GameObject characterPrefab = null;

    [SerializeField]
    Transform prefabInstantiationReference = null;

    [SerializeField]
    Text textField = null;

    [SerializeField]
    float maxInstantiationDistance = 50f;

    int numberOfCharacters = 0;

    List<GameObject> characterObjects = new List<GameObject>();

    void Awake()
    {
        if( characterPrefab == null )
        {
            Debug.Log("Missing prefab! Destroying this component...");
            Destroy( this );
        }
    }
    
    public void AddCharacters( int charactersToAdd )
    {
        if( characterPrefab == null )
            return;
        
        for( int i = 0 ; i < charactersToAdd ; i++ )
        {
            GameObject newCharacter = Instantiate<GameObject>( 
                characterPrefab , 
                prefabInstantiationReference.position + Vector3.right * Random.Range( - maxInstantiationDistance , maxInstantiationDistance ) + Vector3.forward * Random.Range( - maxInstantiationDistance , maxInstantiationDistance ) , 
                Quaternion.identity * Quaternion.Euler( 0 , Random.Range( 0f , 180f ) , 0f ) );
            characterObjects.Add( newCharacter );
        }

        this.numberOfCharacters += charactersToAdd;

        if( textField != null )
            textField.text = this.numberOfCharacters.ToString();
    }

    
    public void RemoveCharacters( int charactersToEliminate )
    {
        if( this.numberOfCharacters < charactersToEliminate )
        {
            RemoveAllCharacters();
            return;
        }
        
        for( int i = charactersToEliminate - 1 ; i >= 0 ; i-- )
        {
            Destroy( characterObjects[i] );            
            characterObjects.RemoveAt( i );
        }

        this.numberOfCharacters -= charactersToEliminate;

        if( textField != null )
            textField.text = this.numberOfCharacters.ToString();
    }

    public void RemoveAllCharacters()
    {
        RemoveCharacters( this.numberOfCharacters );        
    }
}

}