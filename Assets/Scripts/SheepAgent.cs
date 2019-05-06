using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MLAgents;
using UnityEngine;
using TMPro;

public class SheepAgent : Agent
{
    [Header("UI")]
    public GameObject statsPanel;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI thirstText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI deductText;
    public TextMeshProUGUI absorbText;
    public TextMeshProUGUI interactText;
    public bool selected = false;
    public bool displayLeft = false;

    [Header("Specific to Sheep")]
    public float stepReward;

    [Header("Attributes")]
    public float healthMulti = 1;
    public float hungerMulti = 1;
    public float thirstMulti = 1;
    public float speedMulti = 1;
    public float deductMulti = 1;
    public float absorbMulti = 1;
    public float interactDisMulti = 1;

    public Vector3 startPos;
    private float maxHealth;
    private float maxHunger;
    private float maxThirst;

    private float baseHealth = 100;
    private float baseHunger = 100;
    private float baseThirst = 100;
    private float baseSpeed = 0.05f;
    private float baseDeductRate = 0.05f;
    private float baseAbsorbRate = 0.5f;
    private float baseInteractDis = 1;

    public string sheepName = "Dolly";
    public float health = 100;
    public float hunger = 100;
    public float thirst = 100;
    private float speed = 0.04f;
    private float deductRate = 0.25f;
    private float absorbRate = 0.5f;
    private float interactDis = 1;

    private float previousGrassDis = 100;
    private float previousWaterDis = 100;

    private GameObject[] grasses;
    private GameObject[] waters;
    private GameObject[] wolves;
    private SheepDogController sheepDog;
    private HerdManager herdManager;

    private void Start()
    {
        herdManager = FindObjectOfType<HerdManager>();
    }

    public override void InitializeAgent()
    {
        startPos = this.transform.position;
        grasses = GameObject.FindGameObjectsWithTag("Grass");
        waters = GameObject.FindGameObjectsWithTag("Water");
        wolves = GameObject.FindGameObjectsWithTag("Wolf");

        sheepDog = FindObjectOfType<SheepDogController>();
    }

    public override void CollectObservations()
    {
        Vector2 closestGrass = closestPos(grasses);
        Vector2 closestWater = closestPos(waters);
        Vector2 closestWolf = closestPos(wolves);

        AddVectorObs(closestGrass);//+2
        AddVectorObs(closestWater);//+2
        AddVectorObs(closestWolf);//+2

        AddVectorObs(sheepDog.transform.position);//+3
        AddVectorObs(this.transform.position);//+3

        //percentage variable stats
        float healthPercentage = health / maxHealth;
        float hungerPercenatge = hunger / maxHunger;
        float thirstPercentage = thirst / maxThirst;

        AddVectorObs(healthPercentage);//+1
        AddVectorObs(hungerPercenatge);//+1
        AddVectorObs(thirstPercentage);//+1
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float reward = 0;
        //prioritise what is most urgent
        float hungerTargetMulti = 1;
        float thirstTargetMulti = 1;

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
        {
            float x = Mathf.Clamp(vectorAction[0], -1f, 1f);
            float y = Mathf.Clamp(vectorAction[1], -1f, 1f);

            statusText.text = "";

            //move
            Vector2 move = new Vector2(x, y);
            this.transform.Translate(move * speed);
            //deduct hunger
            if(hunger > 0)
            {
                hunger -= deductRate;
            }
            //deduct thirst
            if(thirst > 0)
            {
                thirst -= deductRate;
            }

            if (hunger < 0)
            {
                //straving
                health -= deductRate;
                reward += -0.1f;
                statusText.text = "Hungry";
            }

            if (thirst < 0)
            {
                //dehydrated
                health -= deductRate;
                reward += -0.1f;
                statusText.text = "Thirsty";
            }

            if(hunger < 0 && thirst < 0)
            {
                statusText.text = "Dying";
            }
        }

        if (health < 0)
        {
            Done();
            reward += -1f;
        }
        else
        {
            Vector2 closestGrass = closestPos(grasses);
            Vector2 closestWater = closestPos(waters);
            Vector2 closestWolf = closestPos(wolves);

            float grassDis = disTo(closestGrass);
            float grassReward = (1 / grassDis);
            float waterDis = disTo(closestWater);
            float waterReward = (1 / waterDis);
            float wolfDis = disTo(closestWolf);
            float sheepDogDis = disTo(sheepDog.transform.position);

            float hungerPercenatge = hunger / maxHunger;
            float thirstPercentage = thirst / maxThirst;

            //prioritise what is most urgent
            if (hungerPercenatge < thirstPercentage)
            {
                hungerTargetMulti = 2;
                //grass reward - closer = larger reward
                reward += (Mathf.Clamp(grassReward, 0, 0.5f)) * hungerTargetMulti;
            }
            else
            {
                //defaults to water
                thirstTargetMulti = 2;
                //water reward - closer = larger reward
                reward += (Mathf.Clamp(waterReward, 0, 0.5f)) * thirstTargetMulti;

            }

            //wolf bitten punishment
            if (wolfDis < interactDis)
            {
                Done();
                reward += -1f;
            }
                     
            //punish completely running off
            if (sheepDogDis > (50 * interactDis))
            {
                Done();
                reward += -1f;
            }
            
            //eat
            if(grassDis < interactDis)
            {
                if (hunger < (maxHunger * 0.9f))
                {
                    hunger += absorbRate;
                    reward += (0.5f) * hungerTargetMulti;
                }
                else hunger = maxHunger;
            }
            
            //drink
            if(waterDis < interactDis)
            {
                if (thirst < (maxThirst * 0.9f))
                {
                    thirst += absorbRate;
                    reward += (0.5f) * thirstTargetMulti;
                }
                else thirst = maxThirst;
            }

            //health regen
            if((thirst > (maxThirst * 0.9f)) && (hunger > (maxHunger * 0.9f)) && health < maxHealth)
            {
                health += absorbRate;
                reward += 0.2f;
            }
        }

        SetReward(reward);

        stepReward = reward;
    }

    public override void AgentReset()
    {
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        this.transform.position = sheepDog.transform.position + startPos;

        randomAttributeMultis();
        setAtrributes();

        health = maxHealth;
        hunger = maxHunger;
        thirst = maxThirst;
    }

    public void randomAttributeMultis()
    {
        healthMulti = Random.Range(0.1f, 1.9f);
        hungerMulti = Random.Range(0.1f, 1.9f);
        thirstMulti = Random.Range(0.1f, 1.9f);
        speedMulti = Random.Range(0.1f, 1.9f);
        deductMulti = Random.Range(0.1f, 1.9f);
        absorbMulti = Random.Range(0.1f, 1.9f);
        interactDisMulti = Random.Range(0.1f, 1.9f);
    }

    public void setAtrributes()
    {
        health = baseHealth * Mathf.Clamp(healthMulti, 0.5f, 1.5f);
        maxHealth = baseHealth * Mathf.Clamp(healthMulti, 0.5f, 1.5f);
        hunger = baseHunger * Mathf.Clamp(hungerMulti, 0.5f, 1.5f);
        maxHunger = baseHunger * Mathf.Clamp(hungerMulti, 0.5f, 1.5f);
        thirst = baseThirst * Mathf.Clamp(thirstMulti, 0.5f, 1.5f);
        maxThirst = baseThirst * Mathf.Clamp(thirstMulti, 0.5f, 1.5f);

        speed = baseSpeed * Mathf.Clamp(speedMulti, 0.5f, 1.5f);
        deductRate = baseDeductRate * Mathf.Clamp(deductMulti, 0.5f, 1.5f);
        absorbRate = baseAbsorbRate * Mathf.Clamp(absorbMulti, 0.5f, 1.5f);
        interactDis = baseInteractDis * Mathf.Clamp(interactDisMulti, 0.5f, 1.5f);

        nameText.text = genRandomName();
        healthText.text = "Health: " + statsPlusMinus(healthMulti);
        hungerText.text = "Hunger: " + statsPlusMinus(hungerMulti);
        thirstText.text = "Thirst: " + statsPlusMinus(thirstMulti);
        speedText.text = "Speed: " + statsPlusMinus(speedMulti);
        deductText.text = "Deduct: " + statsPlusMinus(deductMulti);
        absorbText.text = "Absorb: " + statsPlusMinus(absorbMulti);
        interactText.text = "Interact: " + statsPlusMinus(absorbMulti);
    }

    private string statsPlusMinus(float stat)
    {
        if (stat > 1.5f) return "++";
        if (stat > 1f) return "+";
        if (stat > 0.5f) return "-";
        else return "--";
    }


    private string genRandomName()
    {
        string[] names1 = new string[] { "Snowy", "Snow White", "Daisy", "Snowflake", "Cloud", "Milky", "Cream", "Snowball", "John", "John", "John", "Dreamy" };
        string[] names2 = new string[] { "Algernon", "Dolly", "Shelly", "Molly", "Kelly", "Nelly", "Bob", "Steve", "Joe", "Emma", "Emai", "Lauren", "John" };

        int index1 = Random.Range(0, names1.Length);
        int index2 = Random.Range(0, names2.Length);

        return names1[index1] + "-" + names2[index2];
    }

    public override void AgentOnDone()
    {
        herdManager.killSheep(this);
    }

    private void OnMouseUpAsButton()
    {
        if(!selected)
        {
            herdManager.addBreedingSheep(this);
        }

        else
        {
            herdManager.removeBreedingSheep(this);
        }

        selected = !selected;
        statsPanel.SetActive(!statsPanel.activeSelf);
    }

    private Vector2 closestPos(GameObject[] posArr)
    {
        float min = 10000;
        Vector2 output = this.transform.position;

        for(int i = 0;i<posArr.Length;i++)
        {
            Vector3 pos = posArr[i].transform.position;
            float dis = Vector3.Distance(pos, this.transform.position);

            if(Mathf.Abs(dis) < min)
            {
                min = dis;
                output = (Vector2)pos;
            }
        }

        return output;
    }

    private float disTo(Vector2 pos)
    {
        return Mathf.Abs(Vector2.Distance(this.transform.position, pos));
    }
}
