# TicTocGuardians

박스를 밀어 이동시키는 퍼즐 요소와 녹화/재생(TV) 기믹을 결합한 3D 퍼즐 플랫포머입니다.

**개발 기간**: 2024.05 ~ 2024.09

## 사용 엔진

- Unity (C#, URP)

## 핵심 기술

- 박스 푸시 기반 레벨 오브젝트/퍼즐 시스템
- 녹화 후 재생되는 TV 화면 기믹 (RenderTexture + 커스텀 TV 셰이더)
- 자체 제작 레벨 에디터
- 스테이지 난이도(단계별 타이머/Fog 설정)별 레벨 구성
- Outline 렌더링을 활용한 상호작용 오브젝트 강조

## 기여 개요

2인 팀 프로젝트로 참여하여 레벨 오브젝트/매니저/플레이어 로직, 레벨 에디터, 녹화-재생 기믹 및 TV 셰이더, UI(스테이지 설명/시놉시스/크레딧) 구현을 담당했습니다.

## 프로젝트 구조

```
Assets/TicTocGuardians/
├─ Animation, Animator, CustomAssets, Materials, Prefabs, RenderTextures, Scenes, URP
└─ Scripts/
   ├─ Assets/
   ├─ Game/
   │  ├─ LevelObjects/   # 박스 등 레벨 오브젝트
   │  ├─ Manager/
   │  ├─ Player/
   │  └─ UI/
   ├─ Interface/
   └─ LevelEditor/       # 자체 제작 레벨 에디터
```
