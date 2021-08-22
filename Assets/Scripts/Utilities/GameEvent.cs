using System;
using UnityEngine.Events;

[Serializable]
public class GameEvent : UnityEvent { }

[Serializable]
public class GameEventInt : UnityEvent<int> { }

[Serializable]
public class GameEventString : UnityEvent<string> { }