using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SheetViewer : EditorWindow
{

    [MenuItem("Window/GoogleSheet")]
    public static void OpenPanel()
    {

        var window = CreateWindow<SheetViewer>();
        window.Show();

    }

    private void OnEnable()
    {
        
        TextField documentIDField = new TextField("documentID");
        TextField gidField = new TextField("gid");

        Button getBtn = new Button(() =>
        {

            GoogleSheet.GetSheetData(documentIDField.value, gidField.value, this, (b, s) =>
            {

                if (b)
                {

                    Debug.Log(s);

                }
                else
                {

                    Debug.Log("실패");

                }

            });

        });

        getBtn.style.height = new StyleLength(30);

        rootVisualElement.Add(documentIDField);
        rootVisualElement.Add(gidField);
        rootVisualElement.Add(getBtn);

    }

}