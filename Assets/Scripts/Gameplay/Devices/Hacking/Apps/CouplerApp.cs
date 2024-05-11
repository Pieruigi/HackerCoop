using DG.Tweening;
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
        }

        [SerializeField]
        GameObject ui;

        [SerializeField]
        GameObject spawnPrefab;

        [SerializeField]
        Transform spawnLayer;

        [SerializeField]
        Color hittableSpawnColor = Color.green;

        float radius = 0.115f;

        float spawnRadius = 0.01f;

        float spawnRate = .3f;

        float spawnSpeed = 0.2f;

        int hitCount = 0;

        int hitTarget = 6;

        PlayerDevice device;

        List<SpawnInfo> spawnInfoList = new List<SpawnInfo>();
        float elapsed = 0;

        
        private void Awake()
        {
            device = GetComponentInParent<PlayerDevice>();
            spawnRate = radius * .8f / spawnSpeed;
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
                spawnInfoList.Add(new SpawnInfo() { spawnedObject = go, hittable = false, moveDirection = dir });
            }

            // Update spawned objects
            foreach(var info in spawnInfoList )
                info.spawnedObject.transform.localPosition += info.moveDirection.normalized * spawnSpeed * Time.deltaTime;

            // Check input
            if (device.GetButtonDown())
            {
                // Target info
                var info = spawnInfoList.Find(s => s.hittable && !s.hit);
                if (info != null)
                {
                    info.hit = true;
                    hitCount++;
                    Destroy(info.spawnedObject);
                    spawnInfoList.Remove(info);
                }
                else
                {
                    
                }
                
            }
            
        }

        private void OnEnable()
        {
            ClearAll();
            ui.SetActive(true);
            elapsed = Constants.HackingAppPlayDelay;
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
            SpriteRenderer sr = info.spawnedObject.GetComponent<SpriteRenderer>();
            Color targetColor = sr.color * 3f;
            seq.Append(DOTween.To(() => sr.color, x => sr.color = x, targetColor, .05f));
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
            SpriteRenderer sr = info.spawnedObject.GetComponent<SpriteRenderer>();
            Color targetColor = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
            Sequence seq = DOTween.Sequence();
            seq.Append(DOTween.To(() => sr.color, x => sr.color = x, targetColor, .2f));
            seq.onComplete += () => { Destroy(info.spawnedObject); spawnInfoList.Remove(info); };
            seq.Play();
        }

        void ClearAll()
        {
            elapsed = 0;
            hitCount = 0;
            foreach (SpawnInfo info in spawnInfoList)
            {
                Destroy(info.spawnedObject);
            }
            spawnInfoList.Clear();
            

        }

    }

}
