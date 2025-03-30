using UnityEngine;

public interface IAction
{
    public void Invoke(GameObject gameObject);
}

public abstract class BaseAction : ScriptableObject, IAction
{
    public abstract void Invoke(GameObject gameObject);
}
