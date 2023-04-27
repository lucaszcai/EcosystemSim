using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genes
{
    public bool male;
    
    // stores trait and its value
    public Dictionary<string, float> values;
    static System.Random prng = new System.Random();

    public Genes(Dictionary<string, float> values)
    {
        male = RandomValue() < 0.5f;
        this.values = values;
    }

    public Genes(string[] traits, float[] defaults)
    {
        values = new Dictionary<string, float>();
        male = RandomValue() < 0.5f;

        for(int i = 0; i < traits.Length; i++)
        {
            values[traits[i]] = Random.Range(0.5f, 1.5f) * defaults[i];
        }

        Debug.Log("values " + values);
    }

    // constructor with parents
    public Genes(Genes mother, Genes father)
    {

    }

    public float getVal(string trait)
    {
        return values[trait];
    }

    public bool getMale()
    {
        return male;
    }

    //public static Genes RandomGenes(int num)
    //{
    //    //Dictionary<string, float> values = new float[num];
    //    //for (int i = 0; i < num; i++)
    //    //{
    //    //    values[i] = RandomValue();
    //    //}
    //    //return new Genes(values);
    //}

    static float RandomValue()
    {
        return (float)prng.NextDouble();
    }

    static float RandomGaussian()
    {
        float u1 = (float)(1 - prng.NextDouble());
        float u2 = (float)(1 - prng.NextDouble());
        double randStdNormal = Mathf.Sqrt(-2 * Mathf.Log(u1)) * Mathf.Sin(2 * Mathf.PI * u2);
        return (float)randStdNormal;
    }
}
