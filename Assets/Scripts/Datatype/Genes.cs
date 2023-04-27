using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genes
{
    public bool male;

    const float mutationFactor = 0.3f;
    
    // stores trait and its value
    public Dictionary<string, float> values;
    static System.Random prng = new System.Random();
    string[] traitStrings;

    public Genes(Dictionary<string, float> values)
    {
        male = RandomValue() < 0.5f;
        this.values = values;
    }

    public Genes(string[] traits, float[] defaults)
    {
        traitStrings = traits;
        values = new Dictionary<string, float>();
        male = RandomValue() < 0.5f;

        for(int i = 0; i < traits.Length; i++)
        {
            values[traits[i]] = Random.Range(0.5f, 1.5f) * defaults[i];
        }

        //Debug.Log("values " + values);
    }

    // constructor with parents
    public Genes(string[] traits, float[] defaults, Genes mother, Genes father)
    {
        values = new Dictionary<string, float>();
        male = RandomValue() < 0.5f;

        for(int i = 0; i < traits.Length; i++)
        {
            // 50/50 chance of taking trait from mother or father
            if(RandomValue() < 0.5f)
            {
                // take from mother
                values[traits[i]] = mother.getVal(i);
            }
            else
            {
                values[traits[i]] = father.getVal(i);
            }

            // add a random amount to gene
            float mutAmt = RandomGaussian() * mutationFactor;
            values[traits[i]] += mutAmt;

            Debug.Log("mutation amount: " + mutAmt);
        }

    }


    public float getVal(string trait)
    {
        return values[trait];
    }

    public float getVal(int index)
    {
        return values[traitStrings[index]];
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
