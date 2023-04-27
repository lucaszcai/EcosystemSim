using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class Logger : MonoBehaviour
{
    public string csvFileName = "population_log.csv";
    public string traitsFileName = "trait_log.csv";
    private StreamWriter streamWriter;

    private StreamWriter traitStreamWriter;

    void Start()
    {
        streamWriter = new StreamWriter(csvFileName);
        traitStreamWriter = new StreamWriter(traitsFileName);
        streamWriter.WriteLine("Timestamp,Population");

        traitStreamWriter.WriteLine("Timestamp,Speed,Sensing");
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

    public void writeTraits(float speed, float sensing)
    {
        float timestamp = Time.time;
        traitStreamWriter.WriteLine(timestamp + ","+ speed + "," + sensing);
        traitStreamWriter.Flush();

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