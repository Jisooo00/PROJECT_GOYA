using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRawimgScroll : MonoBehaviour
{
    [SerializeField] private RawImage _img; // Inspector 창에서 설정할 수 있는 RawImage 컴포넌트
    [SerializeField] private float _x, _y; // 이미지가 x축과 y축으로 움직이는 속도

    // Update is called once per frame
    void Update()
    {
        // 프레임 간 이동량을 계산하여 일정한 속도로 이미지가 움직이도록 하기
        _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_x,_y ) * Time.deltaTime, _img.uvRect.size);
    }
}
