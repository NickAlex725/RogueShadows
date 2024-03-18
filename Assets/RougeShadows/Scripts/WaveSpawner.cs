using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Spawner Info")]
    [SerializeField] private GameObject[] _enemyTypes;
    [SerializeField] private int[] _enemiesPerWaveAndNumberOfWaves;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _delayBetweenEnemies;
    [SerializeField] private int _secondsBetweenWaves;

    [Header("Wave SpawnerUI")]
    [SerializeField] private TextMeshProUGUI _currentWaveText;
    [SerializeField] private TextMeshProUGUI _enemiesRemainingText;
    [SerializeField] private TextMeshProUGUI _waveCountDownText;
    [SerializeField] private GameObject _victoryUI;

    private GameObject[] _enemiesToSpawn;
    private int _currentTimer;
    private int _enemiesLeft;
    private bool _isSpawning = false;
    private int _numberOfWaves;
    private int _currentWave = 0;
    private bool _allWavesComplete = false;

    private void Awake()
    {
        _numberOfWaves = _enemiesPerWaveAndNumberOfWaves.Length;
        _currentWaveText.text = (_currentWave + 1).ToString();
        _currentTimer = _secondsBetweenWaves;
        WaveSetUp();
    }

    private void WaveSetUp()
    {
        Debug.Log("Setting up: Wave " + (_currentWave + 1) + " and spawning " + _enemiesPerWaveAndNumberOfWaves[_currentWave] + " enemies");
        _currentWaveText.text = (_currentWave + 1).ToString();
        _enemiesToSpawn = new GameObject[_enemiesPerWaveAndNumberOfWaves[_currentWave]];
        StartCoroutine(WaveDelay());
    }

    private IEnumerator SpawnWithDelay()
    {
        _isSpawning = true;
        _enemiesRemainingText.text = _enemiesPerWaveAndNumberOfWaves[_currentWave].ToString();
        _enemiesLeft = _enemiesPerWaveAndNumberOfWaves[_currentWave];
        for (int i = 0; i < _enemiesPerWaveAndNumberOfWaves[_currentWave]; i++)
        {
            _enemiesToSpawn[i] = GrabRandomEnemyType();
            Instantiate(_enemiesToSpawn[i], GrabRandomSpawnPoint());
            if(i == _enemiesPerWaveAndNumberOfWaves[_currentWave] - 1)
            {
                break;
            }
            yield return new WaitForSeconds(_delayBetweenEnemies);
        }
        _isSpawning = false;
    }

    private IEnumerator WaveDelay()
    {
        _currentTimer = _secondsBetweenWaves;
        _waveCountDownText.gameObject.SetActive(true);
        for (int i = 0; i < _secondsBetweenWaves; i++)
        {
            _waveCountDownText.text = _currentTimer.ToString();
            yield return new WaitForSeconds(1);
            _currentTimer--;
        }
        _waveCountDownText.gameObject.SetActive(false);
        StartCoroutine(SpawnWithDelay());
    }

    private GameObject GrabRandomEnemyType()
    {
        var index = Random.Range(0, _enemyTypes.Length);
        return _enemyTypes[index];
    }

    private Transform GrabRandomSpawnPoint()
    {
        var index = Random.Range(0, _spawnPoints.Length);
        return _spawnPoints[index];
    }

    private void WaveEnd()
    {
        _currentWave++;
        if(_currentWave >= _numberOfWaves)
        {
            AllWavesCompelte();
        }
        else
        {
            WaveSetUp();
        }
    }

    private void AllWavesCompelte()
    {
        Debug.Log("Vicotry!");
        _allWavesComplete = true;
        _currentWaveText.gameObject.SetActive(false);
        _enemiesRemainingText.gameObject.SetActive(false);
        _victoryUI.gameObject.SetActive(true);
    }

    public void RemoveEnemy()
    {
        _enemiesLeft--;
        _enemiesRemainingText.text = _enemiesLeft.ToString();
        if (!_allWavesComplete && !_isSpawning && _enemiesLeft <= 0)
        {
            WaveEnd();
        }
    }
}