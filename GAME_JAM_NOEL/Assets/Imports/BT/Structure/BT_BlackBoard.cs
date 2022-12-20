using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlackBoard", menuName = "ScriptableObjects/BT/Base/BlackBoard")]
public class BT_BlackBoard : ScriptableObject
{
    private Dictionary<string, int>    _intValues    = new Dictionary<string, int>();
    private Dictionary<string, float>  _floatValues  = new Dictionary<string, float>();
    private Dictionary<string, bool>   _boolValues   = new Dictionary<string, bool>();
    private Dictionary<string, object> _objectValues = new Dictionary<string, object>();

    public void SetInt(string _name, int value)
    {
        if (_intValues.ContainsKey(_name))
            _intValues[_name] = value;
        else
            _intValues.Add(_name, value);
    }
    public bool GetInt(string _name, out int value) => _intValues.TryGetValue(_name, out value);

    public void SetFloat(string _name, float value)
    {
        if (_floatValues.ContainsKey(_name))
            _floatValues[_name] = value;
        else
            _floatValues.Add(_name, value);
    }
    public bool GetFloat(string _name, out float value) => _floatValues.TryGetValue(_name, out value);

    public void SetBool(string _name, bool value)
    {
        if (_boolValues.ContainsKey(_name))
            _boolValues[_name] = value;
        else
            _boolValues.Add(_name, value);
    }
    
    public bool GetBool(string _name) => _boolValues.TryGetValue(_name, out bool value) && value;

    public void SetObject(string _name, object value)
    {
        if (_objectValues.ContainsKey(_name))
            _objectValues[_name] = value;
        else
            _objectValues.Add(_name, value);
    }
    public object GetObject(string _name) => _objectValues.TryGetValue(_name, out object value) ? value : null;

    public static BT_BlackBoard Clone()
    {
        return CreateInstance<BT_BlackBoard>();
    }
}
