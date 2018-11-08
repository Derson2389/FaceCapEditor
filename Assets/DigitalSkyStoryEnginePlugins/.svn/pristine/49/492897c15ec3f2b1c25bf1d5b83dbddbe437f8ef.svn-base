using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SvnForUnity{
 
    static string SVN_BASE = CutsceneAssetsManagerUtility.workspace;
    static string PROJECT_NAME = "tq";
    static string ACT_NAME = "tq1";
    [MenuItem("SVN/Update", false, 1)]
    public static void SvnUpdate()
    {
        processCommand("svn", "update \""+SVN_BASE+"\"");
    }



    public static void SvnPreUpdate(string projectName, string actName = null , bool needUpdateRecordData = true)
    {
        string TestDir = Path.Combine(SVN_BASE, projectName);
        string StoryDir = Path.Combine(CutsceneAssetsManagerUtility._storyStr, actName);
        string DesDir = Path.Combine(TestDir, StoryDir);
        
        processCommand("svn", "update \"" + DesDir + "\"");
        if (needUpdateRecordData)
        {
            string RecordDataPath = UnityEngine.PlayerPrefs.GetString("TrackerEditorWindow_savePath", "RecordDatas/");
            string RecordDataDir = Path.Combine(Application.dataPath, RecordDataPath);
            processCommand("svn", "update \"" + RecordDataDir + "\"");
        }
    }


    public static void SvnAddFolderAndFile(string projectName, string actName = null)
    {
        string TestDir = Path.Combine(SVN_BASE, projectName);
        if (actName != null)
        {
            string StoryDir = Path.Combine(CutsceneAssetsManagerUtility._storyStr, actName);
            string DesDir = Path.Combine(TestDir, StoryDir);
            string metaFile = Path.Combine(StoryDir, DesDir + ".meta");
            processCommand("svn", "add \"" + DesDir + "\"" + " --force");
            processCommand("svn", "add \"" + metaFile + "\"");
        }
        else
        {
            string metaFile = Path.Combine(SVN_BASE, projectName + ".meta");
            processCommand("svn", "add \"" + TestDir + "\"" + " --force");
            processCommand("svn", "add \"" + metaFile + "\"");
        }
    }

    public static void SvnCommitFolderAndFile(string projectName, string actName = null, bool needUpdateRecordData = true)
    {
        ///提交录制数据
        ///
        if (needUpdateRecordData)
        {
            string RecordDataPath = UnityEngine.PlayerPrefs.GetString("TrackerEditorWindow_savePath", "RecordDatas/");
            string RecordDataDir = Path.Combine(Application.dataPath, RecordDataPath);
            string CommitRecordDataDesc = " -m " + "\"+ Auto create and update new preject!!!\"";
            processCommand("svn", "add \"" + RecordDataDir + "\"" + " --force");
            processCommand("svn", "commit \"" + RecordDataDir + "\"" + CommitRecordDataDesc);
        }
        ///

        string TestDir = Path.Combine(SVN_BASE, projectName);
        string CommitDesc =" -m " + "\"+ Auto create and update new preject!!!\"";
        if (actName == null)
        {
            string metaFile = Path.Combine(SVN_BASE, projectName + ".meta");
            processCommand("svn", "commit \"" + TestDir + "\""+ CommitDesc);
            processCommand("svn", "commit \"" + metaFile + "\"" + CommitDesc);
        }
        else
        {
            string StoryDir = Path.Combine(CutsceneAssetsManagerUtility._storyStr, actName);
            string metaFileDir = Path.Combine(TestDir, CutsceneAssetsManagerUtility._storyStr);
            string DesDir = Path.Combine(TestDir, StoryDir);
            string metaFile = Path.Combine(metaFileDir, actName+".meta");
            processCommand("svn", "commit \"" + DesDir + "\""+ CommitDesc);
            processCommand("svn", "commit \"" + metaFile + "\"" + CommitDesc);
        }
    }

    private static void processCommand(string command, string argument)
    {
        ProcessStartInfo start = new ProcessStartInfo(command);
        start.Arguments = argument;
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = true;
 
        if(start.UseShellExecute)
        {
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;
            start.RedirectStandardInput = false;
        }
        else
        {
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }
 
        Process p = Process.Start(start);
 
        if(!start.UseShellExecute)
        {
            UnityEngine.Debug.Log(p.StandardOutput);
            UnityEngine.Debug.Log(p.StandardError);
        }
 
        p.WaitForExit();
        p.Close();
    }
 
}