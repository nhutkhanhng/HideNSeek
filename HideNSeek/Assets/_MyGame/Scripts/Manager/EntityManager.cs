using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EntityManager : MonoSingleton<EntityManager>
{
    private Dictionary<int, BaseUnit> m_EntityMap = new Dictionary<int, BaseUnit>();

    public void RegisterEntity(BaseUnit newEntity)
    {
        m_EntityMap.TryAdd(newEntity.ID(), newEntity);
    }

    public bool RemoveRegisterEnity(BaseUnit pEntitpy)
    {
        return m_EntityMap.Remove(pEntitpy.ID());
    }

    public BaseUnit GetEntity(int Id)
    {
        if (m_EntityMap.ContainsKey(Id))
        {
            return m_EntityMap[Id];
        }

        return null;
    }
    public void Clear()
    {
        m_EntityMap = new Dictionary<int, BaseUnit>();
    }
}
