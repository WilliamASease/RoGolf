﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeeEnum;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreCard : MonoBehaviour
{
    public const string SCENE_NAME = "ScoreCardScene";

    private GameObject gcObject;

    public Text[] Hole = new Text[18];
    public Text[] Back = new Text[18];
    public Text[] Front = new Text[18];
    public Text[] Par = new Text[18];
    public Text[] Hcp = new Text[18];
    public Text[] Strokes = new Text[18];
    public Text[] Putts = new Text[18];
    public Text[] Fir = new Text[18];
    public Text[] Gir = new Text[18];
    public Text[] tot = new Text[18];
    
    void Start()
    {
        gcObject = GameObject.Find(GameController.NAME);
        Game game = gcObject.GetComponent<Game>();

        List<HoleData> holesPlayed = game.GetHoleBag().GetHolesPlayed();

        for(int i = 0; i < 18; i++)
        {
            Hole[i].text = (i+1).ToString();
            if (i < holesPlayed.Count)
            {
                HoleData h = holesPlayed[i];

                Back[i].text = MathUtil.ToYardsRounded(h.GetLengthBack()).ToString();
                Front[i].text = MathUtil.ToYardsRounded(h.GetLengthFront()).ToString();
                if (h.GetTee() == Tee.BACK) { Back[i].fontStyle = FontStyle.Bold; }
                else { Front[i].fontStyle = FontStyle.Bold; }
                Par[i].text = h.GetPar().ToString();
                Par[i].fontStyle = FontStyle.Bold;
                Hcp[i].text = "";
                Strokes[i].text = h.GetStrokes().ToString();
                Putts[i].text = h.GetPutts().ToString();
                Fir[i].text = h.GetFir() ? "X" : "";
                Gir[i].text = h.GetGir() ? "X" : "";
            }
            else
            {
                Back[i].text = "";
                Front[i].text = "";
                Par[i].text = "";
                Hcp[i].text = "";
                Strokes[i].text = "";
                Putts[i].text = "";
                Fir[i].text = "";
                Gir[i].text = "";
            }
        }

        foreach (Text t in tot) t.fontStyle = FontStyle.Bold;

        List<HoleData> front = holesPlayed.Take(9).ToList();
        tot[0].text = "OUT";
        tot[1].text = (front.Sum(h => MathUtil.ToYardsRounded(h.GetLengthBack()))).ToString();
        tot[2].text = (front.Sum(h => MathUtil.ToYardsRounded(h.GetLengthFront()))).ToString();
        tot[3].text = (front.Sum(h => h.GetPar())).ToString();
        tot[4].text = "AvgHcp";
        tot[5].text = (front.Sum(h => h.GetStrokes())).ToString();
        tot[6].text = (front.Sum(h => h.GetPutts())).ToString();
        tot[7].text = (front.Sum(h => h.GetFir() ? 1 : 0)).ToString();
        tot[8].text = (front.Sum(h => h.GetGir() ? 1 : 0)).ToString();

        List<HoleData> back = holesPlayed.Skip(9).Take(9).ToList();
        tot[9].text = "IN";
        tot[10].text = (back.Sum(h => MathUtil.ToYardsRounded(h.GetLengthBack()))).ToString();
        tot[11].text = (back.Sum(h => MathUtil.ToYardsRounded(h.GetLengthFront()))).ToString();
        tot[12].text = (back.Sum(h => h.GetPar())).ToString();
        tot[13].text = "AvgHcp";
        tot[14].text = (back.Sum(h => h.GetStrokes())).ToString();
        tot[15].text = (back.Sum(h => h.GetPutts())).ToString();
        tot[16].text = (back.Sum(h => h.GetFir() ? 1 : 0)).ToString();
        tot[17].text = (back.Sum(h => h.GetGir() ? 1 : 0)).ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            // Load shop scene
            SceneManager.LoadScene(ShopController.SCENE_NAME);
        }
    }
}
