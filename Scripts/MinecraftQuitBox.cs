using GorillaLocomotion;
using HarmonyLib;
using UnityEngine;

namespace DevMinecraftMod.Scripts
{
    public class MinecraftQuitBox : GorillaTriggerBox
    {
        void Start()
        {
            gameObject.layer = 15;
        }

        public override void OnBoxTriggered()
        {
            Vector3 target = new Vector3(-64f, 12.534f, -83.014f);

            Traverse.Create(GTPlayer.Instance).Field("lastPosition").SetValue(target);
            Traverse.Create(GTPlayer.Instance).Field("lastLeftHandPosition").SetValue(target);
            Traverse.Create(GTPlayer.Instance).Field("lastRightHandPosition").SetValue(target);
            Traverse.Create(GTPlayer.Instance).Field("lastHeadPosition").SetValue(target);

            GTPlayer.Instance.leftControllerTransform.position = target;
            GTPlayer.Instance.rightControllerTransform.position = target;
            GTPlayer.Instance.bodyCollider.attachedRigidbody.transform.position = target;

            GTPlayer.Instance.GetComponent<Rigidbody>().position = target;
            GTPlayer.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
