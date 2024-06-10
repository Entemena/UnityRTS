using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    private UIManager _UIManager;
    private Building _placedBuilding = null;
    private Ray _ray;
    private RaycastHit _raycastHit;
    private Vector3 _lastPlacementPosition;

    public void SelectPlacedBuilding(int buildingDataIndex)
    {
        PreparePlacedBuilding(buildingDataIndex);
    }

    public void Awake()
    {
        _UIManager = GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_placedBuilding != null)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                CancelPlacedBuilding();
                return;
            }
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log("Raycast: " + _ray.ToString());

            if (Physics.Raycast(_ray, out _raycastHit, 1000f, Globals.TERRAIN_LAYER_MASK))
            {
                Debug.Log("Raycast hit at: " + _raycastHit.point);
                _placedBuilding.SetPosition(_raycastHit.point);
                _lastPlacementPosition = _raycastHit.point;
            }
            _placedBuilding.CheckValidPlacement(); // Check placement validity every frame

            if (_placedBuilding.HasValidPlacement && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceBuilding();
            }
        }
    }

    void PreparePlacedBuilding(int buildingDataIndex)
    {
        BuildingData buildingData = Globals.BUILDING_DATA[buildingDataIndex];
        // Destroy the previous "phantom" if there is one and it's not fixed
        if (_placedBuilding != null && !_placedBuilding.IsFixed)
        {
            Destroy(_placedBuilding.Transform.gameObject);
        }

        _placedBuilding = new Building(buildingData);
        _lastPlacementPosition = Vector3.zero;
    }

    void PreparePlacedBuilding(BuildingData buildingData)
    {
        // destroy the previous "phantom" if there is one
        if (_placedBuilding != null && !_placedBuilding.IsFixed)
        {
            Destroy(_placedBuilding.Transform.gameObject);
        }
        Building building = new Building(buildingData);
        _placedBuilding = building;
        _lastPlacementPosition = Vector3.zero;
    }

    void PlaceBuilding()
    {
        _placedBuilding.Place();
        if (_placedBuilding.CanBuy())
        {
            PreparePlacedBuilding(_placedBuilding.DataIndex);
        }
        else
        {
            _placedBuilding = null;
        }
        EventManager.TriggerEvent("UpdateResourceTexts");
        EventManager.TriggerEvent("CheckBuildingButtons");
    }

    void CancelPlacedBuilding()
    {
        // Destroy the "phantom" building
        Destroy(_placedBuilding.Transform.gameObject);
        _placedBuilding = null;
    }
}
