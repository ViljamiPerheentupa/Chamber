using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerDamage {
    void TakeDamage(int damage, GameObject source);
}
public interface IRestorable {
    void Restore();
    void Lock();
}

public interface IUIMessage {
    void UIMessage(string text, float duration);
}