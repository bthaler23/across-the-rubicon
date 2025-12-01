using UnityEditor;
using UnityEngine;

namespace GamePlugins.Attributes
{
	// The property drawer class should be placed in an editor script, inside a folder called Editor.

	// Tell the RangeDrawer that it is a drawer for properties with the RangeAttribute.
	[CustomPropertyDrawer(typeof(RequiredBoolAttribute))]
	public class RequiredBoolAttributeDrawer : PropertyDrawer
	{

		// Draw the property inside the given rect
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{

			// First get the attribute since it contains the range for the slider
			RequiredBoolAttribute requiredBool = attribute as RequiredBoolAttribute;
			EditorGUIUtility.labelWidth = 220;
			// Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
			if (property.propertyType == SerializedPropertyType.Boolean)
			{
				Color previousGUIColor = GUI.color;
				if (property.boolValue == requiredBool.defaultOkForBuilds)
				{
					GUI.color = Color.green;
				}
				else
				{
					GUI.color = Color.red;
				}
				property.boolValue = EditorGUI.Toggle(position, label, property.boolValue);
				GUI.color = previousGUIColor;
			}
			else
			{
				EditorGUI.LabelField(position, label.text, "Use Range with bool");
			}
		}
	}
}