using UnityEngine;
using UnityEngine.InputSystem; // [重點] 引用新版 Input System
using Core.Logic;
using Core.Models;
using Core.Interfaces;
using Core;

namespace Unity.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _startLevelId = 1;

        [Header("References")]
        [SerializeField] private LevelLoader _levelLoader;

        // 核心系統 (純 C#)
        private GridSystem _gridSystem;
        private CircuitSystem _circuitSystem;

        // 輸入鎖 (防止移動動畫中重複輸入)
        private bool _isMoving = false;
        private float _moveTimer = 0f;

        private void Awake()
        {
            // 1. 初始化核心系統 (Core)
            _gridSystem = new GridSystem(10, 10);
            _circuitSystem = new CircuitSystem(_gridSystem);

            if (_levelLoader == null)
                _levelLoader = GetComponent<LevelLoader>() ?? gameObject.AddComponent<LevelLoader>();
        }

        private void Start()
        {
            // 2. 載入關卡
            _levelLoader.LoadLevel(_startLevelId, _gridSystem);

            // 3. 初始電路計算
            _circuitSystem.Recalculate();
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            // 簡單的輸入冷卻處理
            if (_isMoving)
            {
                _moveTimer -= Time.deltaTime;
                if (_moveTimer <= 0) _isMoving = false;
                else return;
            }

            // [防呆] 如果沒有鍵盤連接，直接返回
            if (Keyboard.current == null) return;

            GridDirection? moveDir = null;

            // [修正重點] 改用 Keyboard.current.xxxKey.wasPressedThisFrame
            if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
                moveDir = GridDirection.North;
            else if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
                moveDir = GridDirection.South;
            else if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
                moveDir = GridDirection.West;
            else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
                moveDir = GridDirection.East;

            if (moveDir.HasValue)
            {
                // 4. 呼叫核心移動邏輯
                bool success = _gridSystem.MovePlayer(moveDir.Value);

                if (success)
                {
                    // 5. 如果移動成功，重新計算電路
                    _circuitSystem.Recalculate();

                    _isMoving = true;
                    _moveTimer = GameConstants.MOVE_DURATION;
                }
            }
        }
    }
}