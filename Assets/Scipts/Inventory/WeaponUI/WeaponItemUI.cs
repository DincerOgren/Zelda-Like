using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class WeaponItemUI : MonoBehaviour
{
    public WeaponConfig config = null;
    [SerializeField] bool isChosen = false,
        isEquipped=false;


    private int index;
    public Image itemIcon;
    private AttackController attackController;
    private TempInventory inventory;
    private WeaponUIController weaponUIController;
    private void Awake()
    {
    }

    private void Update()
    {
        if (isChosen && config!=null && !isEquipped)
        {
            attackController.EquipWeapon(config);
            isEquipped = true;
        }
    }

   
    public void Set()
    {
        isChosen = true;
    }
    public void DeActivate()
    {
        isChosen=false;
        isEquipped = false;
    }
    public void SetupDefault(TempInventory inventory, AttackController attackController,WeaponUIController wp)
    {

        this.inventory = inventory;
        this.attackController = attackController;
        weaponUIController = wp;
        wp.weaponList[0] = this;
    }
    
    public void Setup(TempInventory inventory, AttackController attackController,WeaponUIController wp,int index,int inventoryIndex)
    {
        this.inventory = inventory;
        this.attackController = attackController;
        weaponUIController = wp;
        this.index= index;
        if (inventory.GetItemInSlot(inventoryIndex, SlotType.Weapon) == null)
        {
            itemIcon.sprite = null;
            itemIcon.gameObject.SetActive(false);
            wp.weaponList[index] = null;
            return;
        }
        else
        {
            //var tempItem = inventory.GetItemInSlot(index, SlotType.Weapon);
            //itemIcon.sprite = tempItem.GetIcon();
            //config = tempItem as WeaponConfig;
            //itemIcon.gameObject.SetActive(true);
            config = inventory.GetItemInSlot(inventoryIndex, SlotType.Weapon)as WeaponConfig;
            itemIcon.sprite = config.GetIcon();
            wp.weaponList[index] = this;
            itemIcon.gameObject.SetActive(true);
        }

        
    }
}
