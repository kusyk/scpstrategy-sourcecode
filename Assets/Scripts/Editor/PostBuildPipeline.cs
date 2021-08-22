using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class PostBuildPipeline
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if(target != BuildTarget.StandaloneWindows && target != BuildTarget.StandaloneWindows64)
        {
            Debug.LogError("It is not a windows build. Cannot remove git files.");
            return;
        }

        pathToBuiltProject = Path.GetDirectoryName(pathToBuiltProject);
        string debug = "Removing all git files from " + pathToBuiltProject + "...";
        string gitIgnorePath = pathToBuiltProject + @"\SCP_Strategy_Data\StreamingAssets\Game\.gitignore";

        if (File.Exists(gitIgnorePath))
        {
            File.Delete(gitIgnorePath);
            debug += "\n.gitignore deleted.\n" + gitIgnorePath;
        }
        else
        {
            debug += "\n.gitignore does not exist.\n" + gitIgnorePath;
        }

        pathToBuiltProject += @"\SCP_Strategy_Data\StreamingAssets\Game\.git\";


        UpdateFileAttributes(new DirectoryInfo(pathToBuiltProject));


        if (Directory.Exists(pathToBuiltProject))
        {
            Directory.Delete(pathToBuiltProject, true);
        }
        else
        {
            debug += "\nPath does not exist.";
        }

        Debug.Log(debug);
    }
    private static void UpdateFileAttributes(DirectoryInfo dInfo)
    {
        // Set Directory attribute
        dInfo.Attributes &= ~FileAttributes.ReadOnly;

        // get list of all files in the directory and clear 
        // the Read-Only flag

        foreach (FileInfo file in dInfo.GetFiles())
        {
            file.Attributes &= ~FileAttributes.ReadOnly;
        }

        // recurse all of the subdirectories
        foreach (DirectoryInfo subDir in dInfo.GetDirectories())
        {
            UpdateFileAttributes(subDir);
        }
    }


}
