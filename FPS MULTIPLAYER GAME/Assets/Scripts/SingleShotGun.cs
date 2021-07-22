using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
	[SerializeField] Camera cam;
	
	[SerializeField] GameObject muzzleFlash;
	[SerializeField] Transform muzzleFlashPosition;

	PhotonView PV;

	[SerializeField] Animator anim;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

    private void Start()
    {
		//muzzleFlashPosition = transform.Find("RifleMuzzleFlash");
    }

    public override void Use()
	{
		Shoot();
	}

	
	void Shoot()
	{
		//anim.SetTrigger("shoot");
		//anim.SetBool("shoot bool", false);

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
