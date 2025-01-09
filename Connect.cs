using BepInEx.Unity.IL2CPP.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using TestWai;
using Unity.VisualScripting;
using UnityEngine;
using System.IO.Clear;
public static class SocketServer
{
    private static TcpListener server;
    private static Thread serverThread;
    private static bool isRunning = false;

    public static void StartServer()
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (isRunning) return;

        isRunning = true;
        serverThread = new Thread(() =>
        {
            try
            {
                // 监听端口
                server = new TcpListener(IPAddress.Loopback, 12348); // 监听本地 12345 端口
                server.Start();
                Console.WriteLine("Socket 服务已启动，等待连接...");

                while (isRunning)
                {
                    // 等待客户端连接
                    var client = server.AcceptTcpClient();
                    Console.WriteLine("客户端已连接");

                    // 创建一个线程处理客户端请求
                    Thread clientThread = new Thread(() =>
                    {
                        HandleClient(client);
                    });
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Socket 服务出错: " + ex.Message);
            }
        });
        serverThread.Start();
    }

    public static void StopServer()
    {
        isRunning = false;
        server?.Stop();
        serverThread?.Join();
        Console.WriteLine("Socket 服务已停止");
    }

    private static void HandleClient(TcpClient client)
    {
        try
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;
            
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                // 接收到的消息
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("getMessage " + message);

                // 解析消息并调用命令
                string response = ProcessCommand(message);

                // 发送响应
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("处理客户端时出错: " + ex.Message);  // 记录错误
        }
        finally
        {
            client.Close();  // 确保客户端连接关闭
        }
    }


    private static string ProcessCommand(string command)
    {
        try
        {
            // 解析指令，例如 "applyhp|1|10|5"
            Console.WriteLine("进入ProcessCommand" + command);  
            string[] parts = command.Split('|');
            Console.WriteLine("parts长度"+parts.Length);
            if (parts.Length > 0)
            {
                string methodName = parts[0]; // 方法名，例如 "applyhp"
                // 根据指令调用对应方法
                switch (methodName)
                {
                    case "setSun":
                        int sunCount = int.Parse(parts[1]);
                        Board.Instance.theSun = sunCount;
                        break;
                    case "cardNoCD":
                        Plugin.config.isCardNoCD = !Plugin.config.isCardNoCD;
                        break;
                    case "gloveNoCD":
                        Plugin.config.isGloveNoCD = !Plugin.config.isGloveNoCD;
                        break;
                    case "hammerNoCD":
                        Plugin.config.isHammerNoCD = !Plugin.config.isHammerNoCD;
                        break;
                    case "freePlant":
                        Plugin.config.isFreePlant = !Plugin.config.isFreePlant;
                        break;
                    case "developerMode":
                        GameAPP.developerMode = !GameAPP.developerMode;
                        break;
                    case "fastShoot":
                        Plugin.config.isFastShoot = !Plugin.config.isFastShoot;
                        break;
                    case "countlessGold":
                        Plugin.config.isGoldCountless = !Plugin.config.isGoldCountless;
                        break;
                    case "ignoreZombieIn":
                        Plugin.config.isIgnoreZombieIn = !Plugin.config.isIgnoreZombieIn;
                        break;
                    case "clearPlant":
                        clearPlant();
                        break;
                    case "clearZombie":
                        clearZombie();
                        break;
                    case "createItem":
                        int type = int.Parse(parts[1]);
                        CreateItem.Instance.SetCoin(2,2, type,0);
                        break;
                    case "plantMode":
                        Plugin.config.plantMode = int.Parse(parts[1]);
                        break;
                    case "saveLineup":
                        saveLineup(parts[1]);
                        break;
                    case "loadLineup":
                        Board.Instance.StartCoroutine(loadLineup(parts[1]));
                        break;
                    case "bulletKill":
                        Plugin.config.isBulletKill = !Plugin.config.isBulletKill;
                        break;
                    case "randomBullet":
                        Plugin.config.isRandomBullet = !Plugin.config.isRandomBullet;
                        break;
                    case "randomPlant":
                        Plugin.config.isRandomPlant = !Plugin.config.isRandomPlant;
                        break;
                    case "randomZombie":
                        Plugin.config.isRandomZombie = !Plugin.config.isRandomZombie;
                        break;
                    case "createZombieRate":
                        Plugin.config.isCreateZombieRate = !Plugin.config.isCreateZombieRate;
                        break;
                    case "zombieRate":
                        Plugin.config.createZombieRate = int.Parse(parts[1]);
                        break;
                    case "zombieCold":
                        Plugin.config.isZombieCold = !Plugin.config.isZombieCold;
                        break;
                    case "zombieFreeze":
                        Plugin.config.isZombieFreeze = !Plugin.config.isZombieFreeze;
                        break;
                    case "zombieMindControlled":
                        Plugin.config.isZombieMindControlled = !Plugin.config.isZombieMindControlled;
                        break;
                    case "plantWD":
                        Plugin.config.isPlantWD = !Plugin.config.isPlantWD;
                        break;
                    case "zombieGrap":
                        Plugin.config.isZombieGrap = !Plugin.config.isZombieGrap;
                        break;
                    case "zombieJalaed":
                        Plugin.config.isZombieJalaed = !Plugin.config.isZombieJalaed;
                        break;
                    case "zombieDropGardenPlant":
                        dropPlants();
                        break;
                    case "ClickZombie":
                        Plugin.config.isClickZombie = !Plugin.config.isClickZombie;
                        break;
                    case "ClickZombie2":
                        Plugin.config.isClickZombie2 = !Plugin.config.isClickZombie2;
                        break;
                    case "ClickPlant":
                        Plugin.config.isClickPlant = !Plugin.config.isClickPlant;
                        break;
                    case "clickZombieType":
                        Plugin.config.clickZombieType= int.Parse(parts[1]);
                        break;
                    case "clickPlantType":
                        Plugin.config.clickPlantType= int.Parse(parts[1]);
                        break;
                    case "readBuffs":
                        SomeMethod.ReadBuffsToGame();
                        break;
                    case "loadMixData":
                        Plugin2.loadload();
                        break;
                    default:
                        Console.WriteLine("未知指令:" + methodName);
                        return "未知指令: " + methodName;
                }
            }

            return "指令格式错误";
        }
        catch (Exception ex)
        {
            return "处理指令时出错: " + ex.Message;
        }
    }

    public static void clearPlant()
    {
        foreach (Plant i in Board.Instance.plantArray)
        {
            if (i != null)
            {
                UnityEngine.Object.Destroy(i.gameObject);
            }
        }
    }

    public static void clearZombie()
    {
        foreach (Zombie i in Board.Instance.zombieArray)
        {
            if (i != null)
            {
                UnityEngine.Object.Destroy(i.gameObject);
            }
        }

    }

    public static void saveLineup(string fileName)
    {

        string targetPath = Path.Combine(Plugin.dllDirectory, "keep",fileName);
        string result = "";
        foreach (Plant i in Board.Instance.plantArray)
        {
            if (i != null)
            {
                result += "(" + i.thePlantColumn + "," + i.thePlantRow + "," + i.thePlantType + ")";
            }
        }
        File.WriteAllText(targetPath, result, Encoding.UTF8);
    }

    public static IEnumerator loadLineup(string fileName) {
        string targetPath = Path.Combine(Plugin.dllDirectory, "keep", fileName);
        string content = File.ReadAllText(targetPath);

        // 使用正则表达式匹配三元组中的数字
        Regex regex = new Regex(@"\((\d+),(\d+),(\d+)\)");
        MatchCollection matches = regex.Matches(content);

        // 用List存储所有数字
        List<int> numbers = new List<int>();


        foreach (Match match in matches)
        {
            for (int i = 1; i <= 3; i++)  // 每个三元组有三个数字
            {
                numbers.Add(int.Parse(match.Groups[i].Value));
            }
        }
        int[] numberArray = numbers.ToArray();
        Console.WriteLine(numberArray.Length);
        for (int i = 0; i+2 < numberArray.Length; i += 3)
        {
            yield return new WaitForSeconds(0.1f);
            CreatePlant.Instance.SetPlant(numberArray[i], numberArray[i+1], numberArray[i+2]);
        }

    }

    public static void dropPlants()
    {
        foreach (Zombie i in Board.Instance.zombieArray)
        {
            if (i != null)
            {
                i.DropGardenPlant();
            }
        }
    }

}

