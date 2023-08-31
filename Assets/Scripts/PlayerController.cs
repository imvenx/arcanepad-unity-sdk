using System.Collections;
using System.Collections.Generic;
using ArcanepadSDK.Models;
using ArcanepadSDK.PadEvents;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public IframePad PadData { get; private set; }

    public void Initialize(IframePad pad)
    {
        this.PadData = pad;
    }

    void Start()
    {
        Arcane.pad.On(AEventName.Left, PadData.ClientId, (LeftEvent e) =>
        {
            this.transform.Translate(Vector3.left);
            Debug.Log("moved to  the left");
        });
        Arcane.pad.On(AEventName.Right, PadData.ClientId, (RightEvent e) =>
        {
            this.transform.Translate(Vector3.right);
            Debug.Log("moved to  the Right");
        });
        Arcane.pad.On(AEventName.Up, PadData.ClientId, (UpEvent e) =>
        {
            this.transform.Translate(Vector3.up);
            Debug.Log("moved to  the Up");
        });
        Arcane.pad.On(AEventName.Down, PadData.ClientId, (DownEvent e) =>
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
