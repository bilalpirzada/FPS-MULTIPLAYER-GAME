using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
	[SerializeField] Camera cam;

	[SerializeField] GameObject muzzleFlash;
	[SerializeField] Transform muzzleFlashPosition;

	PhotonView PV;



	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	private void Start()
	{
		//muzzleFlashPosition = transform.Find("RifleMuzzleFlash");

	}

	private void Update()
	{
		Aim(Input.GetMouseButton(1));

	}

	


	private void Aim(bool playerIsAiming)
    {

		Transform t_anchor = this.transform.Find("Anchor");
		Transform t_state_ads = this.transform.Find("States/ADS");
		Transform t_state_hip = this.transform.Find("States/Hip");



		
		float aimSpeed = ((GunInfo)itemInfo).aimSpeed;

		//Debug.Log("aimspeed:   " + aimSpeed);


		if (playerIsAiming)
			{

			//aim
			if (t_anchor)
			{
				t_anchor.position = Vector3.Lerp(t_anchor.position,
				t_state_ads.position, Time.deltaTime * aimSpeed);
				Debug.Log("inside if anchor not null");
			}
				//hide the cross hair
				PlayerUIManager.Instance.crossHair.SetActive(false);
			
		}
		else
		{
			//hip
			if (t_anchor)
			{
				t_anchor.position =
					Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.deltaTime * aimSpeed);
			}
				//show the cross hair
				PlayerUIManager.Instance.crossHair.SetActive(true);
		}
		
	}

    public override void Use()
	{
		Shoot();
	}

	
	void Shoot()
	{
		

		//1. use below code if you want to see the muzzle on every player view 
		//Destroy(PhotonNetwork.Instantiate("muzzleFlash", muzzleFlashPosition.position, Quaternion.identity),
			//.03f);
		//PV.RPC("MuzzleFlash", RpcTarget.All);
		//--------------------------------------------  1.

		Destroy(Instantiate(muzzleFlash, muzzleFlashPosition.position, Quaternion.identity), .03f);



		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
		ray.origin = cam.transform.position;
		if(Physics.Raycast(ray, out RaycastHit hit))
		{
			hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
			PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
		}
	}

	

	[PunRPC]
	void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
	{
		

		Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
		if(colliders.Length != 0)
		{
			GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
			Destroy(bulletImpactObj, 10f);
			bulletImpactObj.transform.SetParent(colliders[0].transform);
		}
	}
}
