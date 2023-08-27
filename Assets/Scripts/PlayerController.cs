using System.Collections;
using System.Collections.Generic;
using ArcanepadSDK.Models;
using ArcanepadSDK.PadEvents;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Arcane.msg.On(AEventName.Left, (LeftEvent e, string from) =>
        {
            this.transform.Translate(Vector3.left);
            Debug.Log("moved to  the left");
        });
        Arcane.msg.On(AEventName.Right, (RightEvent e, string from) =>
        {
            this.transform.Translate(Vector3.right);
            Debug.Log("moved to  the Right");
        });
        Arcane.msg.On(AEventName.Up, (UpEvent e, string from) =>
        {
            this.transform.Translate(Vector3.up);
            Debug.Log("moved to  the Up");
        });
        Arcane.msg.On(AEventName.Down, (DownEvent e, string from) =>
        {
            this.transform.Translate(Vector3.down);
            Debug.Log("moved to  the Down");
        });
    }

    // Update is called once per frame
    void Update()
    {
        // this.transform.Translate(Vector3.forward);
    }
}
