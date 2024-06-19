using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using TMPro;
using Photon.Pun;

namespace DealCancel.Patches
{
    public class NetworkDealBossHook
    {
        private static readonly ModalOption[] _cancelModalOptions = [
            new("<color=red>Cancel deal</color>", () => NetworkDealBoss.me.RemoveDeal(NetworkDealBoss.activeDeal)),
            new("Dismiss")
        ];

        internal static void Init()
        {
            On.NetworkDealBoss.RPCA_AddDeal += MMHook_Postfix_SpawnCancelButton;
        }

        private static void MMHook_Postfix_SpawnCancelButton(On.NetworkDealBoss.orig_RPCA_AddDeal orig, NetworkDealBoss self, byte dealIndex, byte rewardIndex, DIFFICULTY difficulty)
        {
            try // cut out possible issues when sponsorship dont have sponsor item
            {
                orig(self, dealIndex, rewardIndex, difficulty);
            }
            catch { }
            if (GameObject.Find("CancelDealButton") == null && PhotonNetwork.IsMasterClient)
            {
                Transform _monitorCanvasTransform = GameObject.Find("NetworkDealStation/Monitor/MonitorCanvas").transform;
                GameObject _cancelButtonOriginal = _monitorCanvasTransform.Find("DealFailed/AcceptButton").gameObject;
                GameObject _cancelDealButtonObject = Object.Instantiate(_cancelButtonOriginal, _cancelButtonOriginal.transform.position, new Quaternion(0, 0, 0, 0), _monitorCanvasTransform.Find("DealProgress"));
                _cancelDealButtonObject.name = "CancelDealButton";
                _cancelDealButtonObject.GetComponent<Button>().onClick.AddListener(() => Modal.Show("are you sure to cancel current sponsorship?", "After cancelling current deal, you will be unable to receive another one later unless you are using DealReroll.", _cancelModalOptions));
                Transform _outlineTransform = _cancelDealButtonObject.transform.Find("outline");
                _outlineTransform.GetComponent<ProceduralImage>().color = Color.red;
                TextMeshProUGUI _textMesh = _outlineTransform.GetComponentInChildren<TextMeshProUGUI>();
                _textMesh.text = "CANCEL DEAL";
                _textMesh.fontSize = 45;
            }
        }
    }
}
