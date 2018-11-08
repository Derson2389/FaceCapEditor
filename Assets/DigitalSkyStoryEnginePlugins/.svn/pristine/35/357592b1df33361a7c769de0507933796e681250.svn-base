using System.Text;
using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;

public static class SynMappingParser
{
    public static Dictionary<BipBoneHelper.BipBone, string> ParseMapping(string fileName)
    {
        Dictionary<BipBoneHelper.BipBone, string> dict = new Dictionary<BipBoneHelper.BipBone, string>();

        bool doesExist = File.Exists(fileName);
        if (doesExist == false)
            return dict;

        try
        {
            string line;
            StreamReader theReader = new StreamReader(fileName, Encoding.Default);
            using (theReader)
            {
                do
                {
                    line = theReader.ReadLine();

                    if (line != null)
                    {
                        string[] entries = line.Split('=');
                        if (entries.Length > 0)
                        {
                            string boneName = entries[0];
                            string skelName = entries[1].Replace(" ", "");

                            object bipBone = Enum.Parse(typeof(BipBoneHelper.BipBone), boneName);

                            if (bipBone != null)
                            {
                                dict.Add((BipBoneHelper.BipBone)bipBone, skelName);
                            }
                            else
                            {
                                Debug.LogWarning("SynMappingParser boneName error:" + boneName);
                            }
                        }
                    }
                }
                while (line != null);
                theReader.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SynMapping Read Error: " + e.Message.ToString());
            return dict;
        }

        return dict;
    }
}