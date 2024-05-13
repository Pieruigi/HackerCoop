using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HKR
{
    public class CouplerApp : MonoBehaviour
    {
        class SpawnInfo
        {
            public GameObject spawnedObject;
            public bool hittable = false;
            public Vector3 moveDirection;
            public bool hit = false;
            public Vector3 originalScale;
        }

        [SerializeField]
        GameObject ui;

        [SerializeField]
        GameObject spawnPrefab;

        [SerializeField]
        Transform spawnLayer;

        [SerializeField]
        Color hittableSpawnColor = Color.green;

        [SerializeField]
        float timer = 15;

        [SerializeField]
        int hitTarget = 10;

        [SerializeField]
        int maxErrors = 2;

        [SerializeField]
        float radius = 0.115f;

        //[SerializeField]
        //float spawnRadius = 0.01f;

        [SerializeField]
        float objectSpeed = 0.2f;

        float spawnRate = .3f;


        PlayerDevice device;

        List<SpawnInfo> spawnInfoList = new List<SpawnInfo>();
        float elapsed = 0;

        HackingController hackingController;

        
        private void Awake()
        {
            device = GetComponentInParent<PlayerDevice>();
            hackingController = GetComponentInParent<HackingController>();
            spawnRate = radius * .9f / objectSpeed;
        }

        private void Update()
        {
            // Check for spawn
            elapsed += Time.deltaTime;
            if(elapsed > spawnRate)
            {
                elapsed -= spawnRate;
                // Spawn element
                GameObject go = Instantiate(spawnPrefab, spawnLayer);
                go.transform.localPosition = (Random.Range(-1f,1f) * Vector3.right + Random.Range(-1f, 1f) * Vector3.up ).normalized * radius;
                go.transform.localRotation = Quaternion.identity;
                Vector3 dir = go.transform.parent.localPosition - go.transform.localPosition;
                spawnInfoList.Add(new SpawnInfo() { spawnedObject = go, hittable = false, moveDirection = dir, originalScale = go.transform.localScale });
            }

            // Update spawned objects
            foreach(var info in spawnInfoList )
                info.spawnedObject.transform.localPosition += info.moveDirection.normalized * objectSpeed * Time.deltaTime;

            // Check input
            if (device.GetButtonDown())
            {
                // Target info
                var info = spawnInfoList.Find(s => s.hittable && !s.hit);
                if (info != null)
                {
                    info.hit = true;
                    hackingController.HitSucceded();
                    Destroy(info.spawnedObject);
                    spawnInfoList.Remove(info);
                }
                else
                {
                    hackingController.HitFailed();
                }
                
            }
            
        }

        private void OnEnable()
        {
            ClearAll();
            ui.SetActive(true);
            elapsed = -Constants.HackingAppPlayDelay;
            hackingController.SetAppData(timer, hitTarget, maxErrors);
        }

        private void OnDisable()
        {
            ClearAll();
            ui.SetActive(false);
            
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Entered:{other.gameObject.name}");
            SpawnInfo info = spawnInfoList.Where(i => i.spawnedObject == other.gameObject).First();
            if (info == null)
                return;

            // Set the object as hittable
            info.hittable = true;

            // Change the color
            Sequence seq = DOTween.Sequence();
            //SpriteRenderer sr = info.spawnedObject.GetComponent<SpriteRenderer>();
            //Color targetColor = sr.color * 3f;
            //seq.Append(DOTween.To(() => sr.color, x => sr.color = x, targetColor, .05f));
            seq.Append(info.spawnedObject.transform.DOScale(info.originalScale * 1.05f, 0.05f));
            seq.Play();
            //seq.SetDelay(3);
            //seq.onComplete += () => { Destroy(info.spawnedObject); spawnInfoList.Remove(info); };
        }

        private void OnTriggerExit(Collider other)
        {
            SpawnInfo info = spawnInfoList.Where(i => i.spawnedObject == other.gameObject).First();
            if (info == null)
                return;

            // No longer hittable
            info.hittable = false;

            // Move alpha to zero
            //SpriteRenderer sr = info.spawnedObject.GetComponent<SpriteRenderer>();
            //Color targetColor = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
            Sequence seq = DOTween.Sequence();
            //seq.Append(DOTween.To(() => sr.color, x => sr.color = x, targetColor, .2f));
            seq.Append(info.spawnedObject.transform.DOScale(info.originalScale, 0.05f));
            seq.onComplete += () => { Destroy(info.spawnedObject); spawnInfoList.Remove(info); };
            seq.Play();
        }

        void ClearAll()
        {
            elapsed = 0;
            
            foreach (SpawnInfo info in spawnInfoList)
            {
                Destroy(info.spawnedObject);
            }
            spawnInfoList.Clear();
            

        }

    }

}
