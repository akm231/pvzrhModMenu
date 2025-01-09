using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using static Board;
namespace TestWai;
public static class SomeMethod
{
    public unsafe static void CopyToIl2CppArray(bool[] standardArray, Il2CppStructArray<bool> il2cppArray)
    {
        // 确保两个数组长度一致
        if (standardArray.Length != il2cppArray.Length)
        {
            return;
        }

        // 遍历标准数组，将值复制到 Il2CppStructArray 中
        for (int i = 0; i < standardArray.Length; i++)
        {
            il2cppArray[i] = standardArray[i];
        }
    }

    public static void ReadBuffsToGame()
    {
        TravelMgr travelMgr = ComponentHolderProtocol.GetOrAddComponent<TravelMgr>(GameObject.Find("GameAPP"));
        bool[] a = GetEnabledStatusArray(Path.Combine(Plugin.dllDirectory, "buffs.txt"));
        bool[] b = new bool[travelMgr.advancedUpgrades.Length];
        bool[] c = new bool[travelMgr.ultimateUpgrades.Length];
        int i = 0;
        for (; i < travelMgr.advancedUpgrades.Length; i++)
        {
            b[i] = a[i];
        }
        for (int j = 0; j < travelMgr.ultimateUpgrades.Length; j++, i++)
        {
            c[j] = a[i];
        }
        CopyToIl2CppArray(b, travelMgr.advancedUpgrades);
        CopyToIl2CppArray(c, travelMgr.ultimateUpgrades);
    }

    public static bool[] GetEnabledStatusArray(string filePath)
    {
        // 读取文件的所有行
        string[] lines = File.ReadAllLines(filePath);
        int count = 0;

        // 先计算启用状态的数量
        foreach (string line in lines)
        {
            if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                continue; // 跳过注释行和空行

            if (line.Contains("是否启用："))
            {
                count++; // 计数启用状态的数量
            }
        }

        // 创建一个布尔数组以存储启用状态
        bool[] enabledStatus = new bool[count];
        int index = 0;

        // 重新遍历文件行以填充布尔数组
        foreach (string line in lines)
        {
            if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                continue; // 跳过注释行和空行

            if (line.Contains("是否启用："))
            {
                // 提取启用状态并转换为布尔值
                string status = line.Split(new[] { "是否启用：" }, System.StringSplitOptions.None)[1].Trim();
                enabledStatus[index++] = status.Equals("true", System.StringComparison.OrdinalIgnoreCase);
            }
        }

        return enabledStatus;
    }

    public static void SaveBoardTag()
    {
        string result = "";
        result += "# 胆小菇之梦\n是否启用：false\n";
        result += "# 塔防\n是否启用：false\n";
        result += "# 种子雨\n是否启用：false\n";
        result += "# 坚不可摧\n是否启用：false\n";
        result += "# 排山倒海\n是否启用：false\n";
        result += "# 超级随机\n是否启用：false\n";
        result += "# 夜晚\n是否启用：false\n";
        result += "# 大地图\n是否启用：false\n";
        result += "# 无尽\n是否启用：false\n";
        result += "# 允许旅行植物\n是否启用：false\n";
        result += "# 允许旅行buff\n是否启用：false\n";
        result += "# 屋顶\n是否启用：false\n";
        string path = Path.Combine(Plugin.dllDirectory, "Board.txt");
        File.WriteAllText(path, result);
    }

    public static void LoadBoardTag()
    {
        // 文件路径
        string path = Path.Combine(Plugin.dllDirectory, "Board.txt");
        string content = File.ReadAllText(path);

        // 正则表达式匹配 "是否启用：true" 或 "是否启用：false"
        Regex regex = new Regex(@"是否启用：(true|false)");

        // 查找所有匹配的布尔值
        MatchCollection matches = regex.Matches(content);

        // 创建一个布尔数组来存储结果
        bool[] boolArray = new bool[matches.Count];

        // 遍历匹配结果并解析为 bool 值
        for (int i = 0; i < matches.Count; i++)
        {
            boolArray[i] = bool.Parse(matches[i].Groups[1].Value);
        }
        BoardTag tag = new BoardTag();
        tag.isScaredyDream = boolArray[0];
        tag.isTowerDefence = boolArray[1];
        tag.isSeedRain = boolArray[2];
        tag.isIndestructible = boolArray[3];
        tag.isColumn = boolArray[4];
        tag.isSuperRandom = boolArray[5];
        tag.isNight = boolArray[6];
        tag.isBigMap = boolArray[7];
        tag.isEndless = boolArray[8];
        tag.enableTravelPlant = boolArray[9];
        tag.enableTravelBuff = boolArray[10];
        tag.isRoof = boolArray[11];
        Board.Instance.boardTag = tag;
    }

}
