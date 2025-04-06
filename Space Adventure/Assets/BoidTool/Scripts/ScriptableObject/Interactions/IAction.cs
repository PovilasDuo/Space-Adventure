using UnityEngine;

public interface IAction
{
    public void Invoke(GameObject gameObject);
}

public abstract class BaseAction : ScriptableObject, IAction
{
    /// <summary>
    /// This method is called when the action is invoked.
    /// </summary>
    /// <param name="gameObject">The GameObject for which to get the component required</param>
    public abstract void Invoke(GameObject gameObject);
}
