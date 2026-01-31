# InputActions.md
## 入力（Input System / OpenXR）仕様書

---

## 1. 目的

本ドキュメントは、本システムで使用する入力（Input System）について、

- どのデバイスを前提とするか
- どのActionを用意するか
- どのGameObjectがそれを参照するか
- どうやって「6点トラッキング（Head/Hands/Waist/Feet）」のPoseを取得するか

を、**再現可能な形で整理**することを目的とする。

---

## 2. 前提（ランタイム / デバイス）

### 2.1 前提構成（PCVR）

- HMD：Meta Quest 3（Quest Link / PCVR）
- Runtime：SteamVR（OpenXR Runtime）
- Tracker：VIVE Ultimate Tracker（SteamVR上で役割割当）

この構成では、トラッカーのPoseは **OpenXR経由でInput Systemにデバイスとして見える**ことを前提とする。

---

## 3. InputActions資産の方針

### 3.1 InputActionAssetは「1つ」に統一する

プロジェクト内に InputActionAsset が複数存在すると、設定が分散しやすく、実験中の切り替えで事故が起きる。

そのため、最終的には以下の方針に統一する。

- `Assets/_Project/Settings/Input/ProjectInputActions.inputactions`
- 「XR Interaction Toolkitのデフォルト」や「VIVEサンプルのInputActions」から必要なActionMapだけ移植する
- Samplesフォルダ内のInputActionsを直接参照しない（Samplesは再生成/差し替えの可能性があるため）

---

## 4. ActionMap設計

本システムでは、入力を以下の2系統に分ける。

1. **操作系（UI/状態遷移）**
2. **トラッキング系（Pose取得）**

### 4.1 推奨ActionMap一覧

| ActionMap名 | 目的 |
|---|---|
| `XRI` | コントローラの基本入力（Trigger/Grip/Menu等） |
| `System` | システム操作（Calibrate/Start/Stop/Replay） |
| `Trackers` | Ultimate TrackerのPose取得（Waist/Feet等） |

---

## 5. システム操作（System ActionMap）

### 5.1 Action一覧（例）

| Action名 | Type | Control | 用途 |
|---|---:|---|---|
| `Calibrate` | Button | RightController（任意ボタン） | Tポーズキャリブ開始 |
| `StartRecording` | Button | 任意（将来） | 記録開始 |
| `StopRecording` | Button | 任意（将来） | 記録停止 |
| `Replay` | Button | 任意（将来） | 即時リプレイ |

※ 現時点で使用しているのは `Calibrate` のみ（SimpleVRIKTposeCalibrationControllerで参照）。

---

## 6. トラッキング系（Trackers ActionMap）

### 6.1 目的

Ultimate Trackerの姿勢（位置・回転）をInput Systemから取得し、Transformとして利用可能にする。

### 6.2 基本構造

Trackersでは、各トラッカーごとに以下を作る。

- `Pose`（位置＋回転）
- もしくは `Position` と `Rotation` を分ける

※過去に `Pose` 読み取りで型不一致エラーが出たため、現在は **Position/Rotation分離**を推奨する。

---

## 7. Pose取得の実装方針（推奨）

### 7.1 「Pose」をActionで直接読む場合の注意

Input Systemでは、`Pose`型で読むには **ControlもPose型である必要**がある。

過去に以下のエラーが発生した：

- `Pose`でReadValueしようとしたが、ControlがQuaternion（rotation）だった
- 例：
  - `/devicepose/rotation` は Quaternion
  - `/devicepose/position` は Vector3
  - 両方をまとめたActionを `Pose` として読むと例外になることがある

そのため、現仕様では以下の方式を推奨する。

### 7.2 推奨：Position/Rotationを分けて読む

#### Action例（1トラッカー）

- `LeftFoot_Position`（Value / Vector3）
- `LeftFoot_Rotation`（Value / Quaternion）

同様に Waist / RightFoot も作る。

---

## 8. 現在の実装（最低動作確認）

現状、以下の構成で動作確認ができている。

- Head：MainCamera（XR Origin内）
- Hands：XR Controller（Left/Right）
- Trackers（Waist/Feet）：`TrackerDebugPose` が InputActionReference からPoseを読み取り、オブジェクトのTransformに反映

---

## 9. コンポーネントと参照関係

### 9.1 TrackerDebugPose.cs

#### 役割
- InputActionReferenceから Vector3/Quaternion を読み取り
- 対象GameObjectの Transform を更新する

#### Inspectorで必要な項目
- PositionAction（Vector3）
- RotationAction（Quaternion）
- TargetTransform（通常は自分自身でOK）

---

### 9.2 VrikTrackerBinder.cs

#### 役割
- 入力Transform（HMD/Hands/Waist/Feet）を集約
- VRIKターゲット（HeadTarget等）に追従させる

#### Inspectorで必要な項目
- Input（6点）
- Targets（6点）

---

### 9.3 SimpleVRIKTposeCalibrationController.cs

#### 役割
- キャリブレーション入力（CalibrateAction）を受ける
- 6点のTransformからスケールを計算
- VRIKに反映

---

## 10. トラッカー割当（SteamVR側）について

### 10.1 現場運用

SteamVR側でトラッカー役割を割り当てた状態でUnityを起動する。

例：
- Waist
- LeftFoot
- RightFoot

### 10.2 注意点

- SteamVR上の割当が変わるとInput System側のデバイスパスが変わる可能性がある
- ただし現運用では「起動前に割当固定」を行うため、Unity側での追加識別（serial/usage固定など）は必須ではない

---

## 11. 起動時の確認手順（Input）

1. SteamVRを起動
2. Ultimate Trackerを認識させる
3. Trackerの役割を割り当て（Waist/Feet）
4. Quest Link接続 → VR画面が出る
5. Unityで `00_TrackerDebug.unity` を開く
6. Play
7. Waist/FeetのCubeが追従していればOK

---

## 12. 将来拡張（両腕トラッカーなど）

両腕（Ultimate Tracker追加）を行う場合：

- Trackers ActionMapに
  - `LeftHandTracker_Position`
  - `LeftHandTracker_Rotation`
  - `RightHandTracker_Position`
  - `RightHandTracker_Rotation`
  を追加する
- 追従用の `TrackerDebugPose` を追加する
- Binderの入力（leftHandInput/rightHandInput）を「コントローラ」から「トラッカー」に差し替えるだけで切替可能にする

---

## 13. まとめ

- InputActionsは最終的に **プロジェクト内1つに統一**
- トラッカーは **Position/Rotation分離が安全**
- SteamVR側で役割割当を固定して運用するため、Unity側の複雑な識別は当面不要
- 入力は「TrackerDebugPose → Binder → VRIK」の流れで利用する
