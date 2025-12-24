using UnityEngine;
using Core.Models;     // 引用核心資料
using Core.Interfaces;
using DG.Tweening;     // 需安裝 DOTween (若尚未安裝可暫時註解相關代碼)
using Core;            // 引用 GameConstants

namespace Unity.Controllers
{
    public class BlockController : MonoBehaviour
    {
        [Header("Debug Info")]
        [SerializeField] private BlockType _type;
        [SerializeField] private GridDirection _orientation;

        // 視覺相關元件
        private Renderer _renderer;
        private MaterialPropertyBlock _propBlock;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        // 持有邏輯實體的參考 (Core Layer)
        private IBlockEntity _model;

        private void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();
            _propBlock = new MaterialPropertyBlock();
        }

        /// <summary>
        /// 由 GameManager 或 LevelLoader 呼叫，進行初始化
        /// </summary>
        public void Initialize(IBlockEntity model)
        {
            _model = model;
            _type = model.Type;
            _orientation = model.Orientation;

            // 訂閱事件 (Observer Pattern)
            // 當邏輯層發生變化時，自動通知這裡更新畫面
            if (_model is BlockModel blockModel)
            {
                blockModel.OnStateChanged += UpdatePowerVisual;
                blockModel.OnPositionChanged += MoveToPosition;
            }

            // 初始化位置與旋轉
            UpdateTransformImmediate();
            UpdatePowerVisual(_model.CurrentState);
        }

        private void OnDestroy()
        {
            // 記得取消訂閱防止記憶體洩漏
            if (_model is BlockModel blockModel)
            {
                blockModel.OnStateChanged -= UpdatePowerVisual;
                blockModel.OnPositionChanged -= MoveToPosition;
            }
        }

        // --- 視覺表現邏輯 ---

        private void UpdatePowerVisual(SignalState state)
        {
            if (_renderer == null) return;

            bool isOn = state == SignalState.High;
            float intensity = isOn ? GameConstants.BLOOM_INTENSITY_ON : GameConstants.BLOOM_INTENSITY_OFF;
            Color baseColor = isOn ? Color.yellow : Color.gray; // 或是你想要的任何顏色

            // 使用 MaterialPropertyBlock 優化效能 (URP 推薦做法)
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_BaseColor", baseColor);
            _propBlock.SetColor(EmissionColor, baseColor * intensity);
            _renderer.SetPropertyBlock(_propBlock);
        }

        private void MoveToPosition(GridPoint newPos, float duration)
        {
            // 將 Grid 座標轉換為 Unity 世界座標 (假設 y=0)
            Vector3 targetWorldPos = new Vector3(newPos.X * GameConstants.CELL_SIZE, 0, newPos.Z * GameConstants.CELL_SIZE);

            // 使用 DOTween 播放動畫
            transform.DOMove(targetWorldPos, duration).SetEase(Ease.OutQuad);
        }

        private void UpdateTransformImmediate()
        {
            if (_model == null) return;

            // 設定位置
            transform.position = new Vector3(_model.Position.X * GameConstants.CELL_SIZE, 0, _model.Position.Z * GameConstants.CELL_SIZE);

            // 設定旋轉 (根據 GridDirection: North=0, East=1, South=2, West=3)
            // North 對應 Z+, 即 0度
            float rotationY = (int)_model.Orientation * 90f;
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
    }
}