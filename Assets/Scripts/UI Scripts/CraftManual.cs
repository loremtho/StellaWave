using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Craft
{
    public string craftName; //�̸�
    public Sprite craftImage; //�̹���
    public string craftDesc; //���� 
    public string[] craftNeedItem; //�ʿ��� ������
    public int[] craftNeedItemCount; //�ʿ��� �������� ����.
    public GameObject go_Prefab; //���� ��ġ�� ������
    public GameObject go_PreviewPrefab; // �̸����� ������
}

public class CraftManual : MonoBehaviour
{

    //���º���
    public static bool isCraftActivated = false;
    private bool isPreviewActivated = false;

    [SerializeField]
    private GameObject go_BaseUI; //�⺻ ���̽� UI

    private int tabNumber = 0;
    private int page = 1;
    private int selectedSlotNumber;
    private Craft[] craft_SelectedTab;

    [SerializeField]
    private Craft[] craft_fire; //��ںҿ� ��.
    [SerializeField]
    private Craft[] craft_build; //����� ��

    private GameObject go_Preview; //�̸����� �������� ���� ����
    private GameObject go_Prefab; //���� ������ �������� ���� ����

    [SerializeField]
    private Transform tf_Player; //�÷��̾� ��ġ

    //Raycast �ʿ� ���� ����
    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float range;

    //�ʿ��� UI Slot ���
    [SerializeField]
    private GameObject[] go_Slots;
    [SerializeField]
    private Image[] image_Slot;
    [SerializeField]
    private Text[] text_SlotName;
    [SerializeField]
    private Text[] text_SlotDesc;
    [SerializeField]
    private Text[] text_SlotNeedItem;

    //�ʿ��� ������Ʈ
    private Inventory theInventory;

    private QuickSlotController theQuickSlot;

    public bool Stopattak = false;

    private void Start()
    {
        theInventory = FindObjectOfType<Inventory>();
        tabNumber = 0;
        page = 1;
        TabSlotSetting(craft_fire);
    }

    public void TabSetting(int _tabNumber)
    {
        tabNumber = _tabNumber;
        page = 1;

        switch(tabNumber)
        {
            case 0:
                TabSlotSetting(craft_fire); //�Ҽ���
                break;
            case 1:
                TabSlotSetting(craft_build); //���༼��
                break;
       
        }
    }

    private void ClearSlot()
    {
        for (int i = 0; i < go_Slots.Length; i++)
        {
            image_Slot[i].sprite = null;
            text_SlotDesc[i].text = "";
            text_SlotDesc[i].text = "";
            text_SlotNeedItem[i].text = "";
            go_Slots[i].SetActive(false);

        }
    }

    public void RightPageSetting()
    {
        if(page < (craft_SelectedTab.Length/go_Slots.Length) +1 )
        {
            page++;
        }
        else
        {
            page = 1;
        }

        TabSlotSetting(craft_SelectedTab);
    }

    public void LeftPageSetting()
    {
        if(page != 1)
        {
            page--;
        }
        else
        {
            page = (craft_SelectedTab.Length / go_Slots.Length) + 1;
        }
            

        TabSlotSetting(craft_SelectedTab);
    }

    private void TabSlotSetting(Craft[] _craft_tab)
    {
        ClearSlot();
        craft_SelectedTab = _craft_tab;

        int startSlotNumber = (page - 1) * go_Slots.Length; // 4�� ���

       for(int i =startSlotNumber; i<craft_SelectedTab.Length; i++ )
        {
            if( i== page * go_Slots.Length)
            {
                break;
            }

            go_Slots[i - startSlotNumber].SetActive(true);

            image_Slot[i - startSlotNumber].sprite = _craft_tab[i].craftImage;
            text_SlotName[i - startSlotNumber].text = craft_SelectedTab[i].craftName;
            text_SlotDesc[i - startSlotNumber].text = craft_SelectedTab[i].craftDesc;

            for (int x = 0; x < craft_SelectedTab[i].craftNeedItem.Length; x++)
            {
                text_SlotNeedItem[i - startSlotNumber].text += craft_SelectedTab[i].craftNeedItem[x];
                text_SlotNeedItem[i - startSlotNumber].text += " x " + craft_SelectedTab[i].craftNeedItemCount[x] + "\n";
            }
        }
    }

    public void SlotClick(int _slotNumber)
    {
        selectedSlotNumber = _slotNumber + (page - 1) * go_Slots.Length;

        if(!CheckIngredient())
        {
            return;
        }
     

        go_Preview = Instantiate(craft_SelectedTab[selectedSlotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_SelectedTab[selectedSlotNumber].go_Prefab;
        isPreviewActivated  = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        go_BaseUI.SetActive(false);
    }
    
    private bool CheckIngredient()
    {
        for (int i = 0; i < craft_SelectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            if (theInventory.GetItemCount(craft_SelectedTab[selectedSlotNumber].craftNeedItem[i]) < craft_SelectedTab[selectedSlotNumber].craftNeedItemCount[i])
            {
                return false;
            }
        }


        return true;

    }

    private void UseIngredient()
    {
        for (int i = 0; i < craft_SelectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            theInventory.SetItemCount(craft_SelectedTab[selectedSlotNumber].craftNeedItem[i], craft_SelectedTab[selectedSlotNumber].craftNeedItemCount[i]);
       
        }

    }



    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
        {
            Window();
            Stopattak = true;
            
        }

        if(isPreviewActivated)
        {
            PreviewPositionUpdate();
        }

        if(Input.GetButtonDown("Fire1"))
        {
            Build();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
            Stopattak = false;
        }
    }

    private void Build()
    {
        if(isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            UseIngredient();
            Instantiate(go_Prefab, go_Preview.transform.position,go_Preview.transform.rotation);
            Destroy(go_Preview);
            isCraftActivated = false;
            isPreviewActivated= false;
            go_Preview= null;
            go_Prefab= null;
            Stopattak = false;
        }
    }

    private void PreviewPositionUpdate()
    {

        if(Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo, range, layerMask))
        {
            if(hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;

                if(Input.GetKeyDown(KeyCode.Q))
                {
                    go_Preview.transform.Rotate(0, -90f, 0f);
                }
                else if(Input.GetKeyDown(KeyCode.E))
                {
                    go_Preview.transform.Rotate(0, +90f, 0f);
                }

                _location.Set(Mathf.Round(_location.x), Mathf.Round(_location.y / 0.1f)* 0.1f, Mathf.Round(_location.z));

                go_Preview.transform.position = _location;
            }
        }
    }

    private void Cancel()
    {
        if(isPreviewActivated)
        {
            Destroy(go_Preview);
        }
        

        isCraftActivated= false;
        isPreviewActivated= false;
        go_Preview= null;
        go_Prefab = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        go_BaseUI.SetActive(false);
    }

    private void Window()
    {
        if(!isCraftActivated)
        {
            OpenWindow();
        }
        else
        {
            CloseWindow();
        }
    }

    private void OpenWindow()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCraftActivated= true;
        go_BaseUI.SetActive(true);
    }

    private void CloseWindow()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCraftActivated= false;
        go_BaseUI.SetActive(false);
    }

    
}
