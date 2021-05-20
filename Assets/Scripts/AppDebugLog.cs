using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class AppDebugLog {
    public static string logStr = ""; // Everything goes here. Never cleared.


    // NOTE: No difference between normal logs and errors yet!! Might add if we want in the future.
    public static void LogError(string str) {
        Debug.LogError(str);
        logStr += str + "\n";
    }
}
