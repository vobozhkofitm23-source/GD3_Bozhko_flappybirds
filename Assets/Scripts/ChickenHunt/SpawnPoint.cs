using UnityEngine;

namespace ChickenHunt
{
    public class SpawnPoint : MonoBehaviour
    {
        [Header("Chickens")]
        [SerializeField] private Chicken[] _chickenPrefabs;

        [Header("Spawn Direction")]
        [SerializeField] private Vector2 _flyDirection = Vector2.left;

        public Chicken Spawn()
        {
            if (_chickenPrefabs == null || _chickenPrefabs.Length == 0) 
                return null;

            int index = Random.Range(0, _chickenPrefabs.Length);
            Chicken prefab = _chickenPrefabs[index];

            if (prefab == null) return null;

            Chicken chicken = Instantiate(prefab, transform.position, Quaternion.identity);

            if (chicken != null)
            {
                chicken.Initialize(_flyDirection);
            }

            return chicken;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.3f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)_flyDirection.normalized * 1.5f);
        }
    }
}
