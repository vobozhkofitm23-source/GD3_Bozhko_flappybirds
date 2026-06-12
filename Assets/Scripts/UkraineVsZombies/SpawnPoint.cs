using UnityEngine;

namespace UkraineVsZombies
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private GameObject[] _enemyPrefabs;

        public Enemy Spawn()
        {
            if (_enemyPrefabs == null || _enemyPrefabs.Length == 0)
                return null;

            int index = Random.Range(0, _enemyPrefabs.Length);
            GameObject prefab = _enemyPrefabs[index];

            if (prefab == null) return null;

            var obj = Instantiate(prefab, transform.position, Quaternion.identity);
            var enemy = obj.GetComponent<Enemy>();

            if (enemy != null)
                enemy.Initialize();

            return enemy;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.left * 2f);
        }
    }
}