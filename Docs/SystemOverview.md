# SystemOverview.md  

---

## 1. システム概要

本プロジェクトは、  
**Meta Quest 3 + VIVE Ultimate Tracker** を用いた  
**全身動作学習支援システム**である。

VR空間内でユーザの動作を取得・再生・評価し、  
お手本動作との差分を **視覚的・数値的にフィードバック**することを目的とする。

本システムは、  
- 卒業論文発表に耐える再現性  
- 実機環境でのデバッグ容易性    

を重視して設計されている。

---

## 2. 想定利用環境

### ハードウェア

- HMD：Meta Quest 3（PC VR / Quest Link）
  - Head / LeftHand / RightHand
- トラッカー：VIVE Ultimate Tracker
  - Waist / LeftFoot / RightFoot
- PC：Windows（GPU搭載）

### ソフトウェア

- Unity：2022.3 LTS
- XR：OpenXR + SteamVR Runtime
- Input：Unity Input System
- IK：RootMotion FinalIK（VRIK）
- Version Control：Git（GitHub）

---

## 3. システム設計方針（重要）

### 3.1 責務分離

本システムは、以下の考え方に基づき構成される。

- **入力取得**
- **IK制御**
- **キャリブレーション**
- **記録・再生**
- **評価**
- **可視化**

を明確に分離し、  
1スクリプト1責務を原則とする。

---

### 3.2 Inspector依存の最小化

- Scene / Prefab での Inspector 設定は最小限に抑える
- 数値パラメータは ScriptableObject に集約
- 実行時状態はコード側で管理する

これにより、

- 環境差による破綻を防止
- AIエージェントによる自動修正を容易化
- 再現性の高い実験構成を維持

する。

---

### 3.3 設計

本プロジェクトは、

- Sceneは最小構成
- State Machine による進行管理
- Docs/ による設計の明文化

を必須とする。

---

## 4. システム構成（論理）

```text
SystemRoot
├─ StateMachine
├─ InputService
├─ CalibrationService
├─ TrackerRecordingService
├─ TrackerPlaybackService
├─ EvaluationService
├─ VisualizationService
├─ SaveLoadService
└─ DebugHUD
```

---

## 5. 状態遷移（State Machine）

```text
Idle
↓
Calibration
↓
Ready
↓
Recording
↓
ImmediateReplay
↓
Evaluation
↓
Ready / Idle
```

### 状態の役割

| State | 内容 |
|------|------|
| Idle | 初期待機 |
| Calibration | Tポーズによる体格補正 |
| Ready | 記録準備完了 |
| Recording | 動作記録 |
| ImmediateReplay | 直前動作の再生 |
| Evaluation | 誤差評価と可視化 |

---

## 6. 現在実装済み機能

- トラッカー位置・回転取得（OpenXR / Input System）
- 6点（Head / Hands / Waist / Feet）の Transform 入力
- VRIKターゲットへのリアルタイム追従
- Tポーズによる身長・腕・脚キャリブレーション
- キャリブレーション結果の即時反映

---

## 7. 可視化・デバッグ方針

- 実行中の状態は Debug HUD に常時表示
- ログに依存せず「見て分かる」構成を採用
- 評価重み・待ち時間・現在フレームなどを可視化

これにより、  
実機実験中のトラブルシュートと論文説明の両立を行う。

---

## 8. 本ドキュメントの位置づけ

本ファイルは、

- **システム全体の地図**
- **開発で迷わないための基準点**
- **論文執筆時の設計説明の下書き**

として機能する。

詳細は以下を参照：

- Docs/EnvironmentSetup.md  
- Docs/SceneSetup.md  
- Docs/InputActions.md  
- Docs/Calibration.md  
- Docs/Conventions.md  

---

