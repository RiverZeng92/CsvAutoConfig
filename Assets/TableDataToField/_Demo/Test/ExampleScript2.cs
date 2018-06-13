using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class ExampleScript2 : MonoBehaviour
{
    /**
     * 方法一：
     * 为变量加上字段特性，[AutomaticBind] 或 [AutomaticBind("表名","主键名", "键名")] 
     * 调用静态方法AutomaticConfigTable.Bind(要导入配表数据的对象)自动配置数据
     * 注：
     *  没有设置好绑定参数不会自动配置数据
     *  字段特性可选择是否初始化设置自动配置路径
     * 方法二：
     * 动态为变量设置自动配置路径调用AutomaticConfigTable.UpdateTag(要导入配表数据的对象,更新自动配置字段，更新值)
     * 注：
     *  可选择更新字段，后面带上更新值
     *  可动态更新已设置好的字段
     */
    [AutomaticBind("Method2", "1", "test2_1")]
    public int test2_1;

    [AutomaticBind("Method2", "1", "test2_2")]
    public float test2_2;

    [AutomaticBind]
    public string test2_3;
    


    // Use this for initialization
    void Start()
    {
        //方法一
        AutomaticConfigTable.Bind(this);

        //方法二
        AutomaticConfigTable.UpdateTag(this, "test2_3", TablePropertyTag.TABLENAME, "Method2", TablePropertyTag.PRIMARYKEY, "1", TablePropertyTag.KEY, "test2_3");
        //或
        AutomaticConfigTable.UpdateTag(this, "test2_2", TablePropertyTag.TABLENAME, "Method2");
        AutomaticConfigTable.Bind(this);
    }

}
