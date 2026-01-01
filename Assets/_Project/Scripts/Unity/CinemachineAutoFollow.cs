using UnityEngine;
using Unity.Cinemachine; // Cinemachine 3.x 專用命名空間

[RequireComponent(typeof(CinemachineCamera))]
public class CinemachineAutoFollow : MonoBehaviour
{
    private CinemachineCamera _vcam;
    private bool _targetFound = false;

    void Awake()
    {
        _vcam = GetComponent<CinemachineCamera>();
    }

    void LateUpdate()
    {
        // 如果還沒找到目標，或者目前的目標被毀了（例如切換關卡時）
        if (!_targetFound || _vcam.Follow == null)
        {
            FindPlayer();
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            _vcam.Follow = player.transform;
            _targetFound = true;
            Debug.Log($"相機已自動鎖定標籤為 Player 的物件: {player.name}");
        }
    }
}
