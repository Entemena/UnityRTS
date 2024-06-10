using System.Collections.Generic;
using UnityEngine;

public enum BuildingPlacement
{
    INVALID,
    VALID,
    FIXED
};

public class Building
{
    private BuildingData _data;
    private Transform _transform;
    private int _currentHealth;
    private BuildingPlacement _placement;
    private List<Material> _materials;
    private BuildingManager _buildingManager;

    public Building(BuildingData data)
    {
        _data = data;
        _currentHealth = data.HP;

        GameObject g = GameObject.Instantiate(Resources.Load($"Prefabs/Buildings/{_data.Code}")) as GameObject;
        _transform = g.transform;
        _placement = BuildingPlacement.VALID;

        _materials = new List<Material>();
        foreach (Material material in _transform.Find("Mesh").GetComponent<Renderer>().materials)
        {
            _materials.Add(new Material(material));
        }

        _buildingManager = g.GetComponent<BuildingManager>();
        _buildingManager.Initialise(this);  // Ensure this is called after the BuildingManager is obtained.
        SetMaterials();
    }

    public bool HasValidPlacement
    {
        get => _placement == BuildingPlacement.VALID;
    }

    public void SetMaterials()
    {
        SetMaterials(_placement);
    }

    public void SetMaterials(BuildingPlacement placement)
    {
        Debug.Log($"Setting materials for placement: {placement}");
        List<Material> materials = new List<Material>();
        Material refMaterial = null;

        if (placement == BuildingPlacement.VALID)
        {
            refMaterial = Resources.Load("Materials/Valid") as Material;
        }
        else if (placement == BuildingPlacement.INVALID)
        {
            refMaterial = Resources.Load("Materials/Invalid") as Material;
        }
        else if (placement == BuildingPlacement.FIXED)
        {
            materials = _materials;
        }

        if (refMaterial != null)
        {
            for (int i = 0; i < _materials.Count; i++)
            {
                materials.Add(refMaterial);
            }
        }

        if (materials.Count > 0)
        {
            _transform.Find("Mesh").GetComponent<Renderer>().materials = materials.ToArray();
        }
    }

    public void SetPosition(Vector3 position)
    {
        _transform.position = position;
        Debug.Log($"Building moved to: {_transform.position}");
    }
    void OnGUI()
    {
        // Left example
        Utils.DrawScreenRectBorder(new Rect(32, 32, 256, 128), 2, Color.green);
        // Right example
        Utils.DrawScreenRect(new Rect(320, 32, 256, 128), new Color(0.8f, 0.8f, 0.95f, 0.25f));
        Utils.DrawScreenRectBorder(new Rect(320, 32, 256, 128), 2, new Color(0.8f, 0.8f, 0.95f));
    }
    public void Place()
    {
        _placement = BuildingPlacement.FIXED;
        SetMaterials();
        _transform.GetComponent<BoxCollider>().isTrigger = false;

        foreach (KeyValuePair<string, int> pair in _data.Cost)
        {
            Globals.GAME_RESOURCES[pair.Key].addAmount(-pair.Value);
        }
    }

    public void CheckValidPlacement()
    {
        Debug.Log("Checking valid placement...");
        if (_placement == BuildingPlacement.FIXED) return;
        _placement = _buildingManager.CheckPlacement() ? BuildingPlacement.VALID : BuildingPlacement.INVALID;
        Debug.Log($"Placement status: {_placement}");
    }

    public bool CanBuy()
    {
        return _data.CanBuy();
    }

    public bool IsFixed
    {
        get => _placement == BuildingPlacement.FIXED;
    }

    public string Code
    {
        get => _data.Code;
    }

    public Transform Transform
    {
        get => _transform;
    }

    public int HP
    {
        get => _currentHealth; set => _currentHealth = value;
    }

    public int MaxHP
    {
        get => _data.HP;
    }

    public int DataIndex
    {
        get
        {
            for (int i = 0; i < Globals.BUILDING_DATA.Length; i++)
            {
                if (Globals.BUILDING_DATA[i].Code == _data.Code)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
