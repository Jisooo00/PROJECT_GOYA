using System;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEngine.Networking;

public static class GoogleSheet
{

    public static void GetSheetData(string documentID, string sheetID, object onwer,Action<bool, string> process = null)
    {

        EditorCoroutineUtility.StartCoroutine(GetSheetDataCo(documentID, sheetID, process), onwer);

    }

    private static IEnumerator GetSheetDataCo(string documentID, string sheetID, Action<bool, string> process = null)
    {

        string url = $"https://docs.google.com/spreadsheets/d/{documentID}/export?format=tsv&gid={sheetID}";

        UnityWebRequest req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError || req.responseCode != 200)
        {

            process?.Invoke(false, null);
            yield break;

        }

        process?.Invoke(true, req.downloadHandler.text);

    }

}