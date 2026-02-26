using System;
using System.Collections;

public interface ISwitchableJob
{
    void SetJob(BaseWorker job);
}
public interface IDoable
{
    void DoJobAsync(Func<IEnumerator> action);
}