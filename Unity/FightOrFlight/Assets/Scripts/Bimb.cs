using Assets.Scripts;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bimb : MonoBehaviour
{
    public GameObject DamageHitboxPrefub;
    public Animator Animator;
    public Light2D BoomLight, BeepLight;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BimbBoomDelay());
    }

    IEnumerator BimbBoomDelay()
    {
        BoomLight.gameObject.SetActive(false);
        yield return new WaitForSeconds(2.75f);

        BeepLight.gameObject.SetActive(false);
        BoomLight.gameObject.SetActive(true);
        Animator = GetComponent<Animator>();
        Animator.SetBool("isBoom", true);

        //yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < 18; i++)
        {
            float spreadAngle = Random.Range(-360f, 360f); // √енерируем случайный угол разброса
            Quaternion spreadRotation = Quaternion.Euler(0f, 0f, spreadAngle); // —оздаем кватернион поворота дл€ угла разброса

            var hitbox = Instantiate(
                DamageHitboxPrefub,
                this.transform
            ).GetComponent<DamageHitbox>();
            hitbox.transform.localScale = new Vector3(0.5f, 0.6f);
            // ѕримен€ем разброс к направлению снар€да
            Vector3 bulletDirection = spreadRotation * Vector3.up;
            hitbox.init(
                Assets.Scripts.DamageManager.DamageTypes.thermal,
                6,
                this.gameObject,
                true,
                0.7f);
            hitbox.init_as_bullet(7.3f);
            hitbox.transform.up = bulletDirection; // ”станавливаем направление снар€да с учетом разброса


            hitbox = Instantiate(
                DamageHitboxPrefub,
                this.transform
            ).GetComponent<DamageHitbox>();
            hitbox.transform.localScale = new Vector3(0.5f, 0.6f);
            hitbox.init(
                Assets.Scripts.DamageManager.DamageTypes.thermal,
                6,
                this.gameObject,
                false,
                0.7f);
            hitbox.init_as_bullet(7.3f);
            hitbox.transform.up = bulletDirection; // ”станавливаем направление снар€да с учетом разброса
        }
        
        SoundManager.PlaySound(gameObject, "Boom");
        yield return new WaitForSeconds(1);

        PhotonNetwork.Destroy(this.GetComponent<PhotonView>());
    }
        


    // Update is called once per frame
    void Update()
    {
        
    }
}
