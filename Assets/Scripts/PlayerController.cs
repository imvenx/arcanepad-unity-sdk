using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArcanepadSDK;
using ArcanepadSDK.Models;
using ArcanepadSDK.PadEvents;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ArcanePad Pad { get; private set; }

    public void Initialize(ArcanePad pad)
    {
        Pad = pad;
    }

    void Start()
    {
        Pad.On(AEventName.Left, (LeftEvent e) =>
        {
            transform.Translate(Vector3.left);
        });
        Pad.On(AEventName.Right, (RightEvent e) =>
        {
            transform.Translate(Vector3.right);
        });
        Pad.On(AEventName.Up, (UpEvent e) =>
        {
            transform.Translate(Vector3.up);
        });
        Pad.On(AEventName.Down, (DownEvent e) =>
        {
            transform.Translate(Vector3.down);
        });
    }

    void OnDestroy()
    {
        Pad.Off();
    }
}
