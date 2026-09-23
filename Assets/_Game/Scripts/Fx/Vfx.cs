using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace RichRun
{
    /// <summary>
    /// Пул партиклов. Instantiate происходит только пока пул пустой; дальше
    /// системы переиспользуются. Возврат — по таймеру в одном Update, без корутин.
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public class Vfx : MonoBehaviour
    {
        struct Live
        {
            public ParticleSystem Instance;
            public IObjectPool<ParticleSystem> Pool;
            public float ReleaseTime;
        }

        static Vfx _instance;

        [SerializeField] int prewarmPerPrefab = 4;

        readonly Dictionary<ParticleSystem, IObjectPool<ParticleSystem>> _pools = new(8);
        readonly List<Live> _live = new(32);

        void Awake() => _instance = this;

        void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        static bool _warned;

        public static void Play(ParticleSystem prefab, Vector3 position)
        {
            if (!prefab) return;

            if (_instance == null)
            {
                if (!_warned)
                {
                    _warned = true;
                    Debug.LogWarning("[RichRun] В сцене нет компонента Vfx — эффекты играют " +
                                     "без пула. Повесь Vfx на объект Systems.");
                }

                // Без пула объект никто не заберёт, поэтому убираем его по таймеру.
                var solo = Instantiate(prefab, position, prefab.transform.rotation);
                Destroy(solo.gameObject, 5f);
                return;
            }

            _instance.Spawn(prefab, position);
        }

        void Spawn(ParticleSystem prefab, Vector3 position)
        {
            var pool = GetPool(prefab);
            var ps = pool.Get();
            ps.transform.position = position;
            ps.Play(true);

            var main = ps.main;
            _live.Add(new Live
            {
                Instance = ps,
                Pool = pool,
                ReleaseTime = Time.time + main.duration + main.startLifetime.constantMax
            });
        }

        IObjectPool<ParticleSystem> GetPool(ParticleSystem prefab)
        {
            if (_pools.TryGetValue(prefab, out var pool)) return pool;

            pool = new ObjectPool<ParticleSystem>(
                createFunc: () => Instantiate(prefab, transform),
                actionOnGet: ps => ps.gameObject.SetActive(true),
                actionOnRelease: ps => ps.gameObject.SetActive(false),
                actionOnDestroy: ps => Destroy(ps.gameObject),
                defaultCapacity: prewarmPerPrefab);

            _pools.Add(prefab, pool);
            return pool;
        }

        void Update()
        {
            float now = Time.time;
            for (int i = _live.Count - 1; i >= 0; i--)
            {
                if (now < _live[i].ReleaseTime) continue;
                _live[i].Pool.Release(_live[i].Instance);
                _live.RemoveAt(i);
            }
        }
    }
}
