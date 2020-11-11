using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Reflection;

public class LevelDecoder : MonoBehaviour {
    public TextAsset xml;
    public List<Level> levels;
    LevelManager manager;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public Level GetLevel(int id) {
        return levels[id];
    }

    public int LevelCount() {
        return levels.Count;
    }

    public void GetLevels() {
        manager = GetComponent<LevelManager>();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml.text);
        XmlNodeList levelsList = xmlDoc.GetElementsByTagName("level");

        foreach (XmlNode levelInfo in levelsList) {

            XmlNodeList levelcontent = levelInfo.ChildNodes;
            Level lvl = new Level();

            foreach (XmlNode levelsItens in levelcontent) // levels itens nodes.
            {
                if (levelsItens.Name == "title") {
                    lvl.title = levelsItens.InnerText;
                } else if (levelsItens.Name == "description") {
                    lvl.description = levelsItens.InnerText;
                } else if (levelsItens.Name == "light_description_color") {
                    lvl.light_description_color = bool.Parse(levelsItens.InnerText);
                } else if (levelsItens.Name == "valuetoreach") {
                    lvl.valuetoreach = int.Parse(levelsItens.InnerText);
                } else if (levelsItens.Name == "backgroundchangeevery") {
                    lvl.backgroundchangeevery = int.Parse(levelsItens.InnerText);
                } else if (levelsItens.Name == "beamspeed") {
                    lvl.beamspeed = float.Parse(levelsItens.InnerText);
                } else if (levelsItens.Name == "boostspeed") {
                    lvl.boostspeed = float.Parse(levelsItens.InnerText);
                } else if (levelsItens.Name == "background") {
                    lvl.background = int.Parse(levelsItens.InnerText);
                } else if (levelsItens.Name == "endmessage") {
                    lvl.endMessage = levelsItens.InnerText;
                } else if (levelsItens.Name == "achievement") {
                    lvl.achievement = levelsItens.InnerText;
                } else if (levelsItens.Name == "reversed") {
                    lvl.reverse = bool.Parse(levelsItens.InnerText);
                } else if (levelsItens.Name == "enhanced") {
                    lvl.enhanced = bool.Parse(levelsItens.InnerText);
                } else if (levelsItens.Name == "coins") {
                    lvl.coins = int.Parse(levelsItens.InnerText);
                } else if (levelsItens.Name == "ground") {
                    lvl.ground = int.Parse(levelsItens.InnerText);
                }

            }
            levels.Add(lvl);
        }
    }

}


[Serializable]
public struct Level {
    public string title { get; set; }
    public string description { get; set; }
    public bool light_description_color { get; set; }
    public int valuetoreach { get; set; }
    public int backgroundchangeevery { get; set; }
    public float beamspeed { get; set; }
    public float boostspeed { get; set; }
    public int background { get; set; }
    public int coins { get; set; }
    public string endMessage { get; set; }
    public string achievement { get; set; }
    public bool reverse { get; set; }
    public bool enhanced { get; set; }
    public int ground { get; set; }
}

