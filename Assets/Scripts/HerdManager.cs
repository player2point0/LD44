using System.Collections;
using System.Collections.Generic;
using MLAgents;
using TMPro;
using UnityEngine;

public class HerdManager : MonoBehaviour
{
    public bool trainMode;
    public SheepAgent[] trainingSheep;
    public float breedCooldown;
    public GameObject SheepObj;
    public Brain SheepBrain;
    public TextMeshProUGUI breedButtonText;

    [HideInInspector]public List<SheepAgent> sheeps = new List<SheepAgent>();
    private List<SheepAgent> breedingSheep = new List<SheepAgent>();
    private SheepDogController sheepDog;
    private float breedDelay;
    private bool readyToBread;

    void Start()
    {
        sheepDog = FindObjectOfType<SheepDogController>();

        if (!trainMode)
        {
            createRandomSheepAgent(new Vector3(1, 0, 0));
            createRandomSheepAgent(new Vector3(1.3f, 0.3f, 0));
            createRandomSheepAgent(new Vector3(0, 1, 0));
            createRandomSheepAgent(new Vector3(-0.2f, -0.4f, 0));
            breedDelay = breedCooldown;
            readyToBread = false;
        }

        else sheeps = new List<SheepAgent>(trainingSheep);

    }

    private void FixedUpdate()
    {
        if(!trainMode)
        {
            if (!readyToBread)
            {
                if (breedDelay < 0)
                {
                    breedButtonText.text = "Breed Selected";
                    readyToBread = true;
                }

                else
                {
                    breedDelay -= Time.deltaTime;
                    breedButtonText.text = breedDelay.ToString("0.");
                }
            }
        }
    }
    
    public bool sheepTouchingOtherSheep(float radius, SheepAgent sheep)
    {
        for(int i = 0;i<sheeps.Count;i++)
        {
            if (sheep == sheeps[i]) continue;

            float dis = Vector3.Distance(sheep.transform.position, sheeps[i].transform.position);

            if (Mathf.Abs(dis) < radius) return true;
        }

        return false;
    }

    public void breedSelectedSheep()
    {
        if(readyToBread && breedingSheep.Count >= 2)
        {
            Debug.Log(breedingSheep[0].transform.position);
            breedSheep(breedingSheep[0], breedingSheep[1], new Vector3(0,0,0));
            readyToBread = false;
            breedDelay = breedCooldown;

            removeAndHideSheep(breedingSheep[0]);
            removeAndHideSheep(breedingSheep[0]);

            Time.timeScale = 1;
        }
    }

    public void addBreedingSheep(SheepAgent sheep)
    {
        breedingSheep.Add(sheep);

        if(breedingSheep.Count > 2)
        {
            SheepAgent temp = breedingSheep[0];
            removeAndHideSheep(temp);
        }

        if(breedingSheep.Count > 0)
        {
            if(!breedingSheep[0].displayLeft)
            {
                sheep.statsPanel.transform.localPosition = new Vector3(50, -50, 0);
                sheep.displayLeft = true;
            }

            else
            {
                sheep.statsPanel.transform.localPosition = new Vector3(-50, -50, 0);
                sheep.displayLeft = false;
            }
        }

        //pause game
        Time.timeScale = 0;

    }

    private void removeAndHideSheep(SheepAgent sheep)
    {
        sheep.selected = false;
        sheep.displayLeft = false;
        sheep.statsPanel.SetActive(false);

        breedingSheep.Remove(sheep);
    }

    public void removeBreedingSheep(SheepAgent sheep)
    {
        breedingSheep.Remove(sheep);

        //unpause
        if (breedingSheep.Count == 0) Time.timeScale = 1;
    }

    public void killSheep(SheepAgent sheep)
    {
        sheeps.Remove(sheep);
        breedingSheep.Remove(sheep);
        Destroy(sheep.gameObject);
    }

    private void breedSheep(SheepAgent sheepA, SheepAgent sheepB, Vector3 pos)
    {
        //take a random attribute from each parent
        GameObject AgentObj = Instantiate(SheepObj, pos, Quaternion.identity, this.transform);
        SheepAgent tempSheep = AgentObj.GetComponent<SheepAgent>();

        if (Random.Range(0.0f, 1.0f) > 0.5) tempSheep.healthMulti = sheepA.healthMulti;
        else tempSheep.healthMulti = sheepB.healthMulti;

        if (Random.Range(0.0f, 1.0f) > 0.5) tempSheep.hungerMulti = sheepA.hungerMulti;
        else tempSheep.hungerMulti = sheepB.hungerMulti;

        if (Random.Range(0.0f, 1.0f) > 0.5) tempSheep.thirstMulti = sheepA.thirstMulti;
        else tempSheep.thirstMulti = sheepB.thirstMulti;

        if (Random.Range(0.0f, 1.0f) > 0.5) tempSheep.speedMulti = sheepA.speedMulti;
        else tempSheep.speedMulti = sheepB.speedMulti;

        if (Random.Range(0.0f, 1.0f) > 0.5) tempSheep.deductMulti = sheepA.deductMulti;
        else tempSheep.deductMulti = sheepB.deductMulti;

        if (Random.Range(0.0f, 1.0f) > 0.5) tempSheep.absorbMulti = sheepA.absorbMulti;
        else tempSheep.absorbMulti = sheepB.absorbMulti;
        
        if (Random.Range(0.0f, 1.0f) > 0.5) tempSheep.interactDisMulti = sheepA.interactDisMulti;
        else tempSheep.interactDisMulti = sheepB.interactDisMulti;

        tempSheep.setAtrributes();
        tempSheep.GiveBrain(SheepBrain);

        tempSheep.startPos = pos;

        tempSheep.AgentReset();

        sheeps.Add(tempSheep);
    }

    private void createRandomSheepAgent(Vector3 pos)
    {
        GameObject AgentObj = Instantiate(SheepObj, pos, Quaternion.identity, this.transform);
        SheepAgent tempSheep = AgentObj.GetComponent<SheepAgent>();

        tempSheep.randomAttributeMultis();
        tempSheep.setAtrributes();
        tempSheep.GiveBrain(SheepBrain);
        tempSheep.AgentReset();

        sheeps.Add(tempSheep);
    }
}
