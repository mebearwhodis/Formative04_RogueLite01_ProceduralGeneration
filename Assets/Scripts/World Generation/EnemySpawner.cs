using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Bombite_FSM _bombite;
    [SerializeField] private Leever_FSM _leever;
    [SerializeField] private SpikedBeetle_FSM _spikedBeetle;
    [SerializeField] private Stalfos_FSM _stalfos;
    [SerializeField] private int _bombiteCost = 6;
    [SerializeField] private int _leeverCost = 3;
    [SerializeField] private int _spikedBeetleCost = 8;
    [SerializeField] private int _stalfosCost = 3;

    public int SpawnMonster(int randomNumber)
    {
        //Define a range for random positions
        float rangeX = 5.0f;
        float rangeY = 5.0f;

        //Random offsets within the defined range
        float randomOffsetX = Random.Range(-rangeX, rangeX);
        float randomOffsetY = Random.Range(-rangeY, rangeY);

        //Calculate the random position relative to the object's position
        Vector3 randomPosition = transform.position + new Vector3(randomOffsetX, randomOffsetY, 0);

        //Instantiate the object at the random position
        switch (randomNumber)
        {
            case 0:
                Instantiate(_bombite, randomPosition, Quaternion.identity);
                return _bombiteCost;
            case 1:
                Instantiate(_leever, randomPosition, Quaternion.identity);
                return _leeverCost;
            case 2:
                Instantiate(_spikedBeetle, randomPosition, Quaternion.identity);
                return _spikedBeetleCost;
            case 3:
                Instantiate(_stalfos, randomPosition, Quaternion.identity);
                return _stalfosCost;
            default:
                return 0;
        }
    }

//Individual spawn functions for Testing purposes
    public void SpawnBombite()
    {
        Instantiate(_bombite, transform.position, Quaternion.identity);
    }

    public void SpawnLeever()
    {
        Instantiate(_leever, transform.position, Quaternion.identity);
    }

    public void SpawnSpikedBeetle()
    {
        Instantiate(_spikedBeetle, transform.position, Quaternion.identity);
    }

    public void SpawnStalfos()
    {
        Instantiate(_stalfos, transform.position, Quaternion.identity);
    }
}