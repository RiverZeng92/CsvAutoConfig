using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;

public static class AutomaticConfigTable
{
    //加锁对象
    static object lockObject = new object();
    //Tag池
    static Dictionary<object, Dictionary<string, TablePropertyTag>> pool = new Dictionary<object, Dictionary<string, TablePropertyTag>>();

    /// <summary>
    /// 自动绑定读表
    /// </summary>
    /// <param name="bindObject">执行对象</param>
    public static void Bind(object bindObject)
    {
        lock (lockObject)
        {
            DetectionAttribute(bindObject);
            Type type = bindObject.GetType();
            foreach (string fieldName in pool[bindObject].Keys)
            {
                TablePropertyTag tag = pool[bindObject][fieldName];
                if (!string.IsNullOrEmpty(tag.tableName) && !string.IsNullOrEmpty(tag.primaryKey) && !string.IsNullOrEmpty(tag.key))
                {
                    string value = MyTable.GetInstance().GetValue(tag.tableName, tag.primaryKey, tag.key);
                    FieldInfo info = type.GetField(fieldName);
                    info.SetValue(bindObject, TypeDescriptor.GetConverter(info.FieldType).ConvertFrom(value));
                }
            }
        }
    }

    /// <summary>
    /// 更新Tag
    /// </summary>
    /// <param name="bindObject">执行对象</param>
    /// <param name="field">更新字段</param>
    /// <param name="parameters">更新参数</param>
    public static void UpdateTag(object bindObject, string field, params string[] parameters)
    {
        lock (lockObject)
        {
            if (!ObjectIsExist(bindObject))
                AddObject(bindObject);
            if (!FieldIsExist(bindObject, field))
                AddField(bindObject, field, new TablePropertyTag());
            TablePropertyTag tag = pool[bindObject][field];
            for (int i = 0; i < parameters.Length; i += 2)
            {
                FieldInfo info = tag.GetType().GetField(parameters[i]);
                info.SetValue(tag, parameters[i + 1]);
            }
        }
    }

    /// <summary>
    /// 检测特性
    /// </summary>
    /// <param name="bindObject">执行对象</param>
    static void DetectionAttribute(object bindObject)
    {
        FieldInfo[] fields = bindObject.GetType().GetFields();
        foreach (FieldInfo field in fields)
        {
            if (!FieldIsExist(bindObject, field.Name))
            {
                foreach (object attribute in field.GetCustomAttributes(true))
                {
                    Type type = attribute.GetType();
                    if (type.Equals(Type.GetType("System.AutomaticBindAttribute")))
                    {
                        string tableName = (string)type.GetField("tableName").GetValue(attribute);
                        string primaryKey = (string)type.GetField("primaryKey").GetValue(attribute);
                        string key = (string)type.GetField("key").GetValue(attribute);
                        if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(primaryKey) && !string.IsNullOrEmpty(key))
                            UpdateTag(bindObject, field.Name, TablePropertyTag.TABLENAME, tableName, TablePropertyTag.PRIMARYKEY, primaryKey, TablePropertyTag.KEY, key);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 添加对象
    /// </summary>
    /// <param name="bindObject">执行对象</param>
    static void AddObject(object bindObject)
    {
        lock (lockObject)
            if (!ObjectIsExist(bindObject))
                pool.Add(bindObject, new Dictionary<string, TablePropertyTag>());
    }

    /// <summary>
    /// 添加字段
    /// </summary>
    /// <param name="bindObject">执行对象</param>
    /// <param name="field">字段</param>
    /// <param name="tag">tag</param>
    static void AddField(object bindObject, string field, TablePropertyTag tag)
    {
        lock (lockObject)
            if (!FieldIsExist(bindObject, field) && ObjectIsExist(bindObject))
                pool[bindObject].Add(field, tag);
    }

    /// <summary>
    /// 检测对象是否存在池中
    /// </summary>
    /// <param name="bindObject">执行对象</param>
    /// <returns></returns>
    static bool ObjectIsExist(object bindObject)
    {
        lock (lockObject)
            return pool.ContainsKey(bindObject);
    }

    /// <summary>
    /// 检测字段是否存在
    /// </summary>
    /// <param name="bindObject">执行对象</param>
    /// <param name="field">字段</param>
    /// <returns></returns>
    static bool FieldIsExist(object bindObject, string field)
    {
        lock (lockObject)
        {
            if (ObjectIsExist(bindObject))
                return pool[bindObject].ContainsKey(field);
            else
                return false;
        }
    }
}
