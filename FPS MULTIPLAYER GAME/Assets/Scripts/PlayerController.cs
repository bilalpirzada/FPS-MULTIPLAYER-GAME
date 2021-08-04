using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
	[SerializeField] Image healthbarImage;
	[SerializeField] GameObject ui;

	[SerializeField] GameObject cameraHolder;

	[SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

	[SerializeField] Item[] items;

	[SerializeField] GameObject hitScreen_Object;

	

	int itemIndex;
	int previousItemIndex = -1;
	[Range(0,255)]
	[SerializeField] int redScreenIntensity;

	float verticalLookRotation;
	bool grounded;
	Vector3 smoothMoveVelocity;
	Vector3 moveAmount;

	Rigidbody rb;

	PhotonView PV;

	const float maxHealth = 100f;
	float currentHealth = maxHealth;

	PlayerManager playerManager;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();


		if (PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>() != null)
		{
			playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
		}
	}

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;

		if (PV.IsMine)
		{
			EquipItem(0);
		}
		else
		{
			Destroy(GetComponentInChildren<Camera>().gameObject);
			//Destroy(rb);
			Destroy(ui);
		}
	}

	

	void Update()
	{
		


		if(!PV.IsMine)
			return;

		Look();
		Move();
		Jump();
	

		fadeRedScreen();

		for(int i = 0; i < items.Length; i++)
		{
			if(Input.GetKeyDown((i + 1).ToString()))
			{
				EquipItem(i);
				break;
			}
		}

		if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
		{
			if(itemIndex >= items.Length - 1)
			{
				EquipItem(0);
			}
			else
			{
				EquipItem(itemIndex + 1);
			}
		}
		else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
		{
			if(itemIndex <= 0)
			{
				EquipItem(items.Length - 1);
			}
			else
			{
				EquipItem(itemIndex - 1);
			}
		}

		if(Input.GetMouseButtonDown(0))
		{
			items[itemIndex].Use();
		}

		if(transform.position.y < -10f) // Die if you fall out of the world
		{
			Die();
		}
	}

 

    private void fadeRedScreen()
    {
        if(hitScreen_Object != null)
        {
			if(hitScreen_Object.GetComponent<Image>().color.a>0)
            {
				var color = hitScreen_Object.GetComponent<Image>().color;
				color.a -= 0.001f;
				hitScreen_Object.GetComponent<Image>().color = color;
            }
        }
    }

    void Look()
	{
		transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

		verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60f, 90f);

		cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
	}

	void Move()
	{
		Debug.Log("inside move function");
		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

		moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
	}

	void Jump()
	{
		if(Input.GetKeyDown(KeyCode.Space)  )
		{
			rb.AddForce(transform.up * jumpForce);

		}

		if(!grounded)
        {
			this.GetComponent<Rigidbody>().useGravity = true;
        }
        else { this.GetComponent<Rigidbody>().useGravity = false; }

	}

	void EquipItem(int _index)
	{
		if(_index == previousItemIndex)
			return;

		itemIndex = _index;

		items[itemIndex].itemGameObject.SetActive(true);

		if(previousItemIndex != -1)
		{
			items[previousItemIndex].itemGameObject.SetActive(false);
		}

		previousItemIndex = itemIndex;

		if(PV.IsMine)
		{
			Hashtable hash = new Hashtable();
			hash.Add("itemIndex", itemIndex);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		}
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if(!PV.IsMine && targetPlayer == PV.Owner)
		{
			EquipItem((int)changedProps["itemIndex"]);
		}
	}

	public void SetGroundedState(bool _grounded)
	{
		grounded = _grounded;
	}

	void FixedUpdate()
	{
		if(!PV.IsMine)
			return;

		rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}

	public void TakeDamage(float damage)
	{
		PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);

	
	}

	[SerializeField] float bloodIntensity=0.1f;

	// display red screen on hit by bullet from enemy
	void showRedScreenOnHit()
	{
		//showing red screen on hit by enemy
		var color = hitScreen_Object.GetComponent<Image>().color;
		color.a = bloodIntensity;
		hitScreen_Object.GetComponent<Image>().color = color;
	}


	[PunRPC]
	void RPC_TakeDamage(float damage)
	{
		if(!PV.IsMine)
			return;

		showRedScreenOnHit();

		currentHealth -= damage;

		healthbarImage.fillAmount = currentHealth / maxHealth;

		if(currentHealth <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		playerManager.Die();
	}
}