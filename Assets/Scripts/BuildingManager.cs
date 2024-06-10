using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BuildingManager : UnitManager
{
    private BoxCollider _boxCollider;
    private Building _building;
    private int _nCollisions;

    public void Initialise(Building building)
    {
        _boxCollider = GetComponent<BoxCollider>();
        _building = building;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            return;
        _nCollisions++;
        CheckPlacement();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            return;
        _nCollisions--;
        CheckPlacement();
    }

    public bool CheckPlacement()
    {
        if (_building == null || _building.IsFixed) return false;
        bool validPlacement = HasValidPlacement();
        _building.SetMaterials(validPlacement ? BuildingPlacement.VALID : BuildingPlacement.INVALID);
        return validPlacement;
    }

    public bool HasValidPlacement()
    {
        if (_nCollisions > 0)
        {
            Debug.Log("Invalid placement due to collision.");
            return false;
        }

        Vector3 p = transform.position;
        Vector3 c = _boxCollider.center;
        Vector3 e = _boxCollider.size / 2;
        float additionalHeight = 1.0f;  // Adjust this value as needed to ensure rays start above the object

        Vector3[] bottomCorners = new Vector3[]
        {
        new Vector3(c.x - e.x, c.y - e.y, c.z - e.z),
        new Vector3(c.x - e.x, c.y - e.y, c.z + e.z),
        new Vector3(c.x + e.x, c.y - e.y, c.z - e.z),
        new Vector3(c.x + e.x, c.y - e.y, c.z + e.z)
        };

        int validGroundHits = 0;
        foreach (Vector3 corner in bottomCorners)
        {
            Vector3 rayStart = p + corner + Vector3.up * additionalHeight;  // Starts above the object
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, additionalHeight + 2f, Globals.TERRAIN_LAYER_MASK))  // Ray length covers the added height plus a little extra
            {
                Debug.DrawRay(rayStart, Vector3.down * (additionalHeight + 2f), Color.green, 5f);
                if (Vector3.Angle(hit.normal, Vector3.up) < 5)  // Allowing slight deviations from being perfectly vertical
                {
                    validGroundHits++;
                }
            }
            else
            {
                Debug.DrawRay(rayStart, Vector3.down * (additionalHeight + 2f), Color.red, 5f);
                Debug.Log("No hit detected from " + rayStart);
            }
        }

        bool isValid = validGroundHits >= 3;
        Debug.Log($"Valid ground hits: {validGroundHits}, valid placement: {isValid}");
        return isValid;
    }

}
