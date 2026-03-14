using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [SerializeField]
    private Entity[] entities;

    private Dictionary<string, Entity> entityDictPrefabs = new();
    private Dictionary<string, List<Entity>> entitiesPool = new();
    private Dictionary<int, Entity> instancedEntities = new();

    public void Initialize1()
    {
        SubscribeQueries();
        InitEntitiesPrefabs();
    }

    private void InitEntitiesPrefabs()
    {
        foreach (var entity in entities)
        {
            if (!entityDictPrefabs.ContainsKey(entity.EntityName))
            {
                entityDictPrefabs.Add(entity.EntityName, entity);
            }
            else
            {
                Debug.LogWarning($"Duplicate entity name detected: {entity.EntityName}. Skipping.");
            }
        }
    }

    private void SubscribeQueries()
    {
        QueryBus.Subscribe<GetBuildingDataListQuery, IReadOnlyList<BuildableEntity>>(query =>
            GetBuildingDataList()
        );
        QueryBus.Subscribe<GetEntityQuery, Entity>(query =>
            GetEntity(query.prefabId, query.position)
        );
        QueryBus.Subscribe<GetEntityPrefabQuery, Entity>(query => GetEntityPrefab(query.prefabId));
        QueryBus.Subscribe<GetActiveEntityQuery, Entity>(query => GetActiveEntity(query.prefabId));
        QueryBus.Subscribe<GetEmptyPlotQuery, Plot>(query => GetEmptyPlot());
        QueryBus.Subscribe<GetHarvestablePlotQuery, Plot>(query => GetHarvestablePlot());
        QueryBus.Subscribe<GetInstantiatedEntityQuery, Entity>(query =>
            GetInstancedEntity(query.instanceId)
        );
    }

    public Entity GetEntityPrefab(string prefabId)
    {
        if (entityDictPrefabs.TryGetValue(prefabId, out var prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogError($"Entity prefab not found for name: {prefabId}");
            return null;
        }
    }

    public Entity GetInstancedEntity(int instanceId)
    {
        if (instancedEntities.TryGetValue(instanceId, out var entity))
        {
            return entity;
        }
        else
        {
            Debug.LogError($"Entity instance not found for ID: {instanceId}");
            return null;
        }
    }

    public Entity GetActiveEntity(string prefabId)
    {
        if (entitiesPool.TryGetValue(prefabId, out var pool))
        {
            return pool.FirstOrDefault((x) => x.gameObject.activeSelf);
        }
        return null;
    }

    public Plot GetEmptyPlot()
    {
        var entityName = EntityConstant.Building.PLOT;
        if (entitiesPool.TryGetValue(entityName, out var pool))
        {
            for (var i = 0; i < pool.Count; i++)
            {
                var plot = pool[i] as Plot;
                if (
                    plot != null
                    && plot.gameObject.activeSelf
                    && plot.IsEmpty
                    && plot.BuildingState == BuildingState.READY
                )
                {
                    return plot;
                }
            }
        }
        return null;
    }

    public Plot GetHarvestablePlot()
    {
        var entityName = EntityConstant.Building.PLOT;
        if (entitiesPool.TryGetValue(entityName, out var pool))
        {
            for (var i = 0; i < pool.Count; i++)
            {
                var plot = pool[i] as Plot;
                if (
                    plot != null
                    && plot.gameObject.activeSelf
                    && plot.IsReady
                    && plot.BuildingState == BuildingState.READY
                )
                {
                    return plot;
                }
            }
        }
        return null;
    }

    public DiningTable GetAvailableDiningTable()
    {
        var entityName = EntityConstant.Building.DINING_TABLE;
        if (entitiesPool.TryGetValue(entityName, out var pool))
        {
            for (var i = 0; i < pool.Count; i++)
            {
                var diningTable = pool[i] as DiningTable;
                if (
                    diningTable != null
                    && diningTable.gameObject.activeSelf
                    && diningTable.IsAvailable
                    && diningTable.BuildingState == BuildingState.READY
                )
                {
                    return diningTable;
                }
            }
        }
        return null;
    }

    //! fix here
    private IReadOnlyList<BuildableEntity> GetBuildingDataList()
    {
        return entityDictPrefabs
            .Where(kv => kv.Value is BuildableEntity)
            .Select(kv => kv.Value as BuildableEntity)
            .ToList()
            .AsReadOnly();
    }

    public Entity GetEntity(string prefabId, Vector3 position)
    {
        if (entitiesPool.ContainsKey(prefabId) && entitiesPool[prefabId].Count > 0)
        {
            var inactiveEntity = entitiesPool[prefabId]
                .FirstOrDefault((x) => !x.gameObject.activeSelf);
            if (inactiveEntity != null)
            {
                inactiveEntity.transform.position = position;
                inactiveEntity.gameObject.SetActive(true);
                return inactiveEntity;
            }
            else
            {
                return SpawnEntity(prefabId, position);
            }
        }
        else if (entityDictPrefabs.ContainsKey(prefabId))
        {
            return SpawnEntity(prefabId, position);
        }
        else
        {
            Debug.LogError($"Entity prefab not found for name: {prefabId}");
            return null;
        }
    }

    private Entity SpawnEntity(string prefabId, Vector3 position)
    {
        var entityPrefab = entityDictPrefabs[prefabId];
        if (entityPrefab == null)
        {
            Debug.LogError($"Entity prefab not found for name: {prefabId}");
            return null;
        }
        var instance = Instantiate(entityPrefab, position, Quaternion.identity);
        entitiesPool.TryAdd(prefabId, new List<Entity>());
        entitiesPool[prefabId].Add(instance);
        instancedEntities.Add(instance.InstanceId, instance);
        return instance;
    }
}
