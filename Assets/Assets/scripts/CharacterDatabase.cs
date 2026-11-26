using UnityEngine;

[CreateAssetMenu(menuName = "Game/Character Database")]

public class CharacterDatabase : ScriptableObject

{
    [System.Serializable]
    public class Entry
    {
        public string displayName;
        public Sprite preview;                        
        public RuntimeAnimatorController animator;    // to później jak wlecą animacje itp
        
    }

    public Entry[] characters;
    public int Count => characters != null ? characters.Length : 0;
}