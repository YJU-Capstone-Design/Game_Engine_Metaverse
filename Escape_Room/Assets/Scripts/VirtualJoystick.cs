using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements; // Ű����, ���콺, ��ġ�� �̺�Ʈ�� ������Ʈ�� ���� �� �ִ� ����� ����

public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform stick;
    private RectTransform rectTransform;
    [SerializeField, Range(10f, 150f)]
    private float leverRange;
    Coroutine coroutine;
    Vector2 startPos;
    Vector2 clampedDir; // ���̽�ƽ ���� ������ ��

    private bool isInput;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        coroutine = StartCoroutine(ControlJoystickLever(eventData)); // ������ ����� �ʱ�ȭ�� �ȵ�
        isInput = true;
        startPos = eventData.position;
    }

    // ������Ʈ�� Ŭ���ؼ� �巡�� �ϴ� ���߿� ������ �̺�Ʈ
    // ������ Ŭ���� ������ ���·� ���콺�� ���߸� �̺�Ʈ�� ������ ����    
    public void OnDrag(PointerEventData eventData)
    {
        // ���� ���� ������ ���� �ּ� �̵� �� ����
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

        // ���� ���� ����
        if(Mathf.Abs(inputDir.x) > Mathf.Abs(inputDir.y)) {  clampedDir.y = 0; }
        else if (Mathf.Abs(inputDir.x) < Mathf.Abs(inputDir.y)) { clampedDir.x = 0; }

        // ���� ����
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

        // ���Ŀ� Debug.Log ��� �迭 �߰��ؼ� ���� Ȯ��
        if(clampedDir.x > 35) { Debug.Log("Right"); }
        else if(clampedDir.x < -35) { Debug.Log("Left"); }
        else if (clampedDir.y > 35) { Debug.Log("Up"); }
        else if (clampedDir.y < -35) { Debug.Log("Down"); }

        // ����ġ
        stick.anchoredPosition = Vector2.zero;
    }
}
