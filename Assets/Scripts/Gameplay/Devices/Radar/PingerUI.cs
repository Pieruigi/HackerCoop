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
            float fadeInTime = lifeTime * .1f;
            float fadeOutTime = lifeTime * .5f;
            float fadeOutDelay = lifeTime - fadeInTime - fadeOutTime;

            Color c = Color.red;
            c.a = 0;
            image.color = c;
            Sequence seq = DOTween.Sequence();

            seq.Append(DOTween.To(() => image.color, x => image.color = x, Color.red, fadeInTime));
            seq.Append(DOTween.To(() => image.color, x => image.color = x, c, fadeOutTime).SetDelay(fadeOutDelay)).onComplete += () => { Destroy(gameObject); };
            seq.Play();
        }
    }

}
