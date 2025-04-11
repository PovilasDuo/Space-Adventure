using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BoidManagerUIController : MonoBehaviour
{
    public UIDocument uiDocument; 
    private VisualElement root;
    private SliderInt flockIDSlider;
    private Label flockIDValue;
    private SliderInt visionRadiusSlider;
    private Label visionRadiusValue;
    private SliderInt visionAngleSlider;
    private Label visionAngleValue;
    private SliderInt speedSlider;
    private Label speedValue;
    private Slider cohesionWeightSlider;
    private Label cohesionWeightValue;
    private Slider alignmentWeightSlider;
    private Label alignmentWeightValue;
    private Slider separationWeightSlider;
    private Label separationWeightValue;
    private SliderInt leaderFollowWeightSlider;
    private Label leaderFollowWeightValue;
    private SliderInt avoidanceWeightSlider;
    private Label avoidanceWeightValue;
    private Slider lineFormationWeightSlider;
    private Label lineFormationWeightValue;
    private SliderInt enemyInteractionWeightSlider;
    private Label enemyInteractionWeightValue;
    private Slider stuckThresholdSlider;
    private Label stuckThresholdValue;
    private Slider maxStuckTimeSlider;
    private Label maxStuckTimeValue;
    private SliderInt lineSegmentsSlider;
    private Label lineSegmentsValue;
    private Slider rayCastCooldownSlider;
    private Label rayCastCooldownValue;
    private Slider minXSlider;
    private Label minXValue;
    private Slider maxXSlider;
    private Label maxXValue;
    private Slider minYSlider;
    private Label minYValue;
    private Slider maxYSlider;
    private Label maxYValue;
    private Toggle manageBoidsToggle;
    private Toggle isAggressiveToggle;
    private Toggle interactsWithEnemiesToggle;
    private Toggle displayVisionRangeToggle;
    private Toggle visualizeColorsToggle;
    private Toggle useTrailToggle;
    private Toggle setBoidColorBasedOnFlockIdToggle;
    private Toggle useScreenWarpToggle;
    private Toggle fixZPositionToggle;
    private Toggle boundToAreaToggle;
    private Button applyToAll;

    private VisualElement buttonContainer;
    private List<Button> buttons;

    private List<BoidManager> boidManagers;

    private BoidManager boidManager;
    private BoidSettings settings;
    private BoidManagerSettings boidManagerSettings;

    private BoidSpawner boidSpawner;

    private bool isUIVisible = true;

    private void OnEnable()
    {
        SetUpUI();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.M))
        {
            if (isUIVisible)
            {
                HideUI();
            }
            else
            {
                ShowUI();
            }
        }
    }

    /// <summary>
    /// Sets up the UI by initializing values, adding listeners, and setting up min/max values.
    /// </summary>
    private void SetUpUI()
    {
        SetBoidRelatedValues();
        AddListeners();
        AddMinMaxValues();
        HideUI();
        DeactivateButtonsBasedOnBoidManagers();
    }

    /// <summary>
    /// Sets the boid related values by finding the BoidSpawner and BoidManagers in the scene.
    /// </summary>
    private void SetBoidRelatedValues()
    {
        boidSpawner = GameObject.FindAnyObjectByType<BoidSpawner>();
        if (boidSpawner == null)
        {
            Debug.LogError("No spawner is active. Add a spawner to use the UI");
        }

        boidManagers = GetBoidManagers();
        BoidManager interimBoidManager = boidSpawner.GetBoidManager();
        if (interimBoidManager == null)
        {
            SetCurrentBoidManager(boidManagers[0]);
        }
        else
        {
            boidManager = interimBoidManager;
            settings = boidManager.GetBoidSettings();
            boidManagerSettings = boidManager.GetBoidManagerSettings();
            UpdateBoidManagerSettings(boidManager);
            GetValues();
            AddMinMaxValues();
            SetInitialTextValues();
        }
    }

    /// <summary>
    /// Shows the UI by setting its display style to Flex and disabling the spawner.
    /// </summary>
    private void ShowUI()
    {
        VisualElement root = uiDocument.rootVisualElement;
        root.style.display = DisplayStyle.Flex;
        isUIVisible = true;
        DisableSpawner(false);
    }

    /// <summary>
    /// Hides the UI by setting its display style to None and disabling the spawner.
    /// </summary>
    private void HideUI()
    {
        VisualElement root = uiDocument.rootVisualElement;
        root.style.display = DisplayStyle.None;
        isUIVisible = false;
        DisableSpawner(true);
    }

    /// <summary>
    /// Disables the spawner by enabling or disabling it based on the active parameter.
    /// </summary>
    /// <param name="active">To disable or enable</param>
    private void DisableSpawner(bool active)
    {
        boidSpawner.enabled = active;
    }

    /// <summary>
    /// Gets the values from the UI elements and assigns them to the corresponding variables.
    /// </summary>
    private void GetValues()
    {
        root = uiDocument.rootVisualElement;
        flockIDSlider = root.Q<SliderInt>("flockID");
        flockIDValue = root.Q<Label>("flockIDValue");
        visionRadiusSlider = root.Q<SliderInt>("visionRadius");
        visionRadiusValue = root.Q<Label>("visionRadiusValue");
        visionAngleSlider = root.Q<SliderInt>("visionAngle");
        visionAngleValue = root.Q<Label>("visionAngleValue");
        speedSlider = root.Q<SliderInt>("speed");
        speedValue = root.Q<Label>("speedValue");
        cohesionWeightSlider = root.Q<Slider>("cohesionWeight");
        cohesionWeightValue = root.Q<Label>("cohesionWeightValue");
        alignmentWeightSlider = root.Q<Slider>("alignmentWeight");
        alignmentWeightValue = root.Q<Label>("alignmentWeightValue");
        separationWeightSlider = root.Q<Slider>("separationWeight");
        separationWeightValue = root.Q<Label>("separationWeightValue");
        leaderFollowWeightSlider = root.Q<SliderInt>("leaderFollowWeight");
        leaderFollowWeightValue = root.Q<Label>("leaderFollowWeightValue");
        avoidanceWeightSlider = root.Q<SliderInt>("avoidanceWeight");
        avoidanceWeightValue = root.Q<Label>("avoidanceWeightValue");
        lineFormationWeightSlider = root.Q<Slider>("lineFormationWeight");
        lineFormationWeightValue = root.Q<Label>("lineFormationWeightValue");
        enemyInteractionWeightSlider = root.Q<SliderInt>("enemyInteractionWeight");
        enemyInteractionWeightValue = root.Q<Label>("enemyInteractionWeightValue");
        stuckThresholdSlider = root.Q<Slider>("stuckThreshold");
        stuckThresholdValue = root.Q<Label>("stuckThresholdValue");
        maxStuckTimeSlider = root.Q<Slider>("maxStuckTime");
        maxStuckTimeValue = root.Q<Label>("maxStuckTimeValue");
        lineSegmentsSlider = root.Q<SliderInt>("lineSegments");
        lineSegmentsValue = root.Q<Label>("lineSegmentsValue");
        rayCastCooldownSlider = root.Q<Slider>("rayCastCooldown");
        rayCastCooldownValue = root.Q<Label>("rayCastCooldownValue");
        minXSlider = root.Q<Slider>("minX");
        minXValue = root.Q<Label>("minXValue");
        maxXSlider = root.Q<Slider>("maxX");
        maxXValue = root.Q<Label>("maxXValue");
        minYSlider = root.Q<Slider>("minY");
        minYValue = root.Q<Label>("minYValue");
        maxYSlider = root.Q<Slider>("maxY");
        maxYValue = root.Q<Label>("maxYValue");
        manageBoidsToggle = root.Q<Toggle>("manageBoids");
        isAggressiveToggle = root.Q<Toggle>("isAggressive");
        interactsWithEnemiesToggle = root.Q<Toggle>("interactsWithEnemies");
        displayVisionRangeToggle = root.Q<Toggle>("displayVisionRange");
        visualizeColorsToggle = root.Q<Toggle>("visualizeColors");
        useTrailToggle = root.Q<Toggle>("useTrail");
        setBoidColorBasedOnFlockIdToggle = root.Q<Toggle>("setBoidColorBasedOnFlockId");
        useScreenWarpToggle = root.Q<Toggle>("useScreenWarp");
        fixZPositionToggle = root.Q<Toggle>("fixZPosition");
        boundToAreaToggle = root.Q<Toggle>("boundToArea");

        settings = boidManager.GetBoidSettings();
        boidManagerSettings = boidManager.GetBoidManagerSettings();

        buttonContainer = root.Q<VisualElement>("button-container");
        buttons = buttonContainer.Query<Button>().ToList();
        applyToAll = root.Q<Button>("applyToAll");
    }

    /// <summary>
    /// Adds minimum and maximum values to the sliders.
    /// </summary>
    private void AddMinMaxValues()
    {
        flockIDSlider.lowValue = 0; flockIDSlider.highValue = 9;
        visionRadiusSlider.lowValue = 1; visionRadiusSlider.highValue = 50;
        visionAngleSlider.lowValue = 1; visionAngleSlider.highValue = 180;
        speedSlider.lowValue = 0; speedSlider.highValue = 20;
        cohesionWeightSlider.lowValue = 0; cohesionWeightSlider.highValue = 5;
        alignmentWeightSlider.lowValue = 0; alignmentWeightSlider.highValue = 5;
        separationWeightSlider.lowValue = 0; separationWeightSlider.highValue = 5;
        leaderFollowWeightSlider.lowValue = 0; leaderFollowWeightSlider.highValue = 10;
        avoidanceWeightSlider.lowValue = 0; avoidanceWeightSlider.highValue = 10;
        lineFormationWeightSlider.lowValue = 0; lineFormationWeightSlider.highValue = 10;
        enemyInteractionWeightSlider.lowValue = 0; enemyInteractionWeightSlider.highValue = 10;
        stuckThresholdSlider.lowValue = 0.01f; stuckThresholdSlider.highValue = 1;
        maxStuckTimeSlider.lowValue = 0.1f; maxStuckTimeSlider.highValue = 5;
        lineSegmentsSlider.lowValue = 6; lineSegmentsSlider.highValue = 64;
        rayCastCooldownSlider.lowValue = 0; rayCastCooldownSlider.highValue = 1;
        minXSlider.lowValue = -100; minXSlider.highValue = 100;
        maxXSlider.lowValue = -100; maxXSlider.highValue = 100;
        minYSlider.lowValue = -100; minYSlider.highValue = 100;
        maxYSlider.lowValue = -100; maxYSlider.highValue = 100;
    }

    /// <summary>
    /// Adds listeners to the UI elements to update the settings when their values change.
    /// </summary>
    private void AddListeners()
    {
        flockIDSlider.RegisterValueChangedCallback(evt =>
        {
            settings.flockID = evt.newValue;
            flockIDValue.text = evt.newValue.ToString();
        });
        visionRadiusSlider.RegisterValueChangedCallback(evt =>
        {
            settings.visionRadius = evt.newValue;
            visionRadiusValue.text = evt.newValue.ToString();
        });
        visionAngleSlider.RegisterValueChangedCallback(evt =>
        {
            settings.visionAngle = evt.newValue;
            visionAngleValue.text = evt.newValue.ToString();
        });
        speedSlider.RegisterValueChangedCallback(evt =>
        {
            settings.speed = evt.newValue;
            speedValue.text = evt.newValue.ToString();
        });
        cohesionWeightSlider.RegisterValueChangedCallback(evt =>
        {
            settings.cohesionWeight = evt.newValue;
            cohesionWeightValue.text = evt.newValue.ToString("0.00");
        });
        alignmentWeightSlider.RegisterValueChangedCallback(evt =>
        {
            settings.alignmentWeight = evt.newValue;
            alignmentWeightValue.text = evt.newValue.ToString("0.00");
        });
        separationWeightSlider.RegisterValueChangedCallback(evt =>
        {
            settings.separationWeight = evt.newValue;
            separationWeightValue.text = evt.newValue.ToString("0.00");
        });
        leaderFollowWeightSlider.RegisterValueChangedCallback(evt =>
        {
            settings.leaderFollowWeight = evt.newValue;
            leaderFollowWeightValue.text = evt.newValue.ToString();
        });
        avoidanceWeightSlider.RegisterValueChangedCallback(evt =>
        {
            settings.avoidanceWeight = evt.newValue;
            avoidanceWeightValue.text = evt.newValue.ToString();
        });
        lineFormationWeightSlider.RegisterValueChangedCallback(evt =>
        {
            settings.lineFormationWeight = evt.newValue;
            lineFormationWeightValue.text = evt.newValue.ToString("0.00");
        });
        enemyInteractionWeightSlider.RegisterValueChangedCallback(evt =>
        {
            settings.enemyInteractionWeight = evt.newValue;
            enemyInteractionWeightValue.text = evt.newValue.ToString();
        });
        stuckThresholdSlider.RegisterValueChangedCallback(evt =>
        {
            settings.stuckThreshold = evt.newValue;
            stuckThresholdValue.text = evt.newValue.ToString("0.00");
        });
        maxStuckTimeSlider.RegisterValueChangedCallback(evt =>
        {
            settings.maxStuckTime = evt.newValue;
            maxStuckTimeValue.text = evt.newValue.ToString("0.00");
        });
        lineSegmentsSlider.RegisterValueChangedCallback(evt =>
        {
            settings.lineSegments = evt.newValue;
            lineSegmentsValue.text = evt.newValue.ToString();
        });
        rayCastCooldownSlider.RegisterValueChangedCallback(evt =>
        {
            settings.rayCastCooldown = evt.newValue;
            rayCastCooldownValue.text = evt.newValue.ToString("0.00");
        });
        minXSlider.RegisterValueChangedCallback(evt =>
        {
            settings.minX = evt.newValue;
            minXValue.text = evt.newValue.ToString("0.00");
        });
        maxXSlider.RegisterValueChangedCallback(evt =>
        {
            settings.maxX = evt.newValue;
            maxXValue.text = evt.newValue.ToString("0.00");
        });
        minYSlider.RegisterValueChangedCallback(evt =>
        {
            settings.minY = evt.newValue;
            minYValue.text = evt.newValue.ToString("0.00");
        });
        maxYSlider.RegisterValueChangedCallback(evt =>
        {
            settings.maxY = evt.newValue;
            maxYValue.text = evt.newValue.ToString("0.00");
        });

        manageBoidsToggle.RegisterValueChangedCallback(evt => boidManagerSettings.manageBoids = evt.newValue);
        isAggressiveToggle.RegisterValueChangedCallback(evt => settings.isAggressive = evt.newValue);
        interactsWithEnemiesToggle.RegisterValueChangedCallback(evt => settings.interactsWithEnemies = evt.newValue);
        displayVisionRangeToggle.RegisterValueChangedCallback(evt => {
            settings.displayVisionRange = evt.newValue;
            boidManager.EnableFlockVisionRange(boidManager.GetBoidSettings().flockID, evt.newValue);
        })
        ;
        visualizeColorsToggle.RegisterValueChangedCallback(evt => settings.visualizeColors = evt.newValue);
        useTrailToggle.RegisterValueChangedCallback(evt => settings.useTrail = evt.newValue);
        setBoidColorBasedOnFlockIdToggle.RegisterValueChangedCallback(evt => settings.setBoidColorBasedOnFlockId = evt.newValue);
        useScreenWarpToggle.RegisterValueChangedCallback(evt => settings.useScreenWarp = evt.newValue);
        fixZPositionToggle.RegisterValueChangedCallback(evt => settings.fixZPosition = evt.newValue);
        boundToAreaToggle.RegisterValueChangedCallback(evt => settings.boundToArea = evt.newValue);

        AddButtonListeners();
        applyToAll.RegisterCallback<ClickEvent>(evt =>
        {
            SetCurrentBoidManager(null);
        });
    }

    /// <summary>
    /// Adds listeners to the buttons in the button container.
    /// </summary>
    private void AddButtonListeners()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[i].RegisterCallback<ClickEvent>(CreateButtonClickHandler(index));
        }
    }

    /// <summary>
    /// Creates a button click handler for the buttons in the button container.
    /// </summary>
    /// <param name="index">Index of the button</param>
    /// <returns>The click event for the button</returns>
    private EventCallback<ClickEvent> CreateButtonClickHandler(int index)
    {
        return evt => { SetCurrentBoidManager(boidManagers[index]); };
    }

    /// <summary>
    /// Sets the initial text values for the UI elements based on the settings.
    /// </summary>
    private void SetInitialTextValues()
    {
        flockIDSlider.value = settings.flockID;
        flockIDValue.text = flockIDSlider.value.ToString();
        visionRadiusSlider.value = settings.visionRadius;
        visionRadiusValue.text = visionRadiusSlider.value.ToString();
        visionAngleSlider.value = settings.visionAngle;
        visionAngleValue.text = visionAngleSlider.value.ToString();
        speedSlider.value = settings.speed;
        speedValue.text = speedSlider.value.ToString();
        cohesionWeightSlider.value = settings.cohesionWeight;
        cohesionWeightValue.text = cohesionWeightSlider.value.ToString("0.00");
        alignmentWeightSlider.value = settings.alignmentWeight;
        alignmentWeightValue.text = alignmentWeightSlider.value.ToString("0.00");
        separationWeightSlider.value = settings.separationWeight;
        separationWeightValue.text = separationWeightSlider.value.ToString("0.00");
        leaderFollowWeightSlider.value = settings.leaderFollowWeight;
        leaderFollowWeightValue.text = leaderFollowWeightSlider.value.ToString();
        avoidanceWeightSlider.value = settings.avoidanceWeight;
        avoidanceWeightValue.text = avoidanceWeightSlider.value.ToString();
        lineFormationWeightSlider.value = settings.lineFormationWeight;
        lineFormationWeightValue.text = lineFormationWeightSlider.value.ToString("0.00");
        enemyInteractionWeightSlider.value = settings.enemyInteractionWeight;
        enemyInteractionWeightValue.text = enemyInteractionWeightSlider.value.ToString();
        stuckThresholdSlider.value = settings.stuckThreshold;
        stuckThresholdValue.text = stuckThresholdSlider.value.ToString("0.00");
        maxStuckTimeSlider.value = settings.maxStuckTime;
        maxStuckTimeValue.text = maxStuckTimeSlider.value.ToString("0.00");
        lineSegmentsSlider.value = settings.lineSegments;
        lineSegmentsValue.text = lineSegmentsSlider.value.ToString();
        rayCastCooldownSlider.value = settings.rayCastCooldown;
        rayCastCooldownValue.text = rayCastCooldownSlider.value.ToString("0.00");
        minXSlider.value = settings.minX;
        minXValue.text = minXSlider.value.ToString("0.00");
        maxXSlider.value = settings.maxX;
        maxXValue.text = maxXSlider.value.ToString("0.00");
        minYSlider.value = settings.minY;
        minYValue.text = minYSlider.value.ToString("0.00");
        maxYSlider.value = settings.maxY;
        maxYValue.text = maxYSlider.value.ToString("0.00");

        manageBoidsToggle.value = boidManagerSettings.manageBoids;
        isAggressiveToggle.value = settings.isAggressive;
        interactsWithEnemiesToggle.value = settings.interactsWithEnemies;
        displayVisionRangeToggle.value = settings.displayVisionRange;
        visualizeColorsToggle.value = settings.visualizeColors;
        useTrailToggle.value = settings.useTrail;
        setBoidColorBasedOnFlockIdToggle.value = settings.setBoidColorBasedOnFlockId;
        useScreenWarpToggle.value = settings.useScreenWarp;
        fixZPositionToggle.value = settings.fixZPosition;
        boundToAreaToggle.value = settings.boundToArea;
    }

    /// <summary>
    /// Gets the list of BoidManagers in the scene.
    /// </summary>
    /// <returns>The list of the BoidManagers</returns>
    private List<BoidManager> GetBoidManagers()
    {
        return GameObject.FindObjectsByType<BoidManager>(FindObjectsSortMode.None).ToList();
    }

    /// <summary>
    /// Deactivates the buttons based on the number of BoidManagers in the scene.
    /// </summary>
    private void DeactivateButtonsBasedOnBoidManagers()
    {
        if (boidManagers.Count > 0)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                UpdateActiveButtonStyle(i);
                if (i > boidManagers.Count - 1)
                {
                    buttons[i].SetEnabled(false);
                }
                else
                {
                    buttons[i].SetEnabled(true);
                }
            }
        }
    }

    /// <summary>
    /// Updates the active button style based on the index of the button.
    /// </summary>
    /// <param name="index">The index of the button</param>
    private void UpdateActiveButtonStyle(int index)
    {
        if (index == boidManagers.IndexOf(boidManager))
        {
            buttons[index].AddToClassList("active-button");
        }
        else
        {
            buttons[index].RemoveFromClassList("active-button");
        }
    }

    /// <summary>
    /// Sets the current BoidManager and updates the settings accordingly.
    /// </summary>
    /// <param name="boidManager">The BoidManager to be set as the current</param>
    private void SetCurrentBoidManager(BoidManager boidManager)
    {
        if (boidManager == null)
        {
            foreach (var manager in boidManagers)
            {
                UpdateBoidManagerSettings(manager);
            }
        }
        else
        {
            this.boidManager = boidManager;
            settings = boidManager.GetBoidSettings();
            boidManagerSettings = boidManager.GetBoidManagerSettings();
            boidSpawner.SetBoidManager(boidManager);
            UpdateBoidManagerSettings(boidManager);
        }
        HandleActiveManagerChange();
    }

    /// <summary>
    /// Updates the BoidManager settings based on the current settings.
    /// </summary>
    /// <param name="manager">The BoidManager whose settings should be applied</param>
    private void UpdateBoidManagerSettings(BoidManager manager)
    {
        manager.GetBoidManagerSettings().manageBoids = boidManagerSettings.manageBoids;
        manager.GetBoidSettings().flockID = settings.flockID;
        manager.GetBoidSettings().visionRadius = settings.visionRadius;
        manager.GetBoidSettings().visionAngle = settings.visionAngle;
        manager.GetBoidSettings().speed = settings.speed;
        manager.GetBoidSettings().cohesionWeight = settings.cohesionWeight;
        manager.GetBoidSettings().alignmentWeight = settings.alignmentWeight;
        manager.GetBoidSettings().separationWeight = settings.separationWeight;
        manager.GetBoidSettings().leaderFollowWeight = settings.leaderFollowWeight;
        manager.GetBoidSettings().avoidanceWeight = settings.avoidanceWeight;
        manager.GetBoidSettings().lineFormationWeight = settings.lineFormationWeight;
        manager.GetBoidSettings().enemyInteractionWeight = settings.enemyInteractionWeight;
        manager.GetBoidSettings().isAggressive = settings.isAggressive;
        manager.GetBoidSettings().interactsWithEnemies = settings.interactsWithEnemies;
        manager.GetBoidSettings().stuckThreshold = settings.stuckThreshold;
        manager.GetBoidSettings().maxStuckTime = settings.maxStuckTime;
        manager.GetBoidSettings().lineSegments = settings.lineSegments;
        manager.GetBoidSettings().rayCastCooldown = settings.rayCastCooldown;
        manager.GetBoidSettings().displayVisionRange = settings.displayVisionRange;
        manager.GetBoidSettings().visualizeColors = settings.visualizeColors;
        manager.GetBoidSettings().useTrail = settings.useTrail;
        manager.GetBoidSettings().setBoidColorBasedOnFlockId = settings.setBoidColorBasedOnFlockId;
        manager.GetBoidSettings().useScreenWarp = settings.useScreenWarp;
        manager.GetBoidSettings().fixZPosition = settings.fixZPosition;
        manager.GetBoidSettings().boundToArea = settings.boundToArea;
        manager.GetBoidSettings().minX = settings.minX;
        manager.GetBoidSettings().maxX = settings.maxX;
        manager.GetBoidSettings().minY = settings.minY;
        manager.GetBoidSettings().maxY = settings.maxY;
    }

    /// <summary>
    /// Handles the change of the active manager by updating the values and setting the initial text values.
    /// </summary>
    private void HandleActiveManagerChange()
    {
        GetValues();
        SetInitialTextValues();

        for(int i = 0; i < boidManagers.Count; i++)
        {
            UpdateActiveButtonStyle(i);
        }
    }

    /// <summary>
    /// Sets the UIDocument for the UI controller.
    /// </summary>
    /// <param name="uiDocument">The UIDocument to be set</param>
    private void SetUIDocument(UIDocument uiDocument)
    {
        this.uiDocument = uiDocument;
    }
#if UNITY_EDITOR
    /// <summary>
    /// Creates a new BoidManager UI document in the scene.
    /// </summary>
    [MenuItem("GameObject/Boid System/Create new BoidManager UI document", false, 10)]
    private static void CreateBoidManagerUIController()
    {
        GameObject uiControllerGO = new GameObject("BoidManagerUIController");
        BoidManagerUIController boidManagerUIController =  uiControllerGO.AddComponent<BoidManagerUIController>();
        UIDocument uiDocumnet = uiControllerGO.AddComponent<UIDocument>();
        boidManagerUIController.SetUIDocument(uiDocumnet);
        try
        {
            uiDocumnet.panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/BoidTool/Scripts/UI/UIElements/BoidPanelSettings.asset");
            uiDocumnet.visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BoidTool/Scripts/UI/UIElements/BoidManagerUIRuntime.uxml");
        }
        catch
        {
            Debug.LogError("Some issues arose durring automatic UI Document set up\nYou will need to finish adding the components manually");
        }
    }
#endif
}