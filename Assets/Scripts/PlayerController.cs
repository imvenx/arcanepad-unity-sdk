using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArcanepadSDK;
using ArcanepadSDK.CustomModels;
using ArcanepadSDK.Models;
using ArcanepadSDK.PadEvents;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ArcanePad pad { get; private set; }

    public void Initialize(ArcanePad pad)
    {
        this.pad = pad;
    }

    void Start()
    {
        pad.On(AEventName.Left, (LeftEvent e) =>
        {
            transform.Translate(Vector3.left);
        });
        pad.On(AEventName.Right, (RightEvent e) =>
        {
            transform.Translate(Vector3.right);
        });
        pad.On(AEventName.Up, (UpEvent e) =>
        {
            transform.Translate(Vector3.up);
        });
        pad.On(AEventName.Down, (DownEvent e) =>
        {
            transform.Translate(Vector3.down);
        });

        pad.Emit(new AttackEvent(99));

        pad.StartGetRotationVector();
        pad.OnGetRotationVector((GetRotationVectorEvent e) =>
        {
            transform.rotation = Quaternion.Euler(e.azimuth, e.pitch, e.roll);
        });

    }

    void OnDestroy()
    {
        pad.Off();
    }
}
