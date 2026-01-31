# VR Motion Evaluation System

## 概要

本プロジェクトは、  
**Meta Quest 3 + VIVE Ultimate Tracker** を用いた  
**全身動作の記録・即時リプレイ・評価を行うVR動作学習支援システム**です。

ユーザの実動作を取得し、  
体格差を考慮したキャリブレーションを行った上で、  
お手本動作との差分を **視覚的・数値的にフィードバック** することを目的としています。

本リポジトリは **卒業研究用途** を前提とし、  
再現性・説明可能性・拡張性を重視した構成になっています。

---

## 使用環境

### ハードウェア

- HMD：Meta Quest 3（PC VR / Quest Link）
- トラッカー：VIVE Ultimate Tracker × 3
  - Waist / LeftFoot / RightFoot
- コントローラ：Quest Controller（両手）
- PC：Windows（SteamVR動作可能環境）

### ソフトウェア

- Unity：**2022.3 LTS**
- Input System：Unity Input System
- XR：OpenXR + SteamVR Runtime
- IK：RootMotion FinalIK（VRIK）
- Version Control：Git / GitHub

---

## プロジェクト構成

```text
Assets/
└─ _Project/
   ├─ Scripts/
   │  ├─ Trackers/      # トラッカー入力
   │  ├─ IK/            # VRIK / Binder
   │  ├─ Calibration/   # Tポーズキャリブレーション
   │  ├─ UI/            # 案内UI / HUD
   │  └─ Core/          # 将来のStateMachine等
   ├─ Scenes/
   │  └─ _Dev/
   │     └─ 00_TrackerDebug.unity
   ├─ Materials/
   ├─ Prefabs/
   └─ Settings/
Docs/
```


---

## 最短起動手順（開発者向け）
※FinalIKがインポートされていることが前提

1. SteamVR を起動  
2. Ultimate Tracker を SteamVR 上で以下に割り当て  
   - Waist  
   - LeftFoot  
   - RightFoot  
3. Quest Link で Meta Quest 3 を PC に接続  
4. Unity で `00_TrackerDebug.unity` を開く  
5. Play  
6. トラッカー・HMD・コントローラが追従していれば成功  

---

## 現在実装済みの機能

- トラッカー位置・回転取得（Input System / OpenXR）
- 6点（Head / Hands / Waist / Feet）の Transform 入力
- VRIK ターゲットへのリアルタイム追従
- Tポーズによる身長・腕・脚キャリブレーション
- キャリブレーション結果の即時反映

---

## 設計方針

- **責務分離**
  - 入力 / IK / キャリブレーション / UI を分離
- Inspector 依存を最小化
- AI エージェントによる拡張を前提
- Samples に依存しない（自前コードのみ使用）

---

## ドキュメント構成

詳細設計・手順は以下を参照してください。

- `Docs/SystemOverview.md`
- `Docs/EnvironmentSetup.md`
- `Docs/SceneSetup.md`
- `Docs/InputActions.md`
- `Docs/Calibration.md`
- `Docs/Conventions.md`

---

## 注意事項

- `Library/` や 実験データ（json）は Git 管理対象外
- OpenXR Runtime は **SteamVR** を前提
- サンプルコードを使用する場合は `_Project` 配下にコピーして管理する


