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
