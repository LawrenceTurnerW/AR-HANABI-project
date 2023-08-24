using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

using System;


public class MessageGet : MonoBehaviour
{
    string webURL = "https://chanabi.deno.dev/";
    string hc = "hc";
    string message = "message";

    // 文字を中央に持ってくる用の値
    int width = 300 / 20;
    int shiftPosition = 0;

    // 表示するパーティクルは外部参照
    [SerializeField]
    [Tooltip("打ち上げパーティクル")]
    public ParticleSystem launchParticle;

    // 表示するパーティクルは外部参照
    [SerializeField]
    [Tooltip("花火の中身のパーティクル")]
    public ParticleSystem particle;

    [System.Serializable]
    public class Body
    {
        public string message;
        public string dots;
        public string color;
    }

    [System.Serializable]
    public class SaveData
    {
        public List<Body> body;
    }

    void Start()
    {
        StartCoroutine("MessageHanabi");
    }

    IEnumerator MessageHanabi()
    {
        // 現在時刻を取得する。
        DateTime now = DateTime.UtcNow;
        long oldUnixTimeMilliseconds = new DateTimeOffset(now).ToUnixTimeMilliseconds();
        while (true)
        {
            // hcを叩いてdenoがスリープ状態なら起こす
            UnityWebRequest request = UnityWebRequest.Get(webURL + hc);

            //何かしらの理由で失敗した場合10秒後に再度hcを叩く
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("HCに失敗しました。");
                yield return new WaitForSeconds(10f);
            }
            else
            {
                // メッセージ取得API作成
                UnityWebRequest messageRequest = UnityWebRequest.Get(webURL + message + "?time=" + oldUnixTimeMilliseconds);
                yield return messageRequest.Send();

                // 時間を次回に備えて更新
                now = DateTime.UtcNow;
                oldUnixTimeMilliseconds = new DateTimeOffset(now).ToUnixTimeMilliseconds();

                if (messageRequest.result != UnityWebRequest.Result.ConnectionError)
                {
                    // 空なら無視する
                    var saveData = JsonUtility.FromJson<SaveData>(messageRequest.downloadHandler.text);
                    if (saveData.body.Count != 0)
                    {
                        // 2次元配列から1次元配列へ変換する
                        string stringLine = saveData.body[0].dots.Replace("[", "").Replace("]", "");
                        List<int> intList = stringLine.Split(',').ToList().Select(int.Parse).ToList();

                        if (intList.Count >= 0)
                        {
                            // この段階でメッセージ花火を生成することが決定したので打ち上げパーティクルを発生させる
                            // -20 から20の間でランダムに発生場所をずらす
                            shiftPosition = UnityEngine.Random.Range(-20, 20);

                            ParticleSystem launchParticleInstantiate = Instantiate(launchParticle);
                            // パーティクルの発生場所をアタッチしているGameObjectの場所にする。x座標のみshiftPosition分ずらす。
                            launchParticleInstantiate.transform.position = new Vector3(this.transform.position.x + shiftPosition, this.transform.position.y, this.transform.position.z);

                            // 打ち上げ中は待機。
                            launchParticleInstantiate.Play();
                            yield return new WaitForSeconds(5.0f);
                            for (var i = 0; i < intList.Count / 2; i++)
                            {
                                // 発生させるパーティクルの色を決定
                                ParticleSystem newParticle = Instantiate(particle);

                                ParticleSystem.MainModule par = newParticle.main;
                                par.startColor = Util.HexToRGB(saveData.body[0].color);

                                // パーティクルの発生場所を配列から読み取って作成する。x座標のみshiftPosition分ずらす。
                                newParticle.transform.position = new Vector3(intList[i * 2] / 10 - width + shiftPosition, -intList[i * 2 + 1] / 10 + 40, 25);

                                // パーティクルを発生させる。
                                newParticle.Play();

                                // インスタンス化したパーティクルシステムのGameObjectを削除する。
                                Destroy(newParticle.gameObject, 10.0f);
                            }
                        }
                    }
                    // 成功してもしなくても10秒待つ
                    yield return new WaitForSeconds(10f);
                }
            }
        }
    }
}

// カラーコードをUnityのColorに変換する。うまく変換できなければ赤色になる
public static class Util
{
    public static Color HexToRGB(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color)) return color;
        else return Color.red;
    }
}