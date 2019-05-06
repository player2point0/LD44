using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class WolfController : Agent
{
    public float speed;

    private SheepDogController sheepDog;
    private HerdManager herdManager;
    private float previousSheepDistance;
    private Vector3 startPos;

    public override void InitializeAgent()
    {
        sheepDog = FindObjectOfType<SheepDogController>();
        herdManager = FindObjectOfType<HerdManager>();
        previousSheepDistance = 100;
        startPos = this.transform.position;
    }

    public override void CollectObservations()
    {
        Vector3 closestSheep = closestSheepPos(herdManager.sheeps);

        AddVectorObs(closestSheep);//+3
        AddVectorObs(sheepDog.transform.position);//+3
        //AddVectorObs(this.transform.position);//+3
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        float reward = 0;

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
        {
            float x = Mathf.Clamp(vectorAction[0], -1f, 1f);
            float y = Mathf.Clamp(vectorAction[1], -1f, 1f);

            //move
            Vector2 move = new Vector2(x, y);
            this.transform.Translate(move * speed);
            lookAtPos(move);
        }
        
        Vector3 closestSheep = closestSheepPos(herdManager.sheeps);

        float sheepDis = disTo(closestSheep);
        float sheepDogDis = disTo(sheepDog.transform.position);

        //eat sheep
        if (sheepDis < 1)
        {
            reward += 1f;
        }

        else
        {
            reward += -0.001f;
        }

        //encourage getting closer
        if(sheepDis < previousSheepDistance)
        {
            reward += 0.01f;
        }

        previousSheepDistance = sheepDis;

        //starved
        if(sheepDis > 20)
        {
            //Done();
            reward += -1f;
        }

        //bitten
        if (sheepDogDis < 1)
        {
            //Done();
            reward += -1f;
        }

        SetReward(reward);
    }

    public override void AgentReset()
    {
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

        Vector2 pos = Random.insideUnitCircle;

        this.transform.position = pos + (Vector2)startPos; 
    }

    private Vector3 closestSheepPos(List<SheepAgent> posArr)
    {
        float min = 10000;
        Vector2 output = this.transform.position;

        for (int i = 0; i < posArr.Count; i++)
        {
            Vector3 pos = posArr[i].transform.position;
            float dis = Vector3.Distance(pos, this.transform.position);

            if (Mathf.Abs(dis) < min)
            {
                min = dis;
                output = pos;
            }
        }

        return output;
    }

    private float disTo(Vector3 pos)
    {
        return Mathf.Abs(Vector3.Distance(this.transform.position, pos));
    }

    void lookAtPos(Vector2 pos)
    {
        float rot_z = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }

}
