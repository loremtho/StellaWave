using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChanger : MonoBehaviour
{
    public PlayerController player;
    public GameObject WeaponSelectionUI;

    public WeaponManager theWM;

    public GunController Gun;
    public AxeController Axe;

    public bool hasGun = false;

    // Start is called before the first frame update
    
    public void GunA()
    {
        Transform playerWeapon = player.transform.FindDeepChild("SciFi Assault");
        if (playerWeapon != null)
        {
            
            GameObject WeaponObject = playerWeapon.gameObject;
            if (!WeaponObject.activeSelf)
            {
                DeactivateCurrentWeapon();
                Gun.GunMounting();
                theWM.SetCurrentWeaponType("Rifle");
                WeaponObject.SetActive(true);
                Debug.Log("SciFi Assault 오브젝트를 활성화했습니다.");
                WeaponSelectionUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                hasGun = true;
            }
            else
            {
                Debug.Log("SciFi Assault 오브젝트는 이미 활성화되어 있습니다.");
            }
        } 
        else
        {
            Debug.LogError("SciFi Assault 오브젝트를 찾을 수 없습니다.");
        }

    }

    public void GunB()
    {
        Transform playerWeapon = player.transform.FindDeepChild("Sci fi SMG");
        if (playerWeapon != null)
        {
            
            GameObject WeaponObject = playerWeapon.gameObject;
            if (!WeaponObject.activeSelf)
            {
                DeactivateCurrentWeapon();
                Gun.GunMounting();
                theWM.SetCurrentWeaponType("SMG");
                WeaponObject.SetActive(true);
                Debug.Log("Sci fi SMG 오브젝트를 활성화했습니다.");
                WeaponSelectionUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                hasGun = true;
            }
            else
            {
                Debug.Log("Sci fi SMG 오브젝트는 이미 활성화되어 있습니다.");
            }
        } 
        else
        {
            Debug.LogError("Sci fi SMG 오브젝트를 찾을 수 없습니다.");
        }

    }

    public void AxeA()
    {
        Transform playerWeapon = player.transform.FindDeepChild("Axe");
        if (playerWeapon != null)
        {
            GameObject WeaponObject = playerWeapon.gameObject;
            if (!WeaponObject.activeSelf)
            {
                DeactivateCurrentWeapon();
                Axe.AxeMounting();
                theWM.SetCurrentWeaponType("AXE");
                WeaponObject.SetActive(true);
                Debug.Log("Axe 오브젝트를 활성화했습니다.");
                WeaponSelectionUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if(hasGun)
                {
                    hasGun = false;
                }
            }
            else
            {
                Debug.Log("Axe 오브젝트는 이미 활성화되어 있습니다.");
            }
        } 
        else
        {
            Debug.LogError("Axe 오브젝트를 찾을 수 없습니다.");
        }

    }

    private void DeactivateCurrentWeapon()
    {
    // 현재 무기를 찾아서 비활성화
        Transform currentWeapon = player.transform.FindDeepChild("SciFi Assault");
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = player.transform.FindDeepChild("Sci fi SMG");
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }
    
        // 또는 Axe의 경우
        currentWeapon = player.transform.FindDeepChild("Axe");
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }
} 

    
}
