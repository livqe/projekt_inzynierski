using System.Collections.Generic;

public interface ITargetableEffect
{
    int GetTargetCount();

    TargetAlignment GetTargetAlignment();

    void ExecuteWithTarget(List<CardInstance> targets);
}
