using UnityEngine;
using System;
using BuildingType = EntityConstant.Building.BuildingType;
[Serializable]
public class StepData
{
    public BuildingType buildingType;
    public float duration;
}

[CreateAssetMenu(fileName = "New Job Data", menuName = "Job/JobData", order = 1)]
public class JobData : ScriptableObject
{
    [SerializeField] private string jobName;
    [SerializeField] private StepData[] steps;

    public string JobName { get { return jobName; } }
    public StepData[] Steps { get { return steps; } }

}
