using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class cs : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture rt1;
    public RenderTexture rt2;
    private Texture2D inputState;

    public TextAsset dictionaryTextFile;
    private string theWholeFileAsOneLongString;
    private List<string> eachLine;

    private bool swap;

    void readText() {
        theWholeFileAsOneLongString = dictionaryTextFile.text;

        eachLine = new List<string>();
        eachLine.AddRange(theWholeFileAsOneLongString.Split("\n"[0]));



        for (int y = 37; y < 565; y++)
        {
            for (int x = 36; x < 764; x++)
            {
                if (eachLine[y][x] == '#')
                {
                    inputState.SetPixel(x - 36, 565 - y, new Color(255, 255, 0));
                }
                else if (eachLine[y][x] == '@')
                {
                    inputState.SetPixel(x - 36, 565 - y, new Color(0, 0, 255));
                }
                else if (eachLine[y][x] == '~')
                {
                    inputState.SetPixel(x - 36, 565 - y, new Color(255, 0, 0));
                }
                else
                {
                    inputState.SetPixel(x - 36, 565 - y, new Color(0, 0, 0));
                }
            }
        }
        inputState.Apply();
    }

    void Start()
    {
        swap = false;

        rt1 = new RenderTexture(728, 528, 24);
        rt2 = new RenderTexture(728, 528, 24);
        rt1.enableRandomWrite = true;
        rt2.enableRandomWrite = true;
        rt1.Create();
        rt2.Create();

        inputState = new Texture2D(728, 528);
        readText();

        Graphics.Blit(inputState, rt2);

        computeShader.SetTexture(0, "Read", rt2);
        computeShader.SetTexture(0, "Write", rt1);
        computeShader.Dispatch(0, 26, 16, 1);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (swap)
            Graphics.Blit(rt2, dest);
        else
            Graphics.Blit(rt1, dest);
    }

    int updatecount = 0;

    float t = 0f;

    void Update()
    {
        t += Time.deltaTime;

        for (int i = 0; i < 1501; i++)
        {
            //if (t >= 0.2f)
            //{
                //t -= 0.2f;
                if (swap)
                {
                    computeShader.SetTexture(0, "Read", rt2);
                    computeShader.SetTexture(0, "Write", rt1);
                    computeShader.Dispatch(0, 26, 16, 1);
                }
                else
                {
                    computeShader.SetTexture(0, "Read", rt1);
                    computeShader.SetTexture(0, "Write", rt2);
                    computeShader.Dispatch(0, 26, 16, 1);
                }

                swap = !swap;


                updatecount++;
            //}
        }

        string ips = ((double)updatecount / Time.timeAsDouble).ToString("F2");
        string time = Time.timeAsDouble.ToString("F0");
        UnityEngine.Debug.Log("ips: " + ips + "\n time: " + time + " seconds");
    }
}
