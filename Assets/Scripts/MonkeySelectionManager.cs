using UnityEngine;
using UnityEngine.InputSystem;

public class MonkeySelectionManager : MonoBehaviour
{
    public static MonkeySelectionManager Instance { get; private set; }

    [SerializeField] private LayerMask _monkeyLayerMask;
    [SerializeField] private LayerMask _monkeyPlacementLayerMask;

    private GameObject _selectedInUIMonkeyPrefab = null;
    private GameObject _selectedInUIMonkeyInstance = null;

    private GameObject _monkeySelectedCurrentlyInMap = null;
    private GameObject _unselectedMonkeyCurrentlyUnderCursorInMap = null;

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
        if (_selectedInUIMonkeyPrefab != null)
        {
            HandleUIMonkeySelection();
        }
        else
        {
            HandleMapMonkeySelection();
        }
    }

    private void HandleUIMonkeySelection()
    {
        // Move selected monkey together with the cursor

        _selectedInUIMonkeyInstance.transform.position = Helper.GetCursorWorldPosition(0.0f);

        // Check with the help of raycasting if the selected monkey is over the valid placement region

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, _monkeyPlacementLayerMask);

        // If hit is detected, the ray hit a valid spot
        
        if (rayHit)
        {
            // Check with the help of colliders if the selected monkey doesn't overlap other monkeys

            CircleCollider2D collider = _selectedInUIMonkeyInstance.GetComponent<CircleCollider2D>();
            Collider2D[] colliderHits = Physics2D.OverlapCircleAll((Vector2)_selectedInUIMonkeyInstance.transform.position + collider.offset, collider.radius, _monkeyLayerMask);

            // There is always one collider detected, the one belonging to the selected monkey, we're interested in others

            if (colliderHits.Length < 2)
            {
                // Dropping monkey allowed

                _selectedInUIMonkeyInstance.GetComponent<SpriteRenderer>().color = Color.white;

                // Dropping monkey only after click

                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    // Dropped monkey becomes monkey that is currently selected on the map

                    _monkeySelectedCurrentlyInMap = _selectedInUIMonkeyInstance;

                    // The range isn't by default visible and monkey isn't by default selected (darkened)

                    SetMonkeyRangeVisibility(_monkeySelectedCurrentlyInMap, true);
                    SetMonkeyDarkening(_monkeySelectedCurrentlyInMap, true);

                    // Resetting the state of the variables

                    _selectedInUIMonkeyInstance = null;
                    _selectedInUIMonkeyPrefab = null;
                }
            }
            else
            {
                // Dropping monkey not allowed

                _selectedInUIMonkeyInstance.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 0.95f);
            }
        }
        else
        {
            // Dropping monkey not allowed

            _selectedInUIMonkeyInstance.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 0.95f);
        }
    }

    private void HandleMapMonkeySelection()
    {
        // Check with the help of raycasting if cursor hovers over some monkey or not

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, _monkeyLayerMask);

        // If cursor hovers over a monkey, we highlight the monkey by darkening its color, when cursor stop hovering, the color goes back to normal

        if (rayHit)
        {
            // Check over which monkey cursor hovers

            GameObject monkeyHit = rayHit.collider.gameObject;

            // If cursor hovers over the currently selected monkey or the same monkey as in the previous frame - do nothing

            if (monkeyHit != _monkeySelectedCurrentlyInMap && monkeyHit != _unselectedMonkeyCurrentlyUnderCursorInMap)
            {
                // Otherwise, make sure to dehilight the previously hovered monkey

                if (_unselectedMonkeyCurrentlyUnderCursorInMap != null)
                {
                    SetMonkeyDarkening(_unselectedMonkeyCurrentlyUnderCursorInMap, false);
                }

                // And also make sure to upadate and highlight the new hovered monkey

                _unselectedMonkeyCurrentlyUnderCursorInMap = monkeyHit;
                SetMonkeyDarkening(monkeyHit, true);
            }
        }
        else
        {
            // When cursor stops hovering over a monkey, make sure to dehighlight previously highlighted one

            if (_unselectedMonkeyCurrentlyUnderCursorInMap != null)
            {
                SetMonkeyDarkening(_unselectedMonkeyCurrentlyUnderCursorInMap, false);
            }

            // Cursor isn't hovering over any monkey

            _unselectedMonkeyCurrentlyUnderCursorInMap = null;
        }

        // If we click, we either change the selected monkey to a new one, or make sure to deselect everything (when we click on an empty spot or currently selected monkey)

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // No matter where we click, we make sure to deselect the currently selected monkey

            if (_monkeySelectedCurrentlyInMap != null)
            {
                SetMonkeyRangeVisibility(_monkeySelectedCurrentlyInMap, false);
                SetMonkeyDarkening(_monkeySelectedCurrentlyInMap, false);
            }

            // Set the new value for the currently selected monkey

            _monkeySelectedCurrentlyInMap = _unselectedMonkeyCurrentlyUnderCursorInMap;

            // If we clicked on another monkey, we display its range (no need to highlight it since it's already higlighted because of cursor hovering)

            if (_monkeySelectedCurrentlyInMap != null)
            {
                SetMonkeyRangeVisibility(_monkeySelectedCurrentlyInMap, true);
            }

            // Go back to the initial state

            _unselectedMonkeyCurrentlyUnderCursorInMap = null;
        }
    }

    private void SetMonkeyRangeVisibility(GameObject monkey, bool monkeyRangeVisible)
    {
        monkey.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = monkeyRangeVisible ? true : false;
    }

    private void SetMonkeyDarkening(GameObject monkey, bool monkeyDarkened)
    {
        SpriteRenderer spriteRenderer = monkey.GetComponent<SpriteRenderer>();
        float coeff = monkeyDarkened ? -1.0f : 1.0f;
        spriteRenderer.color = Helper.ChangeHSVValue(spriteRenderer.color, coeff * _deltaHSVValue);
    }

    public void SelectMonkeyFromUI(GameObject monkeyPrefab)
    {
        _selectedInUIMonkeyPrefab = monkeyPrefab;

        if (_selectedInUIMonkeyInstance != null)
        {
            Destroy(_selectedInUIMonkeyInstance);
        }

        _selectedInUIMonkeyInstance = Instantiate(_selectedInUIMonkeyPrefab, Helper.GetCursorWorldPosition(0.0f), Quaternion.identity);

        _monkeySelectedCurrentlyInMap = null;
        _unselectedMonkeyCurrentlyUnderCursorInMap = null;
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
