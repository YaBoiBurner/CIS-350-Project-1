﻿/*
 * Robert Krawczyk, Gerard Lamoureux, Jaden Pleasants, Conner Ogle
 * Project1
 * Just knows which slot it's in, creates random name
 */
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Alien : MonoBehaviour
{
    // This script so far only covers dragging and dropping the alien.
    // You could check 'slot' for what resources the planet/ark slot has, to make this alien die or something :)
    private AttributeStorage.Terrain terrain;
    public AttributeStorage.Terrain Terrain => terrain;

    private AttributeStorage.Temperature temperature;
    public AttributeStorage.Temperature Temperature => temperature;

    private AttributeStorage.Resource resource;
    public AttributeStorage.Resource Resource => resource;

    private AttributeStorage.Atmosphere atmosphere;
    public AttributeStorage.Atmosphere Atmosphere => atmosphere;

    public Slot slot;

    //Just a QOL thing, Give each species a randomly generated name.
    private string species;
    public string SpeciesName => species;

    private TurnManager TM;

    // Start is called before the first frame update
    void Start()
    {
        species = MakeRandomName();
        // Register health updates with the turn update event.
        TM = TurnManager.GetTurnManager();
        TM.TurnEvent.AddListener(UpdateHealth);
        // Get random attributes
        terrain = AttributeStorage.Shuffle<AttributeStorage.Terrain>(AttributeStorage.PlanetTerrains).First();
        temperature = AttributeStorage.Shuffle<AttributeStorage.Temperature>(AttributeStorage.PlanetTemps).First();
        resource = AttributeStorage.Shuffle<AttributeStorage.Resource>(AttributeStorage.Resources).First();
        atmosphere = AttributeStorage.Shuffle<AttributeStorage.Atmosphere>(AttributeStorage.Atmospheres).First();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateHealth()
    {
        // TODO: Update health based on things like current conditions
    }

    void OnDestroy()
    {
        TM.TurnEvent.RemoveListener(UpdateHealth);
    }

    //Generate a random name.
    string MakeRandomName()
    {
        string name = "";
        name += RandomConsonant().ToString().ToUpper();
        for (int i = 1; i < Random.Range(3, 10); i++)
        {
            name += (i % 2 == 0) ? RandomConsonant() : RandomVowel();
        }
        return name;
    }
    private static readonly char[] Consonants = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };
    private static readonly char[] Vowels = { 'a', 'e', 'i', 'o', 'u' };
    private char RandomConsonant() => Consonants[Random.Range(0, Consonants.Length)];
    private char RandomVowel() => Vowels[Random.Range(0, Vowels.Length)];
}
