
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/**
 * @author PerezPrieto
 */
[RequireComponent(typeof(Inventory))]
[Serializable]
public class Character: MonoBehaviour{

    public Inventory inventory
    {
        get
        {
            return this.GetComponent<Inventory>();
        }
    }

    public List<Slot> Slots;

    // Custom Editor class for checking Slots and Items
    [CustomEditor(typeof(Character))]
    public class MyScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Show dinamic array panel
            serializedObject.Update();            
            SerializedProperty tps = serializedObject.FindProperty("Slots");
            EditorGUI.BeginChangeCheck();            
            EditorGUILayout.PropertyField(tps, true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                SlotEditorManager();
            }
        }

        private void SlotEditorManager()        {
           
            var myScript = target as Character;
            
            // Check if item correspond with slot
            for (int i = 0; i < myScript.Slots.Count; i++)
            {
                Boolean correct = true;
                Item item = myScript.Slots[i].item;
                // Si el slot actual tiene item
                if (myScript.Slots[i].item != null)
                {
                    // Si el item tiene este tipo de slot
                    int posAct = myScript.Slots[i].item.SlotType.FindIndex(
                            delegate (SlotType slotType)
                            {
                                return slotType == myScript.Slots[i].slotType;
                            });
                    // Si no lo contiene, error 
                    if (posAct == -1)
                    {
                        Debug.Log("Error: " +
                            "The item " + myScript.Slots[i].item.name +
                            " does not correspond with the slot type " + myScript.Slots[i].slotType);
                        correct = false;
                    }

                    //Para cada tipo de slot del item
                    for (int j = 0; j < myScript.Slots[i].item.SlotType.Count; j++)
                    {
                        //si esta definido el tipo de slot
                        if (myScript.Slots[i].slotType != null)
                        {
                            // Si el slot es distinto del actual
                            if (myScript.Slots[i].item.SlotType[j].GetInstanceID() != myScript.Slots[i].slotType.GetInstanceID())
                            {
                                // Buscamos el slot correspondiente
                                int pos = myScript.Slots.FindIndex(
                                        delegate (Slot slot) {
                                            return slot.slotType == myScript.Slots[i].item.SlotType[j];
                                        });

                                // Si tiene un item o no exite, error
                                if (pos == -1 || myScript.Slots[pos].item != null)
                                {
                                    Debug.Log("Error: " +
                                                "The slot " + myScript.Slots[i].slotType + " already have an item");
                                    correct = false;
                                }
                            }
                        }
                    }
                    if (!correct) // Si no es correcto lo desasignamos
                    {
                        myScript.Slots[i].item = null;
                    }
                    else // Si es correcto lo asignamos a todos los slots del item
                    {
                        for (int h = 0; h < myScript.Slots[i].item.SlotType.Count; h++)
                        {
                            int pos = myScript.Slots.FindIndex(
                                delegate (Slot slot) {
                                    return slot.slotType == myScript.Slots[i].item.SlotType[h];
                                });
                            myScript.Slots[pos].item = item;
                        }
                    }
                }   
            }   
        }
    }
}