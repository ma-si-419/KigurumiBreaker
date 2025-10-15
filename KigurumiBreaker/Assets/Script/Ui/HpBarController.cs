//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;


//public class HpBarController : MonoBehaviour
//{
//    [SerializeField] private GameObject _player;               //プレイヤーオブジェクト

//    [SerializeField] private Image _specialGaugeImage;           //ゲージ画像
//    [SerializeField] private Image _auraImage;                  //オーラの画像
//    [SerializeField] private Image _auraImage2;                  //オーラの画像
//    [SerializeField] private Image _hpGaugeImage;                //ゲージ画像

//    [SerializeField] private Color _normalColor = Color.yellow;  //通常時の色
//    [SerializeField] private Color _maxColor = Color.red;        //マックス時の色

//    [SerializeField] private float _flashSpeed;                  //点滅速度    
//    [SerializeField] private float _auraRotateSpeed;             //オーラの回転速度

//    [SerializeField] private TMP_Text _shootNumText;               //弾の数表示用テキスト
//    [SerializeField] private TMP_Text _shootNumMaxText;               //弾の数表示用テキスト
//    [SerializeField] private TMP_Text _hpText;               //弾の数表示用テキスト
//    [SerializeField] private TMP_Text _hpMaxText;               //弾の数表示用テキスト

//    private float _current = 0f;    //現在のゲージ量
//    private float _currentHp = 0f;    //現在のゲージ量
//    private float _max = 100f;      //ゲージの最大量
//    private float _maxHp = 100f;      //ゲージの最大量

//    private int _shootNum = 1;      //プレイヤーの弾の数
//    private int _shootMaxNum = 2;   //プレイヤーの弾の数の最大値

//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
