using ArcanepadSDK;
using ArcanepadSDK.Models;
using ArcanepadSDK.APadEvents;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public ArcanePad Pad { get; private set; }
    public void Initialize(ArcanePad pad)
    {
        Pad = pad;
        Pad.On(AEventName.Left, (ALeftEvent e) =>
       {
           transform.Translate(Vector3.left);
           Pad.Vibrate(200);
       });
        Pad.On(AEventName.Right, (ARightEvent e) =>
        {
            transform.Translate(Vector3.right);
        });
        Pad.On(AEventName.Up, (AUpEvent e) =>
        {
            transform.Translate(Vector3.up);
        });
        Pad.On(AEventName.Down, (ADownEvent e) =>
        {
            transform.Translate(Vector3.down);
        });

        // Pad.Emit(new AttackEvent(99));

        Pad.StartGetQuaternion();
        Pad.OnGetQuaternion((GetQuaternionEvent e) =>
        {
            if (AViewManager.isGamePaused) return;
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
