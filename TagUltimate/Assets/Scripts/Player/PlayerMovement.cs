using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour, IDamageable
{
	public Rigidbody rb;

	public float PlayerSpeed = 2f;

	public float PlayerAcceleration = 1f;

	public float PlayerDeacceleration = 1f;

	public float BoostForce = 10f;
	public float BoostCooldown = 3f;

	private float boostWaitTime = 0f;

	public Image BoostImage;

	public Transform Orientation;

	private PhotonView PV;

	public bool IsGrounded = false;

	private Vector3 normal;

	public Vector3 GravityVector = new Vector3(0, -9.81f, 0);

	public float MaxHealth = 100f;
	public float CurrentHealth = 100f;

	private PlayerManager PlayerManagerForPlayer;

	// Start is called before the first frame update
	void Awake()
	{
		rb = GetComponent<Rigidbody>();


		PV = GetComponent<PhotonView>();


		PlayerManagerForPlayer = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
	}

	void Start()
	{
		if (!PV.IsMine) return;

		CurrentHealth = MaxHealth;
	}


	void Update()
	{

		if (!PV.IsMine) return;

		HandleGroundCheck();
		HandleGravity();

		Vector3 move = Orientation.right * Input.GetAxisRaw("Horizontal") + Orientation.forward * Input.GetAxisRaw("Vertical");

		Vector3 rbVelWithNoY = new Vector3(rb.velocity.x, 0, rb.velocity.z);



		if (move.normalized.magnitude != 0)
		{
			move = ((move.normalized * PlayerSpeed) - rbVelWithNoY) * PlayerAcceleration;

			if (IsGrounded) move = Vector3.ProjectOnPlane(move, normal);

			rb.AddForce(move, ForceMode.Force);
		}
		else
		{
			Vector3 deAcceleration = -rbVelWithNoY * PlayerDeacceleration;

			if (IsGrounded) deAcceleration = Vector3.ProjectOnPlane(deAcceleration, normal);

			rb.AddForce(deAcceleration, ForceMode.Force);
		}


		BoostImage.fillAmount = 1 - (boostWaitTime / BoostCooldown);


		if (boostWaitTime >= 0) boostWaitTime -= Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Space) && boostWaitTime <= 0)
		{
			rb.AddForce(Orientation.forward * BoostForce, ForceMode.Impulse);

			boostWaitTime = BoostCooldown;
		}
	}


	void HandleGravity()
	{
		if (rb.useGravity) return;

		if (!IsGrounded) rb.AddForce(Physics.gravity, ForceMode.Force);

		else if (rb.velocity.y >= Physics.gravity.y) rb.AddForce(Physics.gravity, ForceMode.Force);
	}

	void HandleGroundCheck()
	{
		if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f))
		{
			IsGrounded = true;

			normal = hit.normal;
		}
		else
		{
			IsGrounded = false;


		}
	}

	public void TakeDamage(float damage)
	{
		PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
	}

	[PunRPC]
	void RPC_TakeDamage(float damage, PhotonMessageInfo info)
	{
		if ((int)PV.Owner.CustomProperties["team"] == 0) return;

		CurrentHealth -= damage;

		//healthbarImage.fillAmount = currentHealth / maxHealth;

		if (CurrentHealth <= 0)
		{
			Die();
			//PlayerManager.Find(info.Sender).GetKill();
		}
	}

	public void Die() // function to call to kill player
	{
		PlayerManagerForPlayer.Die();
	}

	public void Expload(float force, Vector3 pos, float range)
	{
		PV.RPC(nameof(AddExploForce), RpcTarget.All, force, pos, range);
	}

	[PunRPC]
	void AddExploForce(float force, Vector3 pos, float range)
	{
		rb.AddExplosionForce(force, pos, range);
	}
}


