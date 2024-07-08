using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyAction { Inventory, Equip, Quest, Achievement, KEYCOUNT}

public static class KeySetting { public static Dictionary<KeyAction, KeyCode> keys= new Dictionary<KeyAction, KeyCode>();}
public class KeyManager : Singleton<KeyManager>
{
    KeyCode[] defaultKeys = new KeyCode[] { KeyCode.I, KeyCode.O, KeyCode.L, KeyCode.K };
    int key = -1;
    private void Awake()
    {
        for(int i=0; i < (int)KeyAction.KEYCOUNT; i++)
        {
            KeySetting.keys.Add((KeyAction)i, defaultKeys[i]);
        }
    }

    private void OnGUI()
    {
        Event keyEvent = Event.current;
        if (keyEvent.isKey)
        {
            KeySetting.keys[(KeyAction)key] = keyEvent.keyCode;
            key = -1;
        }
    }

    public void ChangeKey(int num)
    {
        key = num;
    }
}
