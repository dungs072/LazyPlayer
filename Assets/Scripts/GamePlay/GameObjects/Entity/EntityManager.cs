using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

public struct GetBuildingDataListQuery : IQueryResult<IReadOnlyList<EntityData>> { }

public struct EntityConfig
{
    public Vector3? position;
    public Quaternion? rotation;
    public Vector3? scale;
}

public class EntityManager : MonoBehaviour
{
    [SerializeField]
    private List<Entity> entityPrefabs = new();

    [SerializeField]
    private List<EntityData> entityDataList = new();
    private readonly Dictionary<EntityPrefabId, Entity> entityDictPrefabs = new();
    private readonly Dictionary<EntityPrefabId, List<Entity>> entitiesPool = new();
    private readonly Dictionary<EntityId, EntityData> entityDataDict = new();
    private readonly Dictionary<int, Entity> entityInstanceDict = new();

    public void Initialize1()
    {
        InitPrefabAndDataList();
        InitDictPrefabs();
        InitEntityDataDict();
        SubscribeQueries();
    }

    private void InitPrefabAndDataList()
    {
        if (entityPrefabs.Count > 0 && entityDataList.Count > 0)
        {
            Debug.LogWarning("Entity prefabs and data list are already initialized. Skipping.");
            return;
        }
        var gameLoader = GameLoader.Instance;
        var entityPrefabObjects = gameLoader.GetAsset(LabelKeys.ENTITY_PREFABS);
        var entityDataObjects = gameLoader.GetAsset(LabelKeys.ENTITY_DATA);
        foreach (var obj in entityPrefabObjects)
        {
            if (obj is Entity entityPrefab)
            {
                entityPrefabs.Add(entityPrefab);
            }
            else
            {
                Debug.LogWarning($"Asset with name {obj} is not of type Entity. Skipping.");
            }
        }
        foreach (var obj in entityDataObjects)
        {
            if (obj is EntityData entityData)
            {
                entityDataList.Add(entityData);
            }
            else
            {
                Debug.LogWarning($"Asset with name {obj} is not of type EntityData. Skipping.");
            }
        }
    }

    private void InitDictPrefabs()
    {
        foreach (var entity in entityPrefabs)
        {
            var prefabId = EntityPrefabConstant.GetPrefabId(entity.name);
            if (!entityDictPrefabs.ContainsKey(prefabId))
            {
                entityDictPrefabs.Add(prefabId, entity);
            }
            else
            {
                Debug.LogWarning($"Duplicate entity name detected: {entity.name}. Skipping.");
            }
        }
    }

    private void InitEntityDataDict()
    {
        foreach (var data in entityDataList)
        {
            var entityId = EntityId.ParseId(data.EntityId);
            if (!entityDataDict.ContainsKey(entityId))
            {
                entityDataDict.Add(entityId, data);
            }
            else
            {
                Debug.LogWarning($"Duplicate entity data ID detected: {data.EntityId}. Skipping.");
            }
        }
    }

    private void SubscribeQueries()
    {
        QueryBus.Subscribe<GetBuildingDataListQuery, IReadOnlyList<EntityData>>(
            (query) =>
            {
                return GetEntityDataList<EntityData>(data => data.EntityPrefab is BuildableEntity);
            }
        );
    }

    public Entity Acquire(EntityId entityId, EntityConfig config = default)
    {
        return Acquire<Entity>(entityId, config);
    }

    public T Acquire<T>(EntityId entityId, EntityConfig config = default)
        where T : Entity
    {
        var entityData = entityDataDict[entityId];
        var entityPrefab = entityData.EntityPrefab;
        var prefabId = EntityPrefabConstant.GetPrefabId(entityPrefab.name);
        Entity selectedEntity = null;
        if (entitiesPool.ContainsKey(prefabId) && entitiesPool[prefabId].Count > 0)
        {
            var inactiveEntity = entitiesPool[prefabId]
                .FirstOrDefault((x) => !x.gameObject.activeSelf);
            if (inactiveEntity != null)
            {
                selectedEntity = inactiveEntity;
            }
            else
            {
                selectedEntity = SpawnEntity(prefabId);
            }
        }
        else if (entityDictPrefabs.ContainsKey(prefabId))
        {
            selectedEntity = SpawnEntity(prefabId);
        }
        else
        {
            Debug.LogError($"Entity prefab not found for name: {prefabId}");
            return null;
        }
        if (config.position.HasValue)
            selectedEntity.transform.position = config.position.Value;

        if (config.rotation.HasValue)
            selectedEntity.transform.rotation = config.rotation.Value;

        if (config.scale.HasValue)
            selectedEntity.transform.localScale = config.scale.Value;
        selectedEntity.gameObject.SetActive(true);
        selectedEntity.Initialize(entityData);
        return (T)selectedEntity;
    }

    private Entity SpawnEntity(EntityPrefabId prefabId)
    {
        var entityPrefab = entityDictPrefabs[prefabId];
        if (entityPrefab == null)
        {
            Debug.LogError($"Entity prefab not found for name: {prefabId}");
            return null;
        }
        var instance = Instantiate(entityPrefab);
        instance.Init();
        entitiesPool.TryAdd(prefabId, new List<Entity>());
        entitiesPool[prefabId].Add(instance);
        Debug.Log(instance.InstanceId);
        entityInstanceDict.Add(instance.InstanceId, instance);
        return instance;
    }

    public void Release(Entity entity)
    {
        entity.gameObject.SetActive(false);
    }

    public Entity FindActiveEntity(EntityId entityId, Func<Entity, bool> condition = null)
    {
        return FindActiveEntity<Entity>(entityId, condition);
    }

    public T FindActiveEntity<T>(EntityId entityId, Func<T, bool> condition = null)
        where T : Entity
    {
        var entityData = entityDataDict[entityId];
        var prefabId = EntityPrefabConstant.GetPrefabId(entityData.EntityPrefab.name);

        if (entitiesPool.TryGetValue(prefabId, out var pool))
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i] is not T entity)
                    continue;

                if (!entity.gameObject.activeSelf)
                    continue;

                if (condition == null || condition(entity))
                    return entity;
            }
        }

        return null;
    }

    public IReadOnlyList<T> GetEntityDataList<T>(Func<T, bool> condition = null)
        where T : EntityData
    {
        var result = new List<T>();
        foreach (var data in entityDataDict.Values)
        {
            if (data is T typedData)
            {
                if (condition == null || condition(typedData))
                {
                    result.Add(typedData);
                }
            }
        }
        return result;
    }

    public EntityData GetEntityData(EntityId entityId)
    {
        return GetEntityData<EntityData>(entityId);
    }

    public T GetEntityData<T>(EntityId entityId)
        where T : EntityData
    {
        if (entityDataDict.TryGetValue(entityId, out var data))
        {
            if (data is T typedData)
            {
                return typedData;
            }
            else
            {
                Debug.LogError($"Entity data with ID {entityId} is not of type {typeof(T).Name}");
                return null;
            }
        }
        else
        {
            Debug.LogError($"Entity data not found for ID: {entityId}");
            return null;
        }
    }

    public Entity GetInstantiatedEntity(int instanceId)
    {
        if (entityInstanceDict.TryGetValue(instanceId, out var entity))
        {
            return entity;
        }
        else
        {
            Debug.LogError($"Entity not found for instance ID: {instanceId}");
            return null;
        }
    }
}
