using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    [SerializeField]
    public float span;

    [SerializeField]
    public ParticleSystem[] particles;

    void Start()
    {
        StartCoroutine("Logging");
    }

    IEnumerator Logging()
    {
        while (true)
        {
            int count = Random.Range(0, particles.Length);
            span += count / 10;
            yield return new WaitForSeconds(span);
            Debug.LogFormat("{0}秒経過", span);
            // パーティクルシステムのインスタンスを生成する。
            ParticleSystem newParticle = Instantiate(particles[count]);
            // パーティクルの発生場所をこのスクリプトをアタッチしているGameObjectの場所にする。
            newParticle.transform.position = this.transform.position;

            // パーティクルを発生させる。
            newParticle.Play();
            // インスタンス化したパーティクルシステムのGameObjectを削除する。(任意)
            // ※第一引数をnewParticleだけにするとコンポーネントしか削除されない。
            Destroy(newParticle.gameObject, 20.0f);
        }
    }
}