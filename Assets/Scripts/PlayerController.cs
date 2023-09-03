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
    public ArcanePad Pad { get; private set; }
    private Quaternion InitialQuaternion = new Quaternion(0, 0, 0, 0);
    public void Initialize(ArcanePad pad)
    {
        Pad = pad;
        Pad.On(AEventName.Left, (LeftEvent e) =>
       {
           transform.Translate(Vector3.left);
           Pad.Vibrate(200);
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

        Pad.Emit(new AttackEvent(99));

        Pad.StartGetQuaternion();


        // bool firstTime = true;
        Pad.OnGetQuaternion((GetQuaternionEvent e) =>
        {
            // if (firstTime)
            // {
            //     initialQuaternion = new Quaternion(-e.x, -e.y, e.z, e.w);
            //     firstTime = false;
            // }
            // transform.rotation = Quaternion.Inverse(initialQuaternion) * new Quaternion(-e.x, -e.y, e.z, e.w);
            transform.rotation = new Quaternion(e.x, e.y, e.z, e.w);

        });

    }

    void OnDestroy()
    {
        Pad.Off();
    }
}
