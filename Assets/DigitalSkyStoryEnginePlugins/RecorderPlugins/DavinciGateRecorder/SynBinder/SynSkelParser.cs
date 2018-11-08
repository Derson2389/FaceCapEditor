using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public static class SynSkelParser
{
    public enum SkelUnits
    {
        Centimeters,
        Inches
    }

    [System.Serializable]
    public class Skel
    {
        public string name;
        public SkelUnits units;
        public int referenceIMU;
        public int trackerID;
        public int numNodes;
        [System.Serializable]
        public class Node
        {
            public int id;
            public string nodeName;
            public int flags;
            public Vector3 pos;
        }
        public Node[] nodes;
        public int numBones;
        [System.Serializable]
        public class Bone
        {
            public int id;
            public string boneName;
            public int flags;
            public int parent;
            public int child;
            public int[] IMUs;
            public float imuScale;
            public float thickness;
        }
        public Bone[] bones;
    }
    public static Skel Parse(string filename)
    {
        // New skel
        Skel skel = new Skel();
        
        // Handle any problems that might arise when reading the text
        try
        {
            string line;
            // Create a new StreamReader, tell it which file to read and what encoding the file
            // was saved as
            StreamReader theReader = new StreamReader(filename, Encoding.Default);
            // Immediately clean up the reader after this block of code is done.
            // You generally use the "using" statement for potentially memory-intensive objects
            // instead of relying on garbage collection.
            // (Do not confuse this with the using directive for namespace at the 
            // beginning of a class!)
            using (theReader)
            {
                // While there's lines left in the text file, do this:
                do
                {
                    line = theReader.ReadLine();
					//Debug.Log("Parsing line: " + line);
                    if (line != null)
                    {
                        // Get words
                        string[] words = line.Split(new char[0]);

                        // Header
                        switch (words[0])
                        {
                            case "Name":
                                for (int j = 1; j < words.Length; j++)
                                    skel.name += words[j];
                                break;
                            case "Units":
                                switch (words[1])
                                {
                                    case "Inches":
                                        skel.units = SkelUnits.Inches;
                                        break;
                                    case "Centimeters":
                                        skel.units = SkelUnits.Centimeters;
                                        break;
                                }
                                break;
                            case "ReferenceIMU":
                                Int32.TryParse(words[1], out skel.referenceIMU);
                                break;
                            case "ReferenceTracker":
                                Int32.TryParse(words[1], out skel.trackerID);
                                break;
                            case "NumNodes":
                                Int32.TryParse(words[1], out skel.numNodes);
                                skel.nodes = new Skel.Node[skel.numNodes];
                                break;
                            case "NumBones":
                                Int32.TryParse(words[1], out skel.numBones);
                                skel.bones = new Skel.Bone[skel.numBones];
                                break;
                        }

                        // Nodes
                        if (words[0] == "Node")
                        {
                            Skel.Node node = new Skel.Node();

                            // ID
                            Int32.TryParse(words[1], out node.id);

                            // Name
                            string[] nameWords = ParseTabbedLine(theReader.ReadLine());
                            if (nameWords.Length > 1)
                                for (int j = 1; j < nameWords.Length; j++)
                                    node.nodeName += nameWords[j];
                            
                            // Flags
                            string[] flagWords = ParseTabbedLine(theReader.ReadLine());
                            if (flagWords.Length > 1)
                                Int32.TryParse(flagWords[1], out node.flags);
                            
                            // Position
                            string[] posWords = ParseTabbedLine(theReader.ReadLine());
                            if (posWords.Length > 3)
                            {
                                float x, y, z;
                                float.TryParse(posWords[1], out x);
                                float.TryParse(posWords[2], out y);
                                float.TryParse(posWords[3], out z);
                                Vector3 pos = new Vector3(x, z, y);
                                node.pos = pos;
                            }

                            // Add the skel node
                            skel.nodes[node.id] = node;
                        }

                        // Bones
                        if (words[0] == "Bone")
                        {
                            Skel.Bone bone = new Skel.Bone();

                            // ID
                            Int32.TryParse(words[1], out bone.id);
							//Debug.Log("ID: "+words[1]);
                            // Name
                            string[] nameWords = ParseTabbedLine(theReader.ReadLine());
                            if (nameWords.Length > 1)
                                for (int j = 1; j < nameWords.Length; j++)
                                    bone.boneName += nameWords[j];
                            
                            // Flags
                            string[] flagWords = ParseTabbedLine(theReader.ReadLine());
                            if (flagWords.Length > 1)
                                Int32.TryParse(flagWords[1], out bone.flags);

                            // Parent
                            string[] parentWords = ParseTabbedLine(theReader.ReadLine());
                            if (parentWords.Length > 1)
                            {
                                Int32.TryParse(parentWords[1], out bone.parent);
                            }
                                
                            // Child
                            string[] childWords = ParseTabbedLine(theReader.ReadLine());
                            if (childWords.Length > 1)
                            {
                                Int32.TryParse(childWords[1], out bone.child);
                            }
                            
                            // IMUs
                            string[] nextLine = ParseTabbedLine(theReader.ReadLine());
                            if(nextLine[0] == "IMUs")
                            {
                                if (nextLine.Length > 1)
                                {
                                    bone.IMUs = new int[nextLine.Length - 1];
                                    for (int j = 0; j < nextLine.Length - 1; j++)
                                    {
                                        string imu = nextLine[j + 1];
                                        Int32.TryParse(imu, out bone.IMUs[j]);
                                    }
                                }
                                nextLine = ParseTabbedLine(theReader.ReadLine());
                            }
							
                            // Imu scale
                            if(nextLine[0] == "IMUScale")
                            {
                                if (nextLine.Length > 1)
                                    float.TryParse(nextLine[1], out bone.imuScale);
                                nextLine = ParseTabbedLine(theReader.ReadLine());
                            }

                            // Thickness
                            if(nextLine[0] == "Thickness")
                                if (nextLine.Length > 1)
                                    float.TryParse(nextLine[1], out bone.thickness);

                            // Add the skel bones
                            skel.bones[bone.id] = bone;
                        }
                    }
                }
                while (line != null);
                // Done reading, close the reader and return true to broadcast success    
                theReader.Close();
                return skel;
            }
        }
        // If anything broke in the try block, we throw an exception with information
        // on what didn't work
        catch (Exception e)
        {
            Debug.LogError("Skel read error: " + e.Message);
            return null;
        }
    }
        
    static string[] ParseTabbedLine(string line)
    {
        string[] nodeLine = line.Split('\t');
        string data = nodeLine[1];
        return data.Split(new char[0]);
    }
}