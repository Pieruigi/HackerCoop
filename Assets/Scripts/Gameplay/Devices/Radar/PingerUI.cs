using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HKR.UI
{
    public class PingerUI : MonoBehaviour
    {
        [SerializeField]
        Image image;

        //[SerializeField]
        float fadeInSpeed = .1f;

        //[SerializeField]
        float fadeOutSpeed = .25f;

        float lifeTime;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetLifeTime(float lifeTime)
        {
            this.lifeTime = lifeTime;
            fadeInSpeed = lifeTime * .25f;
            fadeOutSpeed = lifeTime * .75f;

            Color c = Color.red;
            c.a = 0;
            image.color = c;
            Sequence seq = DOTween.Sequence();

            seq.Append(DOTween.To(() => image.color, x => image.color = x, Color.red, fadeInSpeed));
            seq.Append(DOTween.To(() => image.color, x => image.color = x, c, fadeOutSpeed)).onComplete += () => { Destroy(gameObject); };
            seq.Play();
        }
    }

}
