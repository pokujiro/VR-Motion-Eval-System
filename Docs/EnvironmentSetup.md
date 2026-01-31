# EnvironmentSetup.md
## 開発・実行環境セットアップ手順書（PCVR / SteamVR / OpenXR）

---

## 1. 目的

本ドキュメントは、本プロジェクトを **他のPC環境でも再現可能に動作させる** ために、

- 使用するランタイム
- 必須ソフトウェア
- Unity側の設定
- 起動前に人間が行うべき準備

を明確に記述することを目的とする。

---

## 2. 前提構成（固定）

### 2.1 ハードウェア

- HMD：Meta Quest 3
- トラッカー：VIVE Ultimate Tracker × 3
  - Waist
  - LeftFoot
  - RightFoot
- PC：Windows（GPU必須）

---

### 2.2 ソフトウェア

| 種別 | 使用 |
|---|---|
| Unity | 2022.3 LTS |
| XR Runtime | SteamVR（OpenXR） |
| VR接続 | Quest Link（有線 or Air Link） |
| Input | Unity Input System |
| IK | RootMotion FinalIK（VRIK） |

---

## 3. ランタイム方針（重要）

### 3.1 OpenXR Runtime は **SteamVR に固定**

本プロジェクトでは、以下を前提とする。

- OpenXR Runtime：**SteamVR**
- Meta OpenXR Runtime は使用しない

#### 確認方法

1. SteamVR を起動
2. 設定 → OpenXR
3. 「SteamVR が現在の OpenXR Runtime です」と表示されていることを確認

---

## 4. SteamVR 側の設定

### 4.1 起動前の必須手順

1. SteamVR を起動
2. Ultimate Tracker をすべて認識させる
3. トラッカー役割を割り当てる

#### 割り当て例

| Tracker | Role |
|---|---|
| Tracker 1 | Waist |
| Tracker 2 | Left Foot |
| Tracker 3 | Right Foot |

※ **Unity起動前に必ず行うこと**

---

### 4.2 注意点

- トラッカーの役割は SteamVR 起動中に変更可能
- 本プロジェクトでは「起動前に役割固定」を運用ルールとする
- Unity側では serial / usage 固定は行っていない

---

## 5. Quest Link 設定

### 5.1 接続方法

- Quest Link（有線 USB-C / USB3 推奨）
- または Air Link（安定したWi-Fi環境が必要）

### 5.2 推奨

- **USB3 有線接続**
  - フレーム落ち・フリーズが起きにくい
  - キャリブレーション時の安定性が高い

### 5.3 接続確認

- Quest 3 側で「PCに接続済み」と表示
- PC画面がHMDに表示されていること

---

## 6. Unity プロジェクト設定

### 6.1 XR Plugin Management

#### PC（Standalone）

- OpenXR：有効
- Oculus / Meta XR：使用しない

---

### 6.2 OpenXR Feature

有効化を確認：

- ✔ OpenXR Interaction Profiles
- ✔ HTC Vive Tracker OpenXR（使用する場合）

※ 失敗例  
`XR_ERROR_PATH_UNSUPPORTED` が出る場合、  
ランタイム（SteamVR）が正しく設定されていない可能性が高い。

---

### 6.3 Input System

- Input System Package を使用
- `Player Settings > Active Input Handling`
  - **Input System Package (New)** または **Both**

---

## 7. プロジェクト起動手順（最短）

### 手順

1. SteamVR 起動
2. Ultimate Tracker 認識・役割割当
3. Quest Link 接続
4. Unity 起動
5. シーン `00_TrackerDebug.unity` を開く
6. Play

### 成功条件

- HMD / 両手 / Waist / Feet がそれぞれ追従する
- Cube / Debug Object が正しく動く
- エラーがコンソールに出ていない

---

## 8. よくあるトラブルと原因

### 8.1 画面が固まる / フリーズする

| 原因候補 | 対処 |
|---|---|
| USB2 接続 | USB3 ケーブルを使用 |
| Runtime不一致 | SteamVRをOpenXRに設定 |
| Tracker未認識 | SteamVR再起動 |

---

### 8.2 XR_ERROR_PATH_UNSUPPORTED

- OpenXR Runtime が SteamVR でない
- HTC Vive Tracker OpenXR Feature が未有効

---

### 8.3 トラッカーが動かない

- SteamVR 側で役割未割当
- Unity起動後に割当変更した
- InputAction の Binding Path 不一致

---

## 9. 設計上の意図（卒論用補足）

- 実機依存の不安定要素を **起動手順と環境固定** で排除
- ランタイムを SteamVR/OpenXR に一本化することで再現性を確保
- Unity側では極力「取得したPoseを使うだけ」に留める

---

## 10. まとめ

- 実行前準備は **人間が必ず行う**
- Unity側は「正しく入力が来る前提」で設計
- 環境が崩れた場合は、本ドキュメントの手順を最初から確認する

---
