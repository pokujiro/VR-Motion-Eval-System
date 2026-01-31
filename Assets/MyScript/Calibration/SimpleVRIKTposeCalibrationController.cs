using UnityEngine;
using RootMotion.FinalIK;
using UnityEngine.InputSystem;

/// <summary>
/// T-Pose前提でVRIKのスケール・四肢長をキャリブレーションするコントローラ
/// ・入力Transformは VrikTrackerBinder から取得する
/// ・1回のみ実行（再実行防止）
/// ・右手コントローラボタンなどでトリガー
/// </summary>
public class SimpleVRIKTposeCalibrationController : MonoBehaviour
{
    // ==============================
    // 参照オブジェクト
    // ==============================
    [Header("References")]
    [Tooltip("キャリブレーション対象のVRIK")]
    [SerializeField] private VRIK vrik;

    [Tooltip("入力Transformを一元管理するBinder")]
    [SerializeField] private VrikTrackerBinder binder;

    // ==============================
    // 入力（キャリブレーショントリガー）
    // ==============================
    [Header("Calibration Trigger")]
    [Tooltip("キャリブレーションを開始するInputAction（右手ボタンなど）")]
    [SerializeField] private InputActionReference calibrateAction;

    // ==============================
    // UI表示
    // ==============================
    [Header("UI Board")]
    [Tooltip("Tポーズ指示用のUIボード")]
    [SerializeField] private GameObject calibrationBoard;

    [Tooltip("UIボードを表示する秒数")]
    [SerializeField] private float boardDisplayTime = 3.0f;

    // ==============================
    // デバッグ設定
    // ==============================
    [Header("Debug")]
    [SerializeField] private bool logDebug = true;

    // ==============================
    // 内部状態
    // ==============================
    private bool isCalibrated = false;

    // ==============================
    // Binder経由で入力Transformを取得
    // ==============================
    Transform Head => binder.headInput;
    Transform Pelvis => binder.waistInput;
    Transform LeftHand => binder.lefthandInput;
    Transform RightHand => binder.righthandInput;
    Transform LeftFoot => binder.leftfootInput;
    Transform RightFoot => binder.rightfootInput;

    // ==============================
    // InputAction 登録
    // ==============================
    private void OnEnable()
    {
        if (calibrateAction != null)
        {
            calibrateAction.action.performed += OnCalibrateTriggered;
            if (logDebug) Debug.Log("🟢 Calibration Action Enabled");
        }
    }

    private void OnDisable()
    {
        if (calibrateAction != null)
        {
            calibrateAction.action.performed -= OnCalibrateTriggered;
            if (logDebug) Debug.Log("🔴 Calibration Action Disabled");
        }
    }

    /// <summary>
    /// キャリブレーショントリガーが押された時
    /// </summary>
    private void OnCalibrateTriggered(InputAction.CallbackContext ctx)
    {
        if (logDebug)
            Debug.Log("🎮 Calibration Trigger Pressed");

        if (isCalibrated)
        {
            Debug.LogWarning("⚠ 既にキャリブレーション済みのため処理をスキップしました");
            return;
        }

        StartCalibration();
    }

    // ==============================
    // メイン処理
    // ==============================
    /// <summary>
    /// Tポーズ前提のシンプルキャリブレーション処理
    /// </summary>
    public void StartCalibration()
    {
        Debug.Log("🧍 T-Pose Calibration START");

        // --------- 事前チェック ---------
        if (vrik == null)
        {
            Debug.LogError("❌ VRIK が割り当てられていません");
            return;
        }

        if (binder == null)
        {
            Debug.LogError("❌ VrikTrackerBinder が割り当てられていません");
            return;
        }

        if (Head == null || Pelvis == null)
        {
            Debug.LogError("❌ Head または Pelvis の入力Transformがありません");
            return;
        }

        // UI表示
        ShowBoard(true);

        // ==============================
        // ① 身長スケール補正
        // ==============================
        Debug.Log("📏 Step1: Height Calibration");

        float avatarHeight =
            vrik.references.head.position.y -
            vrik.references.root.position.y;

        float userHeight =
            Head.position.y -
            Pelvis.position.y;

        float heightScale = userHeight / avatarHeight;
        vrik.references.root.localScale = Vector3.one * heightScale;

        Debug.Log($"✔ HeightScale = {heightScale:F3}");

        // ==============================
        // ② 腕の長さ補正（Tポーズ前提）
        // ==============================
        Debug.Log("🦾 Step2: Arm Length Calibration");

        float avatarArmSpan =
            Vector3.Distance(vrik.references.leftHand.position,
                             vrik.references.rightHand.position);

        float userArmSpan =
            Vector3.Distance(LeftHand.position,
                             RightHand.position);

        float armScale = userArmSpan / avatarArmSpan;

        vrik.solver.leftArm.armLengthMlp = armScale;
        vrik.solver.rightArm.armLengthMlp = armScale;

        Debug.Log($"✔ ArmScale = {armScale:F3}");

        // ==============================
        // ③ 脚の長さ補正
        // ==============================
        Debug.Log("🦵 Step3: Leg Length Calibration");

        ApplyLegScale(vrik.solver.leftLeg, LeftFoot, "LeftLeg");
        ApplyLegScale(vrik.solver.rightLeg, RightFoot, "RightLeg");

        // ==============================
        // ④ VRIK更新
        // ==============================
        Debug.Log("🔧 Step4: Apply VRIK Solver");

        vrik.solver.FixTransforms();
        vrik.solver.Initiate(vrik.references.root);

        isCalibrated = true;

        Debug.Log("✅ T-Pose Calibration COMPLETED");

        // UI非表示
        Invoke(nameof(HideBoard), boardDisplayTime);
    }

    /// <summary>
    /// 脚の長さを補正する（Pelvis基準）
    /// </summary>
    private void ApplyLegScale(IKSolverVR.Leg legSolver, Transform foot, string label)
    {
        if (foot == null)
        {
            Debug.LogWarning($"⚠ {label} Foot Transform is null");
            return;
        }

        float avatarLegLength =
            Mathf.Abs(vrik.references.pelvis.position.y -
                      vrik.references.leftFoot.position.y);

        float userLegLength =
            Mathf.Abs(Pelvis.position.y -
                      foot.position.y);

        float legScale = userLegLength / avatarLegLength;
        legSolver.legLengthMlp = legScale;

        Debug.Log($"✔ {label} Scale = {legScale:F3}");
    }

    // ==============================
    // UI制御
    // ==============================
    private void ShowBoard(bool visible)
    {
        if (calibrationBoard != null)
        {
            calibrationBoard.SetActive(visible);
            if (logDebug)
                Debug.Log($"🪧 Calibration Board {(visible ? "Shown" : "Hidden")}");
        }
    }

    private void HideBoard()
    {
        ShowBoard(false);
    }
}
