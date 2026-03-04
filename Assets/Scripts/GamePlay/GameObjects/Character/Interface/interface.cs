using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;

public interface ISwitchableJob
{
    void SetJob(BaseWorker job);
}
public interface IDoable
{
    void DoJobAsync(Func<CancellationToken, UniTask> action);
}