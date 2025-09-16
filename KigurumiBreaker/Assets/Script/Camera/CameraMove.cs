using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    [SerializeField] private GameObject _player; // �v���C���[�I�u�W�F�N�g�̎Q��

    [SerializeField] private Vector3 _offset; // �J�����ƃv���C���[�̑��Έʒu

    [SerializeField] private BoxCollider _moveArea; // �J�����̈ړ��͈͂��w�肷��BoxCollider


    private Vector3 _initialRotation; // �J�����̏�����]��ۑ�����ϐ�

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(45.0f, -29.0f, -4.5f); // �J�����̏�����]��ݒ�
    }

    // Update is called once per frame
    void Update()
    {
        // �v���C���[�̈ʒu�ɃI�t�Z�b�g���������ʒu�ɃJ�������ړ�
        transform.position = _player.transform.position + _offset;

        // �J�����̈ʒu���ړ��͈͂𒴂��Ȃ��悤�ɐ���
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, _moveArea.bounds.min.x, _moveArea.bounds.max.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, _moveArea.bounds.min.y, _moveArea.bounds.max.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, _moveArea.bounds.min.z, _moveArea.bounds.max.z);
        transform.position = clampedPosition;
    }
}
