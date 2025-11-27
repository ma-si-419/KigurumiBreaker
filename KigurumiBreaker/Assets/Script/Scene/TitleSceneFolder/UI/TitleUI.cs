using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private GameObject _titleLogo;
    [SerializeField] private GameObject _titleButton;

    [SerializeField] private BreakBlock _titleBreak;

    [SerializeField] private float _size;
    [SerializeField] private float _speed;

    // Update is called once per frame
    void Update()
    {
        //â£Ç¡ÇΩÇÁ
        if(_titleBreak.breakMoment)
        {
            //UIÇï\é¶Ç∑ÇÈ
            _titleLogo.SetActive(true);
            _titleButton.SetActive(true);

            //ägëÂÇµÇΩèÛë‘Ç©ÇÁèkè¨Ç∑ÇÈ
            StartCoroutine(MyStartUI());
        }
        else
        {
            //UIÇîÒï\é¶Ç…Ç∑ÇÈ
            _titleLogo.SetActive(false);
            _titleButton.SetActive(false);
        }
    }


    private IEnumerator MyStartUI()
    {
        while(_size <= 1.0f)
        {
            _titleLogo.transform.localScale = Vector3.Lerp(new Vector3(5, 5, 5), new Vector3(1, 1, 1), _size);
            _size += _speed;

            //Debug.Log(_size);


            yield return null;
        }

    }
}
