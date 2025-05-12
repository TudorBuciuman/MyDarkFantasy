using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IconManager : MonoBehaviour
{
    public string newAppName = "My dark fantasy";
    public string shortcutName = "My dark fantasy"; // without .lnk
    public string icoFileName = "dark.ico"; // Should be in StreamingAssets
    public static byte scene = 2;
    public void Start()
    {
        Application.quitting += OnQuit;
    }
    public void Update()
    {
        if (Input.GetKey(KeyCode.V))
        {
            UnityEngine.Debug.Log("0j");
            SceneManager.LoadScene("Fight");
            //Application.Quit();
        }
    }
    public void Switching()
    {
        switch (scene)
        {
            case 2:
                {
                    newAppName = "YEEZUS";
                    shortcutName = "YEEZUS";
                    icoFileName = "cd.ico";
                    break;
                }
            case 3:
                {
                    newAppName = "It's too late";
                    shortcutName = "YEEZUS";
                    icoFileName = "dark.ico";
                    break;
                }
        }
    }
    public void OnQuit()
    {
        Switching();
        try
        {
            UnityEngine.Debug.Log("1");
            string icoPath = Path.Combine(Application.streamingAssetsPath, icoFileName);
            string batPath = Path.Combine(Application.dataPath, "../update_shortcut.bat");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("@echo off");
            sb.AppendLine("set SCRIPT=\"%TEMP%\\changeShortcut.vbs\"");
            sb.AppendLine("echo Set oWS = WScript.CreateObject(\"WScript.Shell\") > %SCRIPT%");
            sb.AppendLine($"echo sLinkFile = \"%USERPROFILE%\\Desktop\\{shortcutName}.lnk\" >> %SCRIPT%");
            sb.AppendLine("echo Set oLink = oWS.CreateShortcut(sLinkFile) >> %SCRIPT%");
            sb.AppendLine($"echo oLink.Description = \"{newAppName}\" >> %SCRIPT%");
            sb.AppendLine($"echo oLink.IconLocation = \"{icoPath},0\" >> %SCRIPT%");
            sb.AppendLine("echo oLink.Save >> %SCRIPT%");
            sb.AppendLine("cscript /nologo %SCRIPT%");
            sb.AppendLine("del %SCRIPT%");

            File.WriteAllText(batPath, sb.ToString());

            Process.Start(new ProcessStartInfo
            {
                FileName = batPath,
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });

        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Failed to update shortcut: " + e.Message);
        }
    }
}
