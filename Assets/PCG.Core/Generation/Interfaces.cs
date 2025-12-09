public interface ILevelGenerator
{
    LevelData Generate(LevelParams p, System.Random rng);
}

public interface IConstraint
{
    bool Validate(LevelData level);
}

public interface IEvaluator
{
    EvaluationResult Evaluate(LevelData level);
}

public interface ISpawner
{
    void Populate(LevelData level, System.Random rng);
}

public class EvaluationResult
{
    public int roomCount;
    public float navigableRatio;
}
