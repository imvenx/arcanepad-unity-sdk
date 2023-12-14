using System.Collections;
using System.Collections.Generic;
using ArcanepadSDK.Types;
using Newtonsoft.Json;
using UnityEngine;

namespace ArcanepadExample
{
    public class APadManager : MonoBehaviour
    {
        async void Awake()
        {
            Arcane.Init(new ArcaneInitParams(deviceType: ArcaneDeviceType.pad));

            await Arcane.ArcaneClientInitialized();

            Arcane.Pad.Vibrate(500);
        }
    }
}