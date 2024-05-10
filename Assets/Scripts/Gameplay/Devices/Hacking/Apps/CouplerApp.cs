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

        float spawnRate = 1f;

        float spawnSpeed = 0.5f;

        int hitCount = 0;

        int hitTarget = 6;

        

        List<SpawnInfo> spawnInfoList = new List<SpawnInfo>();
        float elapsed = 0;

        
        private void Awake()
        {
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
                info.spawnedObject.transform.localPosition += info.moveDirection * spawnSpeed * Time.deltaTime;


            
        }

        private void OnEnable()
        {
            ClearAll();
            ui.SetActive(true);
            
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

            Sequence seq = DOTween.Sequence();
            seq.SetDelay(3);
            seq.onComplete += () => { Destroy(info.spawnedObject); spawnInfoList.Remove(info); };
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
