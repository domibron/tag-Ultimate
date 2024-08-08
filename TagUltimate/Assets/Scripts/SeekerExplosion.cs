using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class SeekerExplosion : MonoBehaviour
{
	public float MaxRange = 10f;
	public float RageMultForExplosion = 3f;

	public float ExplosionForce = 100f;

	public float CountDown = 3f;
	private float CurrentCountDown;

	private bool CountDownActive = false;

	private PhotonView PV;

	public Image ExplosionCountDownImage;


	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	// Start is called before the first frame update
	void Start()
	{
		if (!PV.IsMine) return;

		CountDownActive = false;
		CurrentCountDown = CountDown;

		ExplosionCountDownImage.gameObject.SetActive(false);

	}

	// Update is called once per frame
	void Update()
	{
		if (!PV.IsMine) return;

		if (CountDownActive && CurrentCountDown >= 0)
		{
			CurrentCountDown -= Time.deltaTime;
		}

		if (CountDownActive)
		{
			ExplosionCountDownImage.gameObject.SetActive(true);

			ExplosionCountDownImage.fillAmount = 1 - (CurrentCountDown / CountDown);
		}


		if (CurrentCountDown <= 0)
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, MaxRange);

			foreach (Collider collider in colliders)
			{
				if (collider.GetComponent<PlayerMovement>() == null) continue;


				if ((int)collider.GetComponent<PhotonView>().Owner.CustomProperties["team"] == 1)
				{
					float damageToTake = (1 - (Vector3.Distance(transform.position, collider.transform.position) / MaxRange)) * collider.GetComponent<PlayerMovement>().MaxHealth + 1;

					collider.GetComponent<IDamageable>()?.TakeDamage(damageToTake);
				}


				//collider.GetComponent<Rigidbody>()?.AddExplosionForce(ExplosionForce, transform.position, MaxRange * RageMultForExplosion);
			}

			GetComponent<PlayerMovement>().Expload(ExplosionForce, transform.position, MaxRange * RageMultForExplosion);

		}



		if (Input.GetKeyDown(KeyCode.E))
		{
			CountDownActive = true;
		}
	}
}
