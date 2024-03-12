using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class PreloadSigningAlias
{
    static PreloadSigningAlias ()
    {
        PlayerSettings.Android.keystorePass = "fX72_88PK34N93g";
        PlayerSettings.Android.keyaliasName = "dual pistolas";
        PlayerSettings.Android.keyaliasPass = "fX72_88PK34N93g";
    }
    
}
