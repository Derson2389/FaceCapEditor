using UnityEngine;

[System.Serializable]
public class CutsceneClip
{
    public string fullname = string.Empty;
    public string name = string.Empty;
    public string belongActName = string.Empty;
    public Texture2D image = null;

    public Texture2D IMGAGE
    {

        get
        {
            if (image == null)
            {
                image = CutsceneAssetsManagerUtility.LoadImage(fullname);
            }

            return image;
        }
    }
}
