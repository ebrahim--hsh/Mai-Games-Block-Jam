using System.Collections.Generic;

public class AgentsDictionary
{
    private static readonly Dictionary<int, AgentBase> Dic = new();

    public static void Add(int key, AgentBase agent)
    {
        Dic.Add(key, agent);
    }

    public static void Remove(int key)
    {
        Dic.Remove(key);
    }

    public static AgentBase GetAgent(int key)
    {
        return Dic[key];
    }
}