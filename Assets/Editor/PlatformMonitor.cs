using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class PlatformMonitor {
    //static BuildTarget cachedPlatform;
    
    static PlatformMonitor() {
        //cachedPlatform = EditorUserBuildSettings.activeBuildTarget;
        // EditorApplication.update += Update;
        EditorUserBuildSettings.activeBuildTargetChanged += OnChangedPlatform;
    } 
    
    static void Update() {
        //     if ( EditorUserBuildSettings.activeBuildTarget != cachedPlatform ) {
        //         OnChangedPlatform();
        //         cachedPlatform = EditorUserBuildSettings.activeBuildTarget;
        //     }
    }
    
    static void OnChangedPlatform()
    {
        Debug.Log( "Changed Platform to " + EditorUserBuildSettings.activeBuildTarget );

        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            PlayerSettings.bundleIdentifier = "com.DualPistolas.ClimbyTower";
        }
        else if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            PlayerSettings.bundleIdentifier = "com.dualpistolas.climbytower1";
        }
        
        //cachedPlatform = EditorUserBuildSettings.activeBuildTarget;
    }
}