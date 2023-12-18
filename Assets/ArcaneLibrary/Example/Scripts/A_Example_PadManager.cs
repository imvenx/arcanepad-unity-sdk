using ArcanepadSDK.Types;
using UnityEngine;
using UnityEngine.UI;

namespace ArcanepadExample
{
    public class A_Example_PadManager : MonoBehaviour
    {
        public Button AttackButton;
        public Button CalibrateQuaternionButton;

        async void Awake()
        {
            Arcane.Init(new ArcaneInitParams(deviceType: ArcaneDeviceType.pad, padOrientation: AOrientation.Portrait));

            await Arcane.ArcaneClientInitialized();

            Arcane.Msg.On("TakeDamage", (TakeDamage e) => Arcane.Pad.Vibrate(100 * e.damage));

            AttackButton.onClick.AddListener(Attack);

            CalibrateQuaternionButton.onClick.AddListener(() => Arcane.Pad.CalibrateQuaternion());
        }

        void Attack()
        {
            Arcane.Msg.EmitToViews(new AttackEvent(5));
        }
    }
}