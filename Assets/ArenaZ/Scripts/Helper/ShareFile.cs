using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShareFile : MonoBehaviour
{
    private string appString = "Download the latest version of Arena Z by going to ";
    private NativeShare nativeShare = new NativeShare();

    public void Share()
    {
        Debug.Log("share: " + DownloadUrl.url);
        nativeShare.SetText(appString + DownloadUrl.url).Share();
    }
}
