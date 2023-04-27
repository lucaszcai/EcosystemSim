using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class Logger : MonoBehaviour
{
    public string csvFileName = "population_log.csv";
    private StreamWriter streamWriter;

    void Start()
    {
        streamWriter = new StreamWriter(csvFileName);
        streamWriter.WriteLine("Timestamp,Population");
    }

    void Update()
    {
        //int totalPopulation = GetTotalPopulation();
        //string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //streamWriter.WriteLine(timestamp + "," + totalPopulation);
        //streamWriter.Flush();
    }

    public void writePopulation(int population)
    {
        float timestamp = Time.time;
        streamWriter.WriteLine(timestamp + "," + population);
        streamWriter.Flush();
    }

    void OnApplicationQuit()
    {
        streamWriter.Close();
    }

    //int GetTotalPopulation()
    //{
    //    // Calculate and return the total population here
    //}
}