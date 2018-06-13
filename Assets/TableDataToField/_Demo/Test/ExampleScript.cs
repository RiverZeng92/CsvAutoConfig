using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class ExampleScript : MonoBehaviour
{



    /*[Table("WorldConfig", "offset", "value")]
    public string name;
    [Table("BulletConfig", "1", "score")]
    public float offset;
    public int ss;*/

    /**
     * 方法一：
     * 在变量名后面加"_table_params"，变量名与键名一致
     * 调用调用静态方法ImportTableDataUtils.importDataFromTableByAttribute(要导入配表数据的对象, "表名", "主键名")；
     * 
     */

    public int test1_table_params;
    public float test2_table_params;
    public string test3_table_params;

   // public int valueName1_table_params;
    
    /**
     * 方法二：
     * 为变量加上字段特性，[Table("表名","主键名", "键名")] 
     * 调用调用静态方法ImportTableDataUtils.importDataFromTableByAttribute(要导入配表数据的对象)；
     */
    [Table("Method2", "1", "test2_1")]
    public int test2_1;

    [Table("Method2", "1", "test2_2")]
    public float test2_2;

    [Table("Method2", "1", "test2_3")]
    public string test2_3;
    


    // Use this for initialization
    void Start()
    {
        //方法一
        ImportTableData.importDataFromTable(this, "Method1", "1");
        //ImportTableData.improtDataFromTable(this, "Method3", "Value");

        //方法二
        ImportTableData.importDataFromTableByAttribute(this);
    }

}
