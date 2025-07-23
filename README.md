# Burger Please 모작
<a name="readme-top"></a>

<p>
  Burger Please 모작 (캐쥬얼 경영 시뮬레이션 게임)
</p>

<img width="373" height="769" alt="Image" src="https://github.com/user-attachments/assets/b337f894-426c-418f-8ab5-6d792439716a"/>
<br/>

<!-- TABLE OF CONTENTS -->

## 목차

1. [프로젝트 개요](#Intro)
2. [게임 플레이](#Play)
3. [핵심 기능](#CoreFeatures)
<br/>

<a name="Intro"></a>
## 프로젝트 개요
- 프로젝트 기간 : 2025.07 ~ 진행중 
- 개발 엔진 및 언어 : Unity6 & C#
- 플랫폼 : 모바일

<br/>


<a name="Play"></a>
## 게임 플레이
[게임 영상](https://youtu.be/wnCnzKtJKHQ)


<br/>

<a name="CoreFeatures"></a>
## 핵심 기능

1. 손님 AI 및 대기 시스템
    - **NavMesh 기반**의 AI 이동으로 손님이 자연스럽게 목적지까지 이동하고,
    - [큐 시스템](https://github.com/haaaabin/BurgerPlease/blob/7a2eabb1f2a88a12a570f1a8f02148c588c5965a/Assets/%40Scripts/Props/Unlockable/Counter.cs#L132-L155)을 활용해 계산대, 키오스크, 드라이브 스루 등에서 순서대로 대기하도록 구현하였습니다.
      
<br/> 

2. 직원 자동화 작업 시스템 리팩토링
    - 직원 AI 자동 작업 배분 로직을 드라이브스루 시스템과 키오스크 주문 시스템에 확장 적용할 수 있도록 공통 로직을 추상화한 베이스 클래스([WorkerSystemBase](https://github.com/haaaabin/BurgerPlease/blob/7a2eabb1f2a88a12a570f1a8f02148c588c5965a/Assets/%40Scripts/System/WorkerSystemBase.cs#L1-L32))로 분리하고, 각 시스템이 이를 상속받아 필요한 부분만 오버라이드하는 구조로 리팩토링하였습니다.
    - [작업(버거 운반, 테이블 청소, 계산 등)](https://github.com/haaaabin/BurgerPlease/blob/7a2eabb1f2a88a12a570f1a8f02148c588c5965a/Assets/%40Scripts/System/MainCounterSystem.cs#L37-L158)은 코루틴을 활용해 주기적으로 처리되며 우선순위와 조건에 따라 유연하게 배분됩니다.
  
<br/>

3. 자동화된 인터랙션 시스템
    - 게임 내 다양한 상호작용 대상(버거 머신, 카운터 등)과 직원 및 플레이어가 자동으로 상호작용할 수 있도록, 확장 가능한 인터랙션 시스템을 설계하였습니다.
    - 각 인터랙션 오브젝트에 [WorkerInteraction](https://github.com/haaaabin/BurgerPlease/blob/7a2eabb1f2a88a12a570f1a8f02148c588c5965a/Assets/%40Scripts/Props/Components/WorkerInteraction.cs#L6-L80) 컴포넌트를 부착하고 해당 오브젝트의 범위에 Worker(직원 또는 플레이어)가 진입하면, 내부 코루틴이 주기적으로 [OnInteraction 델리게이트를 호출](https://github.com/haaaabin/BurgerPlease/blob/7a2eabb1f2a88a12a570f1a8f02148c588c5965a/Assets/%40Scripts/Props/Unlockable/Counter.cs#L55-L68)하는 방식으로 동작합니다

<br/> 

4. 이벤트 기반 UI 갱신 시스템
    - 게임 내 다양한 시스템(직원 고용, 업그레이드 등)과 UI 간의 결합도를 낮추기 위해 이벤트 기반의 UI 시스템을 설계하였습니다.
    - [GameManager](https://github.com/haaaabin/BurgerPlease/blob/7a2eabb1f2a88a12a570f1a8f02148c588c5965a/Assets/%40Scripts/Manager/GameManager.cs#L171-L200) 에서 이벤트 타입별로 델리게이트 배열을 관리하며, 각 시스템과 UI는 필요한 이벤트를 자유롭게 구독/해제할 수 있습니다.
    - 이벤트가 발생하면, 해당 이벤트에 등록된 모든 콜백이 호출되어, UI와 게임 로직 사이의 결합도를 낮추고 유지보수가 용이합니다.
      
<br/> 
      

