# SceneSetup.md  
## シーン構成およびセットアップ手順

---

## 1. 本ドキュメントの目的

本ドキュメントは，本プロジェクトにおける **実験用Unityシーンの構成** を明確化し，  
第三者が **同一条件でシーンを再構築・検証できること** を目的とする。

特に以下を重視する：

- シーン単体で動作確認が可能であること  
- トラッカー入力・IK・キャリブレーションの責務が明確であること  
- Inspector 設定の属人化を防ぐこと  

---

## 2. 使用シーン一覧

| シーン名 | 用途 |
|---|---|
| `00_TrackerDebug.unity` | トラッカー・HMD・コントローラの入力確認用 |
| `01_Playground.unity` | 実験・評価のメインシーン |

本ドキュメントでは **`01_Playground.unity`** を基準として説明する。

---

## 3. シーン全体構成（Hierarchy 概要）

```test
01_Playground
├─ SystemRoot
│ ├─ StateMachine
│ ├─ InputService
│ ├─ CalibrationService
│ ├─ TrackerRecordingService
│ └─ VisualizationService
│
├─ XR Origin (Action-based)
│ ├─ Camera Offset
│ │ └─ Main Camera (HMD)
│ ├─ LeftHand Controller
│ └─ RightHand Controller
│
├─ TrackerInputs
│ ├─ WaistTracker
│ ├─ LeftFootTracker
│ └─ RightFootTracker
│
├─ LearnerAvatar
│ ├─ Animator (Humanoid)
│ └─ VRIK
│
├─ RigTargets
│ ├─ HeadTarget
│ ├─ PelvisTarget
│ ├─ LeftHandTarget
│ ├─ RightHandTarget
│ ├─ LeftFootTarget
│ └─ RightFootTarget
│
└─ UI
└─ CalibrationBoard
```

---

## 4. 各オブジェクトの役割

### 4.1 SystemRoot

- システム全体の起点となるオブジェクト
- 各 Service（入力・キャリブレーション・記録・可視化など）を集約
- Inspector での設定は **原則ここに集約** する

---

### 4.2 XR Origin (Action-based)

- Meta Quest 3（PC VR / SteamVR）用の標準リグ
- 以下の Transform を入力として使用する：
  - Head：`Main Camera`
  - LeftHand：`LeftHand Controller`
  - RightHand：`RightHand Controller`

---

### 4.3 TrackerInputs

- VIVE Ultimate Tracker の Transform 入力をまとめた親オブジェクト
- 各トラッカーは Input System / OpenXR 経由で Pose を取得し，
  対応する GameObject に反映される

| オブジェクト | 対応部位 |
|---|---|
| WaistTracker | 腰 |
| LeftFootTracker | 左足 |
| RightFootTracker | 右足 |

---

### 4.4 LearnerAvatar

- 学習者を表す Humanoid アバター
- 必須コンポーネント：
  - `Animator`（Humanoid 設定済み）
  - `VRIK`（RootMotion FinalIK）

アバター自体は直接トラッカーを参照せず，  
**RigTargets を介して IK 駆動** される。

---

### 4.5 RigTargets

- IK 用のターゲット Transform 群
- 重要：**アバターの子オブジェクトにはしない**
  - ワールド座標系でトラッカー入力をそのまま反映するため

| Target | 対応入力 |
|---|---|
| HeadTarget | HMD |
| PelvisTarget | Waist Tracker |
| LeftHandTarget | Left Controller |
| RightHandTarget | Right Controller |
| LeftFootTarget | Left Foot Tracker |
| RightFootTarget | Right Foot Tracker |

---

### 4.6 UI（CalibrationBoard）

- キャリブレーション時の指示・結果表示用UI
- カメラ追従／ワールド固定のどちらも選択可能
- キャリブレーション状態を明示的にユーザへ提示する目的で使用

---

## 5. トラッカー → IK のデータフロー
```text
[ Tracker / HMD / Controller Transform ]
↓
VrikTrackerBinder
↓
RigTargets (6点)
↓
VRIK
↓
Humanoid Avatar
```


- Binder は **入力TransformとIKターゲットの対応関係を一元管理** する
- キャリブレーション後も Binder は毎フレーム追従を行う

---

## 6. キャリブレーションとの関係

- Tポーズ時に以下を確定する：
  - 身長スケール
  - 腕長倍率
  - 脚長倍率
- キャリブレーション後は **RigTargets の追従のみで姿勢が再現** される
- 詳細は `Docs/Calibration.md` を参照

---

## 7. 設計上の注意点

- シーンは **単体で Play 可能** であること
- Inspector での参照切れが起きない構成にする
- トラッカー未接続時はエラー表示または実行停止することが望ましい

---

## 8. 今後の拡張予定

- お手本アバターの追加（半透明表示）
- 即時リプレイ用アバターの追加
- 評価結果の時系列可視化

これらは本シーン構成を崩さずに追加可能な設計となっている。

---




