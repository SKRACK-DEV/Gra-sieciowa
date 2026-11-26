using System.Collections.Generic;

public static class CharacterSelection
{
    public static int LocalIndex;                                   
    public static readonly Dictionary<ulong, int> Choices = new();  

    public static int Get(ulong clientId) => Choices.TryGetValue(clientId, out var i) ? i : 0;
}
