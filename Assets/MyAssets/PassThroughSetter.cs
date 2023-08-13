using Unity.XR.PXR;
using UnityEngine;

public class PassThroughSetter : MonoBehaviour
{
    void Start()
    {
        // シースルー設定
        PXR_Boundary.EnableSeeThroughManual(true);   
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        // Pauseからの復帰時、再度シースルーにする
        if (!pauseStatus)
        {
            PXR_Boundary.EnableSeeThroughManual(true);
        }
    }
}

