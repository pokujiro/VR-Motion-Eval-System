# Calibration.md

## キャリブレーション仕様書

---

## 1. 目的

本システムにおけるキャリブレーションの目的は、**異なる体格を持つユーザ間でも公平に動作を比較・評価できる状態を作ること**である。

具体的には、

* ユーザの身長・腕長・脚長の違いを吸収する
* トラッカーの物理装着位置のばらつきを補正する
* IK（VRIK）が破綻しない基準姿勢を確定する

ことを目的とする。

---

## 2. キャリブレーションの前提条件

### 2.1 使用する入力点（6点）

キャリブレーションでは以下の **6点のTransform入力** を使用する。

| 部位 | 入力Transform          |
| -- | -------------------- |
| 頭  | HMD（Main Camera）     |
| 左手 | LeftHand Controller  |
| 右手 | RightHand Controller |
| 腰  | Waist Tracker        |
| 左足 | LeftFoot Tracker     |
| 右足 | RightFoot Tracker    |

これらは **VrikTrackerBinder により一元管理**されている。

---

### 2.2 キャリブレーション姿勢

* 姿勢：**Tポーズ**
* 条件：

  * 両腕を水平に広げる
  * 足は肩幅程度
  * 視線は正面
  * 腰・足トラッカーが安定していること

Tポーズは以下の理由で採用している。

* 腕長を左右対称に計測できる
* IKの基準姿勢として安定している
* 説明性・再現性が高い（論文向き）

---

## 3. キャリブレーションの実行タイミング

* **1回のみ実行（One-shot）**
* 右手コントローラのボタン入力で開始
* 実行後はパラメータを固定し、再計算しない

理由：

* 実行中にスケールが変わると IK が破綻しやすい
* 実験の再現性を確保するため

---

## 4. キャリブレーション処理の全体フロー

```
Start
 ↓
UI表示（Tポーズ指示）
 ↓
ユーザがTポーズを取る
 ↓
ボタン入力
 ↓
身長スケール計算
 ↓
腕長倍率計算
 ↓
脚長倍率計算
 ↓
VRIKへ反映
 ↓
FixTransforms
 ↓
完了UI表示
```

---

## 5. 計算内容（実装レベル）

### 5.1 身長スケール（Root Scale）

#### 概要

* **頭 – 腰のY距離** を基準に、アバター全体のスケールを補正する

#### 計算式

```
bodyScale = userHeadToPelvisY / avatarHeadToPelvisY
```

#### Unity実装概念

* user側：

  ```
  abs(headInput.y - pelvisInput.y)
  ```
* avatar側：

  ```
  abs(ik.references.head.y - ik.references.pelvis.y)
  ```

#### 適用先

```
ik.references.root.localScale = Vector3.one * bodyScale;
```

---

### 5.2 腕長補正（Arm Length Multiplier）

#### 概要

* **左右手の距離（Tポーズ時）** を用いて腕の長さを補正
* 左右同一倍率とする（安定性優先）

#### 計算式

```
armLengthMlp = userHandSpan / avatarHandSpan
```

#### 適用先

```
ik.solver.leftArm.armLengthMlp
ik.solver.rightArm.armLengthMlp
```

---

### 5.3 脚長補正（Leg Length Multiplier）

#### 概要

* 腰 – 足のY距離を用いて左右別に計算
* 足の装着位置誤差を考慮するため左右独立

#### 計算式

```
leftLegMlp  = userLeftLegY / avatarLeftLegY
rightLegMlp = userRightLegY / avatarRightLegY
```

#### 適用先

```
ik.solver.leftLeg.legLengthMlp
ik.solver.rightLeg.legLengthMlp
```

---

## 6. 使用スクリプト

### SimpleVRIKTposeCalibrationController.cs

#### 主な責務

* 入力Transformの取得
* キャリブレーション計算
* VRIKへの反映
* UIメッセージ制御
* デバッグログ出力

#### このスクリプトが **やらないこと**

* トラッカーの取得（Input責務）
* IKターゲットの追従（Binder責務）
* データ保存（将来Serviceで分離）

---

## 7. UI仕様（キャリブレーション中）

* 表示位置：ユーザ正面 約1m
* 内容：

  * Tポーズ指示
  * ボタン操作案内
  * 完了後の数値表示（Scale / Arm / Leg）

UIは以下2モードを切り替え可能に設計する。

| モード    | 説明     |
| ------ | ------ |
| Follow | カメラ追従  |
| Fixed  | ワールド固定 |

---

## 8. 設計上の判断理由（論文向け）

* リアルタイム補正ではなく **事前キャリブレーション** を採用
  → 実験再現性を優先
* 筋力・可動域などの個人差は考慮しない
  → 今後の課題として明示可能
* 腕のY補正・胸基準補正は将来拡張項目
  → 現段階では安定性を優先

---

## 9. デバッグ・検証項目

* キャリブ前後でアバターが破綻しない
* 腕・脚が不自然に伸びない
* スケールが極端な値にならない（0 / NaN防止）
* 再生・評価時にキャリブ値が固定されている

---

## 10. 今後の拡張候補

* 胸基準の腕Y補正
* キャリブレーション結果の保存 / 読み込み
* キャリブ失敗時の再試行UI
* 姿勢チェック（Tポーズ検出）

---

## まとめ

本キャリブレーションは、

* **安定性**
* **説明可能性**
* **卒論耐性**

を最優先に設計されており、
即時リプレイ・評価機能の基盤として機能する。


