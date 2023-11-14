using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    public PlayerItem localPlayerItem;

    private PhotonView photonView;

    [SerializeField] private Column[] startPanel;
    [SerializeField] private Column[] lobbyPanel;
    [SerializeField] private Column[] roomPanel;

    [SerializeField] private GameObject arrow;

    [SerializeField] private int columnsIndex;
    [SerializeField] private int rowIndex;

    [SerializeField] private Column[] columns;

    private int currentRoom = 0;
    private void Awake()
    {
        instance = this;

        photonView = GetComponent<PhotonView>();
    }
    private void Start()
    {
        ChangeColumns(0);
    }
    public void Select(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;

        if (context.phase == InputActionPhase.Performed)
        {
            columns[columnsIndex].rows[rowIndex].GetComponentInParent<Button>().onClick.Invoke();
        }
    }

    public void IncreaseColumn(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;

        if (columnsIndex + 1 >= columns.Length) return;

        if (context.phase == InputActionPhase.Performed)
        {
            columnsIndex++;
            while (rowIndex >= columns[columnsIndex].rows.Length) rowIndex--;

            if (currentRoom == 2 && rowIndex == 0)
            {
                localPlayerItem.OnClickRightArrow();
            }

            if (currentRoom == 2 && columnsIndex == 1 && rowIndex == 1) columnsIndex++;

            ChangeArrowPosition(columnsIndex, rowIndex);
        }
    }

    public void DecreaseColumn(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;

        if (columnsIndex - 1 < 0) return;

        if (context.phase == InputActionPhase.Performed)
        {
            columnsIndex--;
            while (rowIndex >= columns[columnsIndex].rows.Length) rowIndex--;

            if (currentRoom == 2 && rowIndex == 0)
            {
                localPlayerItem.OnClickLeftArrow();
            }

            if (currentRoom == 2 && columnsIndex == 1 && rowIndex == 1) columnsIndex--;

            ChangeArrowPosition(columnsIndex, rowIndex);
        }
    }

    public void IncreaseRow(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;

        if (rowIndex + 1 >= columns[columnsIndex].rows.Length) return;

        if (context.phase == InputActionPhase.Performed)
        {
            rowIndex++;

            ChangeArrowPosition(columnsIndex, rowIndex);
        }
    }

    public void DecreaseRow(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;
        
        if (rowIndex - 1 < 0) return;

        if (context.phase == InputActionPhase.Performed)
        {
            rowIndex--;

            ChangeArrowPosition(columnsIndex, rowIndex);
        }
    }

    public void ChangeArrowPosition(int m_columns, int m_row)
    {
        arrow.SetActive(false);

        arrow = columns[m_columns].rows[m_row];

        arrow.SetActive(true);
    }

    public void ChangeIndex(int m_columns, int m_row)
    {
        columnsIndex = m_columns;

        rowIndex = m_row;

        ChangeArrowPosition(m_columns, rowIndex);
    }
    public void ChangeColumns(int m_newColumnsIndex)
    {
        if (m_newColumnsIndex == 0)
            columns = startPanel;
        else if (m_newColumnsIndex == 1)
            columns = lobbyPanel;
        else if (m_newColumnsIndex == 2)
            columns = roomPanel;

        rowIndex = 0;
        columnsIndex = 0;
        currentRoom = m_newColumnsIndex;

        if (currentRoom == 2) columnsIndex = 1;

        ChangeArrowPosition(columnsIndex, rowIndex);
    }
}

[Serializable]
public struct Column
{
    public GameObject[] rows;
}
