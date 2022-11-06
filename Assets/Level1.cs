using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Level1 : MonoBehaviour
{
    public int ideaCount = 10;

    public GameObject ideaPrefab;

    public List<Idea> curIdeas;

    public List<TMP_FontAsset> fonts;

    public string adj, noun;

    private string[] splitAdj, splitNoun;

    // Start is called before the first frame update
    void Start()
    {
        splitAdj = adj.Split(new[] { " " },  System.StringSplitOptions.None);

        splitNoun = noun.Split(new[] { " " }, System.StringSplitOptions.None);

        for (int i = 0; i < ideaCount; i++)
        {
            var p = Instantiate(ideaPrefab).GetComponent<Idea>();


            //This is bad but ¯\_(?)_/¯
            Quaternion newRotation = Random.rotation;

            foreach(Idea idea in curIdeas)
            {
                if (Quaternion.Angle(idea.transform.rotation, newRotation) < 90)
                {
                    newRotation = Random.rotation;                
                }
            }

            p.transform.rotation = newRotation;

            float forwardMulti = Random.Range(3f, 6f);

            p.transform.position += p.transform.forward * forwardMulti;

            p.size = Random.Range(2f, 5f);

            p.curTextMat = new Material(p.curTextMat);

            p.curTextMat.SetFloat("_HueOffset", (float) i / ideaCount);

            p.textField.font = fonts[Random.Range(0, fonts.Count)];

            string sonet = splitAdj[Random.Range(0, splitAdj.Length)] + " " + splitNoun[Random.Range(0, splitNoun.Length)];

            p.displayString = sonet;

            curIdeas.Add(p);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
