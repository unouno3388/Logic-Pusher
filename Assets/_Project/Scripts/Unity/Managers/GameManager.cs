using UnityEngine;
using UnityEngine.InputSystem; // [重點] 引用新版 Input System
using Core.Logic;
using Core.Models;
using Core.Interfaces;
using Core;
using UI.Base;
using System;
//using Unity.Cinemachine;

namespace Unity.Managers
{
    public class GameManager : MonoBehaviour
    {
        // 單例模式
        public static GameManager Instance { get; private set; }

        //[Header("Camera Settings")]
        //[SerializeField] private CinemachineCamera _vcam;

        // 設定
        [Header("Settings")]
        [SerializeField] private int _startLevelId = 1;

        [Header("References")]
        [SerializeField] private LevelLoader _levelLoader;

        // 核心系統 (純 C#)
        private GridSystem _gridSystem;
        private ICircuitSystem _circuitSystem;

        // 輸入鎖 (防止移動動畫中重複輸入)
        private bool _isMoving = false;
        private float _moveTimer = 0f;

        //事件系統
        public event Action OnLevelCompleted;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // 關鍵：切換場景時不要銷毀這個物件
                DontDestroyOnLoad(gameObject);

                // 初始化核心系統
                _gridSystem = new GridSystem(10, 10);
                _circuitSystem = new CircuitSystem(_gridSystem);

                if (_levelLoader == null)
                    _levelLoader = GetComponent<LevelLoader>() ?? gameObject.AddComponent<LevelLoader>();
            }
            else
            {
                // 如果場景中已經有一個 GameManager，就刪除重複的
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // 2. 載入關卡
            //_levelLoader.LoadLevel(_startLevelId, _gridSystem);
            //_levelLoader.LoadLevelFromSO(_startLevelId, _gridSystem);
            //// 3. 初始電路計算
            //_circuitSystem.Recalculate();
        }

        private void Update()
        {
            if(Time.timeScale != 0f)
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            if (_circuitSystem.IsLevelWin()) return;
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
            else if (Keyboard.current.rKey.wasPressedThisFrame)
                ResetLevel();

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

            if (_circuitSystem.IsLevelWin()) 
            {
                //Debug.Log("Level Win!");
                //Debug.Log("Time Scale (OnLevelCompleted前): " + Time.timeScale);
                OnLevelCompleted?.Invoke();
                //Debug.Log("Time Scale (OnLevelCompleted前): " + Time.timeScale);
            }
        }
        private void ResetLevel() 
        {
            _gridSystem.ResetLevel();
            DestroyAllChildren();
            _circuitSystem.ResetWinFlag();
            //_levelLoader.LoadLevel(_startLevelId, _gridSystem);
            _levelLoader.LoadLevelFromSO(_startLevelId, _gridSystem);
            // 3. 初始電路計算
            _circuitSystem.Recalculate();
        }
        public void NextLevel() 
        {
            _gridSystem.ResetLevel();
            DestroyAllChildren();
            _circuitSystem.ResetWinFlag();

            //_levelLoader.LoadLevel(_startLevelId, _gridSystem);
            //_startLevelId ++;
            _levelLoader.LoadLevelFromSO(++_startLevelId, _gridSystem);
            // 3. 初始電路計算
            _circuitSystem.Recalculate();
        }
        public void SelectLevel(int _startLevelId) 
        {
            _gridSystem.ResetLevel();
            DestroyAllChildren();
            _circuitSystem.ResetWinFlag();

            //_levelLoader.LoadLevel(_startLevelId, _gridSystem);
            _levelLoader.LoadLevelFromSO(_startLevelId, _gridSystem);
            // 3. 初始電路計算
            _circuitSystem.Recalculate();
        }

        private void OnDestroy()
        {
            // 清理單例引用
            if (Instance == this)
                Instance = null;
        }
        public void DestroyAllChildren()
        {
            // 遍歷此腳本掛載物件的所有子物件
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }


}