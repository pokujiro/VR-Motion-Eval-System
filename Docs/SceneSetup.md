# SceneSetup.md

（シーン構成・セットアップ手順）

---

## 1. 対象シーン

本システムの動作確認および開発は、以下のシーンを基準として行う。

* シーン名：`01_Playground.unity`
* 目的：

  * トラッカー入力の確認
  * IK（VRIK）による全身追従
  * キャリブレーション動作の検証
  * 今後の機能追加の基盤

本シーンは **「1シーンで全機能が確認できる最小構成」** とする。

---

## 2. ヒエラルキー全体構成

```
01_Playground
├─ Directional Light
│
├─ SYSTEM
│   ├─ SystemRoot
│   │   └─ CalibrationService
│   └─ DebugHUD
│
├─ XR
│   ├─ XR Interaction Manager
│   ├─ Input Action Manager
│   ├─ XR Origin (XR Rig)
│   └─ EventSystem
│
├─ TRACKERS
│   └─ UltimateTrackerInputs
│
├─ UI
│   └─ BoardRoot
│
├─ AVATARS
│   └─ LearnerAvatar
│
└─ IK
    ├─ VrikBinder
    └─ RigTargets
```

---

## 3. SYSTEM グループ

### 3.1 SystemRoot

**役割**

* システム全体の起点
* 状態管理（将来的に StateMachine を内包）
* 各 Service の参照元

**原則**

* Inspector 設定は SystemRoot に集約する
* 他のスクリプトは SystemRoot 経由で参照される

---

### 3.2 CalibrationService

**役割**

* Tポーズによるキャリブレーション処理
* 身長・腕長・脚長スケールの算出
* VRIK へのキャリブ結果反映

**設計意図**

* キャリブレーションは「一時的な処理」ではなく
  **明確な Service（責務）**として分離する
* 入力 Transform や IK 設定への直接依存を避け、
  将来的な拡張・置換を容易にする

---

### 3.3 DebugHUD

**役割**

* 実行中の状態確認用 HUD
* 現在の State / Tracker 接続状況 / キャリブ値の可視化

**方針**

* ログに依存しない
* 実機検証時に即座に状態を把握できることを優先する

---

## 4. XR グループ

### 4.1 XR Origin (XR Rig)

**役割**

* HMD およびコントローラのトラッキング
* Head / LeftHand / RightHand の入力 Transform 提供

**使用要素**

* Main Camera：Head 入力として使用
* Left Controller / Right Controller：手入力として使用

---

### 4.2 Input Action Manager

**役割**

* Input System の Action を有効化
* トラッカーおよびコントローラ入力を統合管理

---

## 5. TRACKERS グループ

### 5.1 UltimateTrackerInputs

**役割**

* VIVE Ultimate Tracker の入力受け口
* Waist / LeftFoot / RightFoot の Transform を管理

**設計方針**

* SteamVR 側で事前に役割（Waist / Foot 等）を割り当てる
* Unity 側では「役割が確定した Transform」として扱う
* トラッカー識別ロジックを Unity 内に持ち込まない

---

## 6. IK グループ

### 6.1 RigTargets

**役割**

* VRIK が参照するターゲット Transform 群

**含まれるターゲット**

* HeadTarget
* LeftHandTarget
* RightHandTarget
* WaistTarget
* LeftFootTarget
* RightFootTarget

**重要事項**

* RigTargets はアバターの子にしない
* ワールド空間で独立した Transform として管理する

---

### 6.2 VrikBinder

**役割**

* 入力 Transform → RigTarget への橋渡し
* 毎フレーム、入力の position / rotation をターゲットに反映

**責務分離**

* 入力取得：XR / TRACKERS
* IK 計算：VRIK
* 接続定義：VrikBinder

---

## 7. AVATARS グループ

### 7.1 LearnerAvatar

**構成**

* Humanoid アバター
* Animator コンポーネント
* VRIK コンポーネント

**役割**

* 学習者の動作を全身で再現
* キャリブレーション結果を反映した動作表示

---

## 8. UI グループ

### 8.1 BoardRoot

**役割**

* キャリブレーション時の説明表示
* 状態遷移時のガイド表示

**仕様**

* カメラ追従 or 固定位置表示を切替可能
* キャリブレーション中の操作指示を明示する

---

## 9. 設計上の注意点

* Scene 内で **役割が重複する GameObject を作らない**
* 「Input → Binder → IK → Avatar」の流れを崩さない
* 一時的なデバッグ用オブジェクトは `_Debug` プレフィックスを付与する

---

## 10. このシーンの位置づけ

* 本シーンは **開発・検証用の基準シーン**
* 実験用・評価用シーンは将来的に別途作成する
* AIによる機能追加・修正は、原則このシーンを基点に行う

---

### 補足

この SceneSetup.md は、
**「第三者」がScene を開いた瞬間に全体構造を理解できること**
を目的としている。
