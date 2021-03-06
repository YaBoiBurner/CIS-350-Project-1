/*
 * Robert Krawczyk, Gerard Lamoureux, Jaden Pleasants, Conner Ogle
 * Project1
 * Just knows which slot it's in, creates random name
 */
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    private int health;
    public int Health => health;

    public static readonly int MAX_HAPPINESS = 5;
    private int happiness;
    public int Happiness => happiness;

    private GameObject[] planets;

    private Slot slot;
    public Slot Slot
    {
        get => slot; set
        {
            slot = value;
            UpdateHappiness(); // small assurance that happiness is updated
        }
    }

    //Just a QOL thing, Give each species a randomly generated name.
    private string species;
    public string SpeciesName => species;

    public List<Sprite> AlienSprites;

    public SpriteRenderer spriteRenderer;

    private TurnManager TM;

    public GameObject healthLostTextPrefab;

    // Start is called before the first frame update
    void Start()
    {
        species = MakeRandomName();
        // Register health updates with the turn update event.
        TM = TurnManager.GetTurnManager();
        TM.TurnEvent.AddListener(UpdateHealth);
        // Get random attributes
        planets = GameObject.Find("/SlotManager").GetComponent<SlotManager>()._planetSlots;

        algorithmAttributes();
        health = 5;
        spriteRenderer.sprite = AlienSprites[Random.Range(0, AlienSprites.Count)];
        UpdateHappiness();
    }

    // Update is called once per frame
    void Update()
    {
        float n = (float)(health / 5.0);
        //Debug.Log(n);
        spriteRenderer.color = new Color(1, n, n);
    }

    /// <summary>
    /// Updates the happiness of the alien.
    /// </summary>
    /// When the alien on its preffered terrain, happiness is increased by 2;
    /// When the alien on its preffered temperature, happiness is increased by 2;
    /// TODO: When the alien on its preffered atmosphere, happiness is increased by 2;
    /// When the alien detects itself as being on the ship, its minimum happiness should be 3.
    void UpdateHappiness()
    {
        int newHappiness = 0;
        if (Slot?.Terrain == Terrain)
        {
            newHappiness += 1;
        }
        else if (Slot?.Terrain == AttributeStorage.Terrain.Ship)
        {
            newHappiness += 1;
        }
        if (Slot?.Temp == Temperature)
        {
            newHappiness += 1;
        }
        else if (Slot?.Temp == AttributeStorage.Temperature.Ship)
        {
            newHappiness += 2; // change this back to 1 later
        }
        if (Slot?.Atmosphere == Atmosphere)
        {
            newHappiness += 2;
        }
        if (Slot?.Resource == Resource)
        {
            newHappiness += 1;
        }
        // Because atmospheres aren't implemented yet, we'll just add 1 to happiness no matter what.
        // newHappiness += 1;
        happiness = Mathf.Clamp(newHappiness, 0, MAX_HAPPINESS);
    }

    void UpdateHealth()
    {
        UpdateHappiness();
        Debug.Log($"{species} happiness: {happiness}");
        if (happiness < 3)
        {
            health--;
            StartCoroutine(HandleHeathText());
        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator HandleHeathText()
    {
        var canvas = GameObject.Find("/Canvas");
        var rectTransform = canvas.GetComponent<RectTransform>();
        var viewport = GameObject.Find("/Main Camera").GetComponent<Camera>();
        var camPosition = viewport.WorldToViewportPoint(transform.position);
        var finalPosition = new Vector3(camPosition.x, camPosition.y, 0);
        var text = Instantiate(healthLostTextPrefab, finalPosition, Quaternion.identity, canvas.transform);
        text.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            (camPosition.x * rectTransform.sizeDelta.x) - (rectTransform.sizeDelta.x / 2),
            (camPosition.y * rectTransform.sizeDelta.y) - (rectTransform.sizeDelta.y / 2)
        );
        text.SetActive(true);
        yield return new WaitForSeconds(1);
        text.SetActive(false);
        Destroy(text);
    }

    void OnDestroy()
    {
        try { GameObject.Find("/WinManager").GetComponent<WinLossManager>().aliensDestroyed++; } catch {}
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

    void algorithmAttributes()
    {
        int planetint = Random.Range(0, planets.Length);
        int ran = Random.Range(0, 3);
        terrain = AttributeStorage.Shuffle<AttributeStorage.Terrain>(AttributeStorage.PlanetTerrains).First();
        temperature = AttributeStorage.Shuffle<AttributeStorage.Temperature>(AttributeStorage.PlanetTemps).First();
        resource = AttributeStorage.Shuffle<AttributeStorage.Resource>(AttributeStorage.Resources).First();
        if (ran == 0)
            terrain = planets[planetint].GetComponent<Slot>().Terrain;
        else if (ran == 1)
            temperature = planets[planetint].GetComponent<Slot>().Temp;
        else if (ran == 2)
            resource = planets[planetint].GetComponent<Slot>().Resource;
        atmosphere = planets[planetint].GetComponent<Slot>().Atmosphere;
    }
}