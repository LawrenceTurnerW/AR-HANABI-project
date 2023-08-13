using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    /// <summary>
    /// 弾の速度 (m/s)
    /// </summary>
    [SerializeField]
    public ParticleSystem[] particles;



    // Update is called once per frame
    public void Activate()
    {
        ShootAmmo();
    }

    /// <summary>
    /// 銃弾を生成する。
    /// </summary>
    private void ShootAmmo()
    {
        int count = Random.Range(0, particles.Length);
        ParticleSystem newParticle = Instantiate(particles[count]);
        // パーティクルの発生場所をこのスクリプトをアタッチしているGameObjectの場所にする。
        newParticle.transform.position = this.transform.position;
        newParticle.transform.rotation = this.transform.rotation;
        newParticle.transform.Rotate(0, 90, 0);

        // パーティクルを発生させる。
        newParticle.Play();
        // インスタンス化したパーティクルシステムのGameObjectを削除する。(任意)
        // ※第一引数をnewParticleだけにするとコンポーネントしか削除されない。
        Destroy(newParticle.gameObject, 20.0f);
    }
}