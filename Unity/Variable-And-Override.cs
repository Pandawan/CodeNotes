//drawer for the variable override
[CustomPropertyDrawer(typeof(VariableOverrideAttribute))]
public class EditorVariableOverrideAttribute : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 16), property);
        if(property.isExpanded)
        {
            Rect rectType = new Rect(EditorGUI.IndentedRect(position).x, position.y + 16, EditorGUI.IndentedRect(position).width, 16);
            Rect rectValue = new Rect(EditorGUI.IndentedRect(position).x, position.y + 32, EditorGUI.IndentedRect(position).width, 16);

            EditorGUI.PropertyField(rectType, property.FindPropertyRelative("variable"), new GUIContent("Variable"), true);

            Variable variable = property.FindPropertyRelative("variable")?.objectReferenceValue as Variable;
            if (variable)
            {
                if (variable.type == TypeEnum.String)
                {
                    property.FindPropertyRelative("value").stringValue = EditorGUI.TextField(rectValue, "Value", property.FindPropertyRelative("value").stringValue);
                }
                else if (variable.type == TypeEnum.Int)
                {
                    int result;
                    if (!int.TryParse(property.FindPropertyRelative("value").stringValue, out result))
                    {
                        property.FindPropertyRelative("value").stringValue = "0";
                    }

                    property.FindPropertyRelative("value").stringValue = EditorGUI.IntField(rectValue, "Value", int.Parse(property.FindPropertyRelative("value").stringValue)).ToString();
                }
                else if (variable.type == TypeEnum.Float)
                {
                    float result;
                    if (!float.TryParse(property.FindPropertyRelative("value").stringValue, out result))
                    {
                        property.FindPropertyRelative("value").stringValue = "0";
                    }

                    property.FindPropertyRelative("value").stringValue = EditorGUI.FloatField(rectValue, "Value", float.Parse(property.FindPropertyRelative("value").stringValue)).ToString();
                }
                else if (variable.type == TypeEnum.Bool)
                {
                    property.FindPropertyRelative("value").stringValue = EditorGUI.Toggle(rectValue, "Value", property.FindPropertyRelative("value").stringValue.ToLower() == "true") ? "true" : "false";
                }
                else if (variable.type == TypeEnum.AudioClip)
                {
                    List<string> names = new List<string>();
                    List<AudioClip> audioClips = ReferenceManager.GetAll<AudioClip>();
                    int selectedIndex = -1;
                    for (int i = 0; i < audioClips.Count; i++)
                    {
                        if (audioClips[i] != null)
                        {
                            names.Add(audioClips[i].name);
                            if (audioClips[i].name == property.FindPropertyRelative("value").stringValue)
                            {
                                selectedIndex = i;
                            }
                        }
                    }

                    names.Add("None");
                    if (selectedIndex == -1) selectedIndex = names.Count - 1;

                    int newIndex = EditorGUI.Popup(rectValue, "Default", selectedIndex, names.ToArray());
                    if (newIndex == audioClips.Count) property.FindPropertyRelative("value").stringValue = "";
                    else property.FindPropertyRelative("value").stringValue = audioClips[newIndex].name;
                }
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * (property.isExpanded ? 3f : 1f);
    }
}

//drawer for the variable definition
[CustomPropertyDrawer(typeof(Variable))]
public class EditorVariableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.hasMultipleDifferentValues)
        {
            EditorGUI.LabelField(position, label.text, "---");
            return;
        }

        List<string> names = new List<string>();
        List<Variable> variables = ReferenceManager.GetAll<Variable>();
        int selectedIndex = -1;
        for (int i = 0; i < variables.Count; i++)
        {
            if (variables[i] != null)
            {
                names.Add(variables[i].name);
                if (variables[i] == property.objectReferenceValue)
                {
                    selectedIndex = i;
                }
            }
        }

        names.Add("None");
        if (selectedIndex == -1) selectedIndex = names.Count - 1;

        int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, names.ToArray());
        if (newIndex == variables.Count) property.objectReferenceValue = null;
        else property.objectReferenceValue = variables[newIndex];

        property.serializedObject.ApplyModifiedProperties();
    }
}