using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Bombite_FSM _bombite;
    [SerializeField] private Leever_FSM _leever;
    [SerializeField] private SpikedBeetle_FSM _spikedBeetle;
    [SerializeField] private Stalfos_FSM _stalfos;
    
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
