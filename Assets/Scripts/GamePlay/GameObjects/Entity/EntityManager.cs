using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [SerializeField] private Entity[] entities;

    private Dictionary<string, Entity> entityDictPrefabs = new();
    private Dictionary<string, List<Entity>> entitiesPool = new();

    public Entity GetActiveEntity(string entityName)
    {
        if (entitiesPool.TryGetValue(entityName, out var pool))
        {
            return pool.FirstOrDefault((x) => x.gameObject.activeSelf);
        }
        return null;
    }
    public Plot GetEmptyPlot(string plotName)
    {
        if (entitiesPool.TryGetValue(plotName, out var pool))
        {
            for (var i = 0; i < pool.Count; i++)
            {
                var plot = pool[i] as Plot;
                if (plot != null && plot.gameObject.activeSelf && plot.IsEmpty
                && plot.BuildingState == BuildingState.READY)
                {
                    return plot;
                }
            }
        }
        return null;
    }
    public Plot GetHarvestablePlot(string plotName)
    {
        if (entitiesPool.TryGetValue(plotName, out var pool))
        {
            for (var i = 0; i < pool.Count; i++)
            {
                var plot = pool[i] as Plot;
                if (plot != null && plot.gameObject.activeSelf && plot.IsReady
                 && plot.BuildingState == BuildingState.READY)
                {
                    return plot;
                }
            }
        }
        return null;
    }
    public DiningTable GetAvailableDiningTable(string diningTableName)
    {
        if (entitiesPool.TryGetValue(diningTableName, out var pool))
        {
            for (var i = 0; i < pool.Count; i++)
            {
                var diningTable = pool[i] as DiningTable;
                if (diningTable != null && diningTable.gameObject.activeSelf &&
                diningTable.IsAvailable && diningTable.BuildingState == BuildingState.READY)
                {
                    return diningTable;
                }
            }
        }
        return null;
    }

    void Awake()
    {
        //! fix here
        QueryBus.Subscribe<GetBuildingDataListQuery, IReadOnlyList<BuildableEntity>>(query => GetBuildingDataList());
        QueryBus.Subscribe<GetEntityQuery, Entity>(query => GetEntity(query.entityName, query.position));
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
    //! fix here
    private IReadOnlyList<BuildableEntity> GetBuildingDataList()
    {
        return entityDictPrefabs.Where(kv => kv.Value is BuildableEntity)
                           .Select(kv => kv.Value as BuildableEntity)
                           .ToList()
                           .AsReadOnly();
    }


    public Entity GetEntity(string entityName, Vector3 position)
    {
        if (entitiesPool.ContainsKey(entityName) && entitiesPool[entityName].Count > 0)
        {
            var inactiveEntity = entitiesPool[entityName].FirstOrDefault((x) => !x.gameObject.activeSelf);
            if (inactiveEntity != null)
            {
                inactiveEntity.transform.position = position;
                inactiveEntity.gameObject.SetActive(true);
                return inactiveEntity;
            }
            else
            {
                return SpawnEntity(entityName, position);
            }
        }
        else if (entityDictPrefabs.ContainsKey(entityName))
        {
            return SpawnEntity(entityName, position);
        }
        else
        {
            Debug.LogError($"Entity prefab not found for name: {entityName}");
            return null;
        }
    }

    private Entity SpawnEntity(string entityName, Vector3 position)
    {
        var entityPrefab = entityDictPrefabs[entityName];
        if (entityPrefab == null)
        {
            Debug.LogError($"Entity prefab not found for name: {entityName}");
            return null;
        }
        var instance = Instantiate(entityPrefab, position, Quaternion.identity);
        entitiesPool.TryAdd(entityName, new List<Entity>());
        entitiesPool[entityName].Add(instance);
        return instance;
    }


}
