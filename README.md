### 개발 환경

- Unity 2022.3.37f1 / Visual Studio 2019

### 개발 언어 및 라이브러리

- C# / DOTween / Spine / PlayFab / Google Login

### 담당 업무

공통

- 캐릭터, 컬렉션, 파티 등 아웃게임 UI 리팩토링
- UnityEngine.Pool을 활용한 이펙트, 팝업 UI 시스템 구축
- 유저의 정령 세팅과 스테이지에 등장하는 기믹에 따라 필요한 이펙트만 불러오는 이펙트 로더 구축
- 커스텀 에디터를 활용해 다양한 연출을 옵션에 따라 연출하고, 순서 변경이 가능한 연출 툴 구현
- 게임 진행도에 따라 Addressables를 활용한 연출 이벤트 동적 로드 구현

모바일 환경

- Google AdMob, PlayFab과 연동해 서버에 광고 시청 데이터를 갱신하는 광고 시스템 구축
- 구글, PlayFab 로그인 시 계정 전환, 중복 로그인 방지 기능 구현 (빌드 환경 한정)

PC 환경

- Scriptable Object와 커스텀 에디터를 통해 JSON Importer 구현
⇒ PlayFab 서버 내 데이터를 클라이언트 내부로 이식
- 서버 내 CloudScript 로직 (아이템 강화, 보상 처리 등) 클라이언트 내부로 이식
