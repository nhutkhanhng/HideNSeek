using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.Utilities
{

[System.AttributeUsage(System.AttributeTargets.Field)]
public class ConditionAttribute : PropertyAttribute
{
    public enum ConditionType
    {
        IsTrue ,
        IsFalse ,
        IsGreaterThan ,
        IsEqualTo ,
        IsLessThan ,
        HasReference
    }

    public enum VisibilityType
    {
        Hidden ,
        NotEditable
    }

    public string conditionPropertyName;
    public ConditionType conditionType;
    public VisibilityType visibilityType;
    public float value;

    /// <summary>
    /// This attribute will determine the visibility of the target property based on some other property condition. Use this attribute if the target property 
    /// depends on some other property inside the class.
    /// </summary>
    /// <param name="conditionPropertyName">Name of the property used by the condition.</param>
    /// <param name="conditionType">The condition type.</param>
    /// <param name="visibilityType">The visibility action to perform if the condition is not met.</param>
    /// <param name="conditionValue">The condition argument value.</param>
    public ConditionAttribute( string conditionPropertyName , ConditionType conditionType , VisibilityType visibilityType = VisibilityType.Hidden , float conditionValue = 0f )
    {
        this.conditionPropertyName = conditionPropertyName;
        this.conditionType = conditionType;
        this.visibilityType = visibilityType;
        this.value = conditionValue;

    }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ConditionAttribute))]
public class ConditionAttributeEditor : PropertyDrawer
{
	ConditionAttribute target;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{		
		if( target == null )
			target = attribute as ConditionAttribute;        

        bool result = CheckCondition( property );
        if( target.visibilityType == ConditionAttribute.VisibilityType.NotEditable )
        {
            GUI.enabled = result;
            EditorGUI.PropertyField( position , property , true );
            GUI.enabled = true;
        }
        else
        {
            if( result )
                EditorGUI.PropertyField( position , property , true );
        }
        

	}

    bool result = false;

    bool CheckCondition( SerializedProperty property )
    {
        SerializedProperty conditionProperty = property.serializedObject.FindProperty( target.conditionPropertyName );

        // If the property is null make the target property visible.
        if( conditionProperty == null )
            return true;
        
        result = false;

        SerializedPropertyType conditionPropertyType = conditionProperty.propertyType;

        if( conditionPropertyType == SerializedPropertyType.Boolean )
        {
            if( target.conditionType == ConditionAttribute.ConditionType.IsTrue )
                result = conditionProperty.boolValue;
            else if( target.conditionType == ConditionAttribute.ConditionType.IsFalse )
                result = !conditionProperty.boolValue;
            
        }
        else if( conditionPropertyType == SerializedPropertyType.Float )
        {
                
                float conditionPropertyFloatValue = conditionProperty.floatValue;
                float argumentFloatValue = target.value;

                switch( target.conditionType )
                {
                    case ConditionAttribute.ConditionType.IsTrue:
                        result = conditionPropertyFloatValue != 0f;
                        break;
                    case ConditionAttribute.ConditionType.IsFalse:
                        result = conditionPropertyFloatValue == 0f;
                        break;
                    case ConditionAttribute.ConditionType.IsGreaterThan:
                        result = conditionPropertyFloatValue > argumentFloatValue;
                        break;
                    case ConditionAttribute.ConditionType.IsEqualTo:
                        result = conditionPropertyFloatValue == argumentFloatValue;
                        break;
                    case ConditionAttribute.ConditionType.IsLessThan:
                        result = conditionPropertyFloatValue < argumentFloatValue;
                        break;
                }
                
        }
        else if( conditionPropertyType == SerializedPropertyType.Integer || conditionPropertyType == SerializedPropertyType.Enum )
        {
            int conditionPropertyIntValue = conditionProperty.intValue;
            int argumentIntValue = (int)target.value;

            switch( target.conditionType )
            {
                case ConditionAttribute.ConditionType.IsTrue:
                    result = conditionPropertyIntValue != 0;
                    break;
                case ConditionAttribute.ConditionType.IsFalse:
                    result = conditionPropertyIntValue == 0;
                    break;
                case ConditionAttribute.ConditionType.IsGreaterThan:
                    result = conditionPropertyIntValue > argumentIntValue;
                    break;
                case ConditionAttribute.ConditionType.IsEqualTo:
                    result = conditionPropertyIntValue == argumentIntValue;
                    break;
                case ConditionAttribute.ConditionType.IsLessThan:
                    result = conditionPropertyIntValue < argumentIntValue;
                    break;
            }
            
        }    
        else if( conditionPropertyType == SerializedPropertyType.ObjectReference )
        {
            UnityEngine.Object conditionPropertyObjectValue = conditionProperty.objectReferenceValue;
            result = conditionPropertyObjectValue != null;
            
        }    
        
        return result;
        
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if( target == null )
			target = attribute as ConditionAttribute;    
        
        return !result && target.visibilityType == ConditionAttribute.VisibilityType.Hidden ? 0f : EditorGUI.GetPropertyHeight( property );
    }

    
		
}

#endif

}

