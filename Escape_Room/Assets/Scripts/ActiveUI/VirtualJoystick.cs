using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements; // 키보드, 마우스, 터치를 이벤트로 오브젝트에 보낼 수 있는 기능을 지원

public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform stick;
    [SerializeField, Range(10f, 150f)]
    private float leverRange;
    Coroutine coroutine;
    Vector2 startPos;
    Vector2 clampedDir; // 조이스틱 최종 포지션 값

    private bool isInput;

    void OnEnable()
    {
        UIManager.Instance.dirLockInput.Clear(); // 오브젝트 활성화 시, 입력값 초기화
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        coroutine = StartCoroutine(ControlJoystickLever(eventData)); // 없으면 제대로 초기화가 안됨
        isInput = true;
        startPos = eventData.position;
    }

    // 오브젝트를 클릭해서 드래그 하는 도중에 들어오는 이벤트
    // 하지만 클릭을 유지한 상태로 마우스를 멈추면 이벤트가 들어오지 않음    
    public void OnDrag(PointerEventData eventData)
    {
        // 방향 세밀 조정을 위해 최소 이동 값 설정
        float xPos = Mathf.Abs(startPos.x) - Mathf.Abs(eventData.position.x);
        float yPos = Mathf.Abs(startPos.y) - Mathf.Abs(eventData.position.y);

        if(Mathf.Abs(xPos) > 150 || Mathf.Abs(yPos) > 150)
        {
            coroutine = StartCoroutine(ControlJoystickLever(eventData));
        }
    }

    IEnumerator ControlJoystickLever(PointerEventData eventData)
    {
        if (!isInput) {StopAllCoroutines();}

        yield return new WaitForSeconds(0.2f);

        var inputDir = eventData.position - new Vector2(960, 390);
        clampedDir = inputDir.normalized * leverRange;

        // 방향 세밀 조정
        if(Mathf.Abs(inputDir.x) > Mathf.Abs(inputDir.y)) {  clampedDir.y = 0; }
        else if (Mathf.Abs(inputDir.x) < Mathf.Abs(inputDir.y)) { clampedDir.x = 0; }

        // 방향 적용
        stick.anchoredPosition = clampedDir;

        isInput = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(SetOrigin(eventData));
    }

    IEnumerator SetOrigin(PointerEventData eventData)
    {
        yield return new WaitForSeconds(0.2f);

        if (clampedDir.x > 35) { UIManager.Instance.dirLockInput.Add("Right"); }
        else if (clampedDir.x < -35) { UIManager.Instance.dirLockInput.Add("Left"); }
        else if (clampedDir.y > 35) { UIManager.Instance.dirLockInput.Add("Up"); }
        else if (clampedDir.y < -35) { UIManager.Instance.dirLockInput.Add("Down"); }

        // SFX Sound
        AudioManager.Instance.SFX(0);
        Debug.Log("Direction Button");

        // 원위치
        stick.anchoredPosition = Vector2.zero;
    }
}
