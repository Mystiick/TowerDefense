﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public GameObject buildingPreview;
    public GameObject tileCover;

    private TowerScriptableObject _currentTower;
    private GameObject _tempTower;

    // Start is called before the first frame update
    void Start()
    {
        ResetBuildingController();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentTower != null)
        {
            UpdatePreview();
        }
    }

    private void UpdatePreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.Buildable))
        {
            // Round the position to the nearest 0.5
            buildingPreview.transform.position = new Vector3(
                hit.point.x - (hit.point.x % .5f),
                hit.point.y - (hit.point.y % .5f),
                hit.point.z - (hit.point.z % .5f)
            );

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 colliderSize = new Vector3(
                    (_currentTower.Width * 5 - .1f) * _tempTower.transform.localScale.x,
                    1 * _tempTower.transform.localScale.y,
                    (_currentTower.Height * 5 - .1f) * _tempTower.transform.localScale.z
                );

                // Look if we are colliding with anything
                var overlaps = Physics.OverlapBox(buildingPreview.transform.position, colliderSize / 2, Quaternion.identity, LayerMask.Default);

                if (overlaps.Length == 0)
                {
                    // Mouse has been pressed, copy building preview to a real tower
                    var go = Instantiate(_tempTower);
                    go.transform.position = buildingPreview.transform.position;

                    var bc = go.AddComponent<BoxCollider>();
                    bc.size = colliderSize;
                    bc.center = Vector3.zero;
                }
            }
        }
    }

    public void SetPreview(TowerScriptableObject tower)
    {
        ResetBuildingController();
        _currentTower = tower;

        _tempTower = AddChild(buildingPreview, _currentTower.PrefabToRender);
        _tempTower.transform.localScale = _currentTower.Scale;

        var tc = AddChild(_tempTower, this.tileCover);
        // Bump up the Y axis 0.01, to prevent Y fighting
        tc.transform.localPosition = new Vector3(0, .01f, 0);
    }

    private GameObject AddChild(GameObject parent, GameObject child)
    {
        GameObject go = Instantiate(child);
        go.transform.parent = parent.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        return go;
    }

    private void ResetBuildingController()
    {
        foreach (Transform c in buildingPreview.transform)
        {
            Destroy(c.gameObject);
        }
    }
}