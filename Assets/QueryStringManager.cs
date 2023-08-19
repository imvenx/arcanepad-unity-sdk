using UnityEngine;
using System.Runtime.InteropServices;

public class QueryStringManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetUrlParam(string param);

    void Start()
    {
        string value = GetUrlParam("myParam");
        Debug.Log("URL parameter value: " + value);
        Debug.Log("absolute URL!: " + Application.absoluteURL);
    }
}
