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

        // Pad.Emit(new AttackEvent(99));

        Pad.StartGetQuaternion();
        Pad.OnGetQuaternion((GetQuaternionEvent e) =>
        {
            if (PlayerManager.isGamePaused) return;
            transform.rotation = new Quaternion(e.x, e.y, e.z, e.w);
        });

        Pad.StartGetPointer();
        pad.OnGetPointer((GetPointerEvent e) =>
        {
            Debug.Log(e.x + " | " + e.y);
        });

        Debug.Log(Pad.User.name);

    }

}
