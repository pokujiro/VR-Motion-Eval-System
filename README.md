# VR Motion Evaluation System

## 概要

本プロジェクトは、

**VR環境（Meta Quest 3 + VIVE Ultimate Tracker）** を用いて
ユーザの全身動作を取得・再生・評価し、
お手本動作との差分を **視覚的・数値的にフィードバックする動作学習支援システム** です。

卒業研究用途を想定し、以下を重視して設計しています。

* 実機環境での再現性
* 体格差を考慮したキャリブレーション
* リアルタイムでのリプレイ・評価
* 後から修正・拡張しやすい構造

---

## 使用環境

### ハードウェア

* HMD：Meta Quest 3（PC VR / SteamVR）

  * Head / LeftHand / RightHand
* トラッカー：VIVE Ultimate Tracker ×3

  * Waist / LeftFoot / RightFoot
* PC：Windows（GPU必須）

### ソフトウェア

* Unity：2022.3 LTS
* XR：OpenXR（Runtime：SteamVR）
* Input：Unity Input System
* IK：RootMotion FinalIK（VRIK）
* Version管理：Git / GitHub

---

## 最短起動手順（開発者向け）
※FinalIKをインポートしている必要がある
1. SteamVR を起動
2. Ultimate Tracker を SteamVR 上で以下に割り当て
   * Waist
   * LeftFoot
   * RightFoot
3. Quest Link を用いて Quest 3 を PC に接続
4. Unity で以下の Scene を開く

```
Assets/Scenes/_Dev/01_Playground.unity
```

5. Play を押す
6. トラッカー・HMD・コントローラが追従していれば成功

---

## 現在実装済みの機能

* トラッカー位置・回転取得（Input System / OpenXR）
* 6点（Head / Hands / Waist / Feet）の Transform 入力
* VRIK ターゲットへのリアルタイム追従
* Tポーズによる身長・腕・脚キャリブレーション
* キャリブレーション結果の即時反映

---

## シーン構成（概要）

```
01_Playground
├─ SYSTEM
│  └─ SystemRoot
│     └─ CalibrationService
├─ XR
│  ├─ XR Origin (XR Rig)
│  ├─ XR Interaction Manager
│  └─ Input Action Manager
├─ TRACKERS
│  └─ UltimateTrackerInputs
├─ IK
│  ├─ VrikBinder
│  └─ RigTargets
├─ UI
│  └─ BoardRoot
└─ AVATARS
   └─ LearnerAvatar
```

詳細は `Docs/SceneSetup.md` を参照してください。

---

## 設計方針

* **責務分離**

  * 入力 / IK / キャリブレーション / UI を分離
* **Inspector 依存を最小化**

  * 設定は SystemRoot に集約
* **再現性重視**

  * 実機前提の初期検証とデバッグ表示
* **Samples に依存しない**

  * 使用する場合は Project 配下にコピーして管理

---

## ドキュメント

詳細な設計・手順は以下を参照してください。

* `Docs/SystemOverview.md`
* `Docs/EnvironmentSetup.md`
* `Docs/SceneSetup.md`
* `Docs/InputActions.md`
* `Docs/Calibration.md`
* `Docs/Conventions.md`

---

## 注意事項

* `Library/` や `Temp/` は Git 管理対象外です
* 実験データ（json）は Git に含めません
* OpenXR Runtime は **SteamVR を前提**としています
* サンプルコードを利用する場合は `_Project` 配下へコピーしてください

---

## ライセンス

* 本リポジトリのスクリプトは研究・学習目的で使用してください
* アバター・外部アセットは各配布元のライセンスに従ってください

---

## 補足

本 README は **プロジェクト全体の入口**としての役割を持ち、
設計意図・実装詳細は Docs 以下に分離しています。


どちらに進みますか？
