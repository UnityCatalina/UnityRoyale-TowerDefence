using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemConf : MonoBehaviour
{

    public int fps = 30;
  
    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = fps;
    }

}
