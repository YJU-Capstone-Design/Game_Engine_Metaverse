using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Narration : MonoBehaviour
{
    public PhotonManager photonManager;

    public string bed;           // 침대
    public string deadBody;      // 시체 
    public string chair;         // 의자
    public string laptop;        // 노트북
    public string document;      // 서류
    public string playerBag;     // 플레이어 가방
    public string deadBodyBag;   // 피해자 가방
    public string wallet;        // 지갑
    public string IDcard;        // 신분증
    public string wallClock;     // 벽걸이 시계
    public string kitchenKnife;  // 식칼
    public string WallTV;        // 벽걸이 TV
    public string keyLock;       // 열쇠 자물쇠
    public string directionLock; // 방향 자물쇠
    public string dialLock;      // 번호 자물쇠
    public string buttonLock;    // 버튼 자물쇠
    public string key;           // 열쇠
    public string storageCloset; // 수납장
    public string hint; // 힌트
    public string hintZero; // 힌트를 다 사용
    public string remote; // 리모컨
    public string livingroomTV; // 거실 TV
    public string whiteBoard; // 화이트보드
    public string doorLock; // 도어락
    public string refrigerator; // 냉장고


    private void Awake()
    {
        bed = "피해자가 쓰러져 있는 침대다. 딱히 단서는 없는 거 같다.";
        deadBody = "죽어 있는 시체다 단서는 딱히 없는 것 같다.";
        chair = "낡고 헤진 의자다. 딱히 특별한 점은 없는 거 같다.";
        laptop = "노트북에 비밀번호가 걸려 있다. 안을 열어보기 위해서는 패스워드가 필요하다.\n노트북의 키패드 방향키의 위쪽 오른쪽 아래쪽 순으로 잉크가 희미하다.";
        document = "3.25이라는 숫자에 빨간펜으로 집착적으로 동그라미가 쳐져 있다. 중요한 날 같다.";
        playerBag = "샤프, 펜, 지우개, 자, 가위 등 평범한 학용품이 들어있다.\n학용품으로 자물쇠 하나를 열 수 있을 것 같다.";
        deadBodyBag = "피해자의 가방입니다. 가방을 살펴보시겠습니까?\n가방을 살펴보고 싶으시면 Enter를 눌러주십시오...";
        wallet = "피해자의 가방 속에서 지갑을 발견했습니다.\n자세히 확인하고 싶으시면 Enter를 눌러주십시오...";
        IDcard = "신분증을 보니 피해자는 25살인 거 같다. 생일은 1999.03.25이다.";
        wallClock = "건전지가 다 닳은 거 같다. 시각은 7:32을 가리키고 있다.";
        kitchenKnife = "피가 묻은 식칼이다. 중요한 증거품이 될 거 같다.";
        WallTV = "복현동에서 일어나는 연쇄살인 사건을 보도 하고 있다.\n지금까지 피해자는 27명인 거 같다.";
        keyLock = "열쇠가 필요한 자물쇠다.\n열쇠를 사용하시길 원하시면 Enter를 눌러주십시오...";
        directionLock = "방향형 자물쇠다.\n입력을 원하시면 Enter를 눌러주십시오...";
        dialLock = "3가지 숫자를 입력하는 번호 자물쇠다.\n입력을 원하시면 Enter를 눌러주십시오...";
        buttonLock = "4가지 숫자를 입력하는 버튼 자물쇠다.\n입력을 원하시면 Enter를 눌러주십시오...";
        key = "이걸로 열쇠형 자물쇠를 열 수 있을 거 같다.";
        storageCloset = "방향 자물쇠의 힌트가 적혀있는 쪽지가 있다.\nAA 건전지 3개를 구매한 영수증이다.";
        hintZero = "사용 가능한 힌트 횟수를 다 사용하셨습니다.";
        remote = "눌러봤지만 아무런 반응이 없다.";
        livingroomTV = "전원은 들어오지만 화면에 반응은 없다.";
        whiteBoard = "피해자들의 사진인 것 같다. 모든 사진에는 X 표시가 되어있다. \n자세히 확인하고 싶으시면 Enter를 눌러주십시오...";
        doorLock = "비밀번호를 입력하면 탈출할 수 있을 거 같다.\n입력을 원하시면 Enter를 눌러주십시오...";
    }

    private void Update()
    {
        hint = $"현재 힌트 사용 가능 횟수는 총 {photonManager.hintCount} 번 남았습니다. \n사용을 원하시면 Enter를 눌러주십시오...";
    }
}
