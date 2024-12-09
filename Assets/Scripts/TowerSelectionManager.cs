using UnityEngine;
using UnityEngine.InputSystem;

public class TowerSelectionManager : MonoBehaviour
{
    public static TowerSelectionManager Instance { get; private set; }

    [SerializeField] private LayerMask _towerLayerMask;
    [SerializeField] private LayerMask _towerPlacementLayerMask;

    private GameObject _selectedInUITowerPrefab = null;
    private GameObject _selectedInUITowerInstance = null;

    private GameObject _towerSelectedCurrentlyInMap = null;
    private GameObject _unselectedTowerCurrentlyUnderCursorInMap = null;

    private const float _deltaHSVValue = 0.2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (_selectedInUITowerPrefab != null)
        {
            HandleUITowerSelection();
        }
        else
        {
            HandleMapTowerSelection();
        }
    }

    private void HandleUITowerSelection()
    {
        // Move selected tower together with the cursor

        _selectedInUITowerInstance.transform.position = Helper.GetCursorWorldPosition(0.0f);

        // Check with the help of raycasting if the selected tower is over the valid placement region

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, _towerPlacementLayerMask);

        // If hit is detected, the ray hit a valid spot
        
        if (rayHit)
        {
            // Check with the help of colliders if the selected tower doesn't overlap other towers

            CircleCollider2D collider = _selectedInUITowerInstance.GetComponent<CircleCollider2D>();
            Collider2D[] colliderHits = Physics2D.OverlapCircleAll((Vector2)_selectedInUITowerInstance.transform.position + collider.offset, collider.radius, _towerLayerMask);

            // There is always one collider detected, the one belonging to the selected tower, we're interested in others

            if (colliderHits.Length < 2)
            {
                // Dropping tower allowed

                _selectedInUITowerInstance.GetComponent<SpriteRenderer>().color = Color.white;

                // Dropping tower only after click

                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    // Dropped tower becomes tower that is currently selected on the map

                    _towerSelectedCurrentlyInMap = _selectedInUITowerInstance;

                    // The range isn't by default visible and tower isn't by default selected (darkened)

                    SetTowerRangeVisibility(_towerSelectedCurrentlyInMap, true);
                    SetTowerDarkening(_towerSelectedCurrentlyInMap, true);

                    // Resetting the state of the variables

                    _selectedInUITowerInstance = null;
                    _selectedInUITowerPrefab = null;
                }
            }
            else
            {
                // Dropping tower not allowed

                _selectedInUITowerInstance.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 0.95f);
            }
        }
        else
        {
            // Dropping tower not allowed

            _selectedInUITowerInstance.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 0.95f);
        }
    }

    private void HandleMapTowerSelection()
    {
        // Check with the help of raycasting if cursor hovers over some tower or not

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, _towerLayerMask);

        // If cursor hovers over a tower, we highlight the tower by darkening its color, when cursor stop hovering, the color goes back to normal

        if (rayHit)
        {
            // Check over which tower cursor hovers

            GameObject towerHit = rayHit.collider.gameObject;

            // If cursor hovers over the currently selected tower or the same tower as in the previous frame - do nothing

            if (towerHit != _towerSelectedCurrentlyInMap && towerHit != _unselectedTowerCurrentlyUnderCursorInMap)
            {
                // Otherwise, make sure to dehilight the previously hovered tower

                if (_unselectedTowerCurrentlyUnderCursorInMap != null)
                {
                    SetTowerDarkening(_unselectedTowerCurrentlyUnderCursorInMap, false);
                }

                // And also make sure to upadate and highlight the new hovered tower

                _unselectedTowerCurrentlyUnderCursorInMap = towerHit;
                SetTowerDarkening(towerHit, true);
            }
        }
        else
        {
            // When cursor stops hovering over a tower, make sure to dehighlight previously highlighted one

            if (_unselectedTowerCurrentlyUnderCursorInMap != null)
            {
                SetTowerDarkening(_unselectedTowerCurrentlyUnderCursorInMap, false);
            }

            // Cursor isn't hovering over any tower

            _unselectedTowerCurrentlyUnderCursorInMap = null;
        }

        // If we click, we either change the selected tower to a new one, or make sure to deselect everything (when we click on an empty spot or currently selected tower)

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // No matter where we click, we make sure to deselect the currently selected tower

            if (_towerSelectedCurrentlyInMap != null)
            {
                SetTowerRangeVisibility(_towerSelectedCurrentlyInMap, false);
                SetTowerDarkening(_towerSelectedCurrentlyInMap, false);
            }

            // Set the new value for the currently selected tower

            _towerSelectedCurrentlyInMap = _unselectedTowerCurrentlyUnderCursorInMap;

            // If we clicked on another tower, we display its range (no need to highlight it since it's already higlighted because of cursor hovering)

            if (_towerSelectedCurrentlyInMap != null)
            {
                SetTowerRangeVisibility(_towerSelectedCurrentlyInMap, true);
            }

            // Go back to the initial state

            _unselectedTowerCurrentlyUnderCursorInMap = null;
        }
    }

    private void SetTowerRangeVisibility(GameObject tower, bool towerRangeVisible)
    {
        tower.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = towerRangeVisible ? true : false;
    }

    private void SetTowerDarkening(GameObject tower, bool towerDarkened)
    {
        SpriteRenderer spriteRenderer = tower.GetComponent<SpriteRenderer>();
        float coeff = towerDarkened ? -1.0f : 1.0f;
        spriteRenderer.color = Helper.ChangeHSVValue(spriteRenderer.color, coeff * _deltaHSVValue);
    }

    public void SelectTowerFromUI(GameObject towerPrefab)
    {
        _selectedInUITowerPrefab = towerPrefab;

        if (_selectedInUITowerInstance != null)
        {
            Destroy(_selectedInUITowerInstance);
        }

        _selectedInUITowerInstance = Instantiate(_selectedInUITowerPrefab, Helper.GetCursorWorldPosition(0.0f), Quaternion.identity);

        _towerSelectedCurrentlyInMap = null;
        _unselectedTowerCurrentlyUnderCursorInMap = null;
    }
}

/*
 * 
 * A layer mask IS NOT the same thing as a layer number
 * 
 * What is layer mask?
 * 0000 0000 0000 0000 0000 0000 0000 0000
 * 32 bits representing 32 layers, from layer 0 to layer 31
 * 0 means layer mask will be deactivated
 * 1 means layer mask will be activated
 * We count from right to left
 * 
 * For example, let's say you want to raycast only objects on layer 3
 * If you pass 3 as an argument representing a layer mask, the layer mask will actually be:
 * 3 = 0000 0000 0000 0000 0000 0000 0000 0011
 * It means that layers 0 and 1 will be used in raycasting, not layer 3
 * 
 * To get the proper layer mask you use GetMask or bit operators, e.g. 1 << 3 (shift one by 3 to the left) (in this case, layer mask will be 8)
 * 
 */
