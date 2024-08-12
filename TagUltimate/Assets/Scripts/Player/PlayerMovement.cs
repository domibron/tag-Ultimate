using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

public class PlayerMovement : MonoBehaviour, IDamageable
{
	#region Variables
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

	public PlayerManager PlayerManagerForPlayer;

	public Image HealthImage;

	public float CollisionHitDamage = 10f;

	public float MinSpeedToDamage = 5f;

	[Range(0, 1)]
	public float PercentileDamageRange = 0.5f;

	public Transform PlayerBody;

	private Vector3 ballRightDir;

	private bool isDead = false;

	#endregion

	#region Awake
	// Start is called before the first frame update
	void Awake()
	{
		rb = GetComponent<Rigidbody>();


		PV = GetComponent<PhotonView>();


		PlayerManagerForPlayer = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
	}
	#endregion

	#region Start
	void Start()
	{
		if (!PV.IsMine) return;

		CurrentHealth = MaxHealth;

		//if ((int)PV.Owner.CustomProperties["team"] == 0) HealthImage.gameObject.SetActive(false);
		if ((int)PV.Owner.CustomProperties["team"] == 1) HealthImage.gameObject.SetActive(true);
	}
	#endregion

	#region Update
	void Update()
	{

		Vector3 ballDir = rb.velocity.normalized;
		ballRightDir = Vector3.Cross(Vector3.up, ballDir);

		float dist = rb.velocity.magnitude * Time.fixedDeltaTime;
		float alpha = (dist * 180.0f) / (Mathf.PI * 0.37f);
		PlayerBody.Rotate(ballRightDir, alpha, Space.World);

		if (!PV.IsMine) return;

		HandleGroundCheck();
		HandleGravity();




		BoostImage.fillAmount = 1 - (boostWaitTime / BoostCooldown);


		if (boostWaitTime >= 0) boostWaitTime -= Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Space) && boostWaitTime <= 0)
		{
			rb.AddForce(Orientation.forward * BoostForce, ForceMode.Impulse);

			boostWaitTime = BoostCooldown;
		}

		if ((int)PV.Owner.CustomProperties["team"] == 1)
		{
			HealthImage.fillAmount = CurrentHealth / MaxHealth;
		}

		if (CurrentHealth <= 0 && !isDead)
		{
			Die();
			//PlayerManager.Find(info.Sender).GetKill();
		}

		if (KillZone.Current == null) return;

		if (transform.position.y <= KillZone.Current.YLevel && !isDead)
		{
			Die();
		}
	}
	#endregion

	#region FixedUpdate
	void FixedUpdate()
	{
		if (!PV.IsMine) return;

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
	}
	#endregion

	#region HandleGravity
	void HandleGravity()
	{
		if (rb.useGravity) return;

		if (!IsGrounded) rb.AddForce(GravityVector, ForceMode.Force);

		else if (rb.velocity.y >= GravityVector.y) rb.AddForce(GravityVector, ForceMode.Force);
	}
	#endregion

	#region HandleGroundCheck
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
	#endregion

	#region TakeDamage
	public void TakeDamage(float damage)
	{
		PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
	}
	#endregion

	#region RPC_TakeDamage
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
	#endregion

	#region Die
	public void Die() // function to call to kill player
	{
		if (isDead) return;
		isDead = true;

		if ((int)PV.Owner.CustomProperties["team"] == 0)
		{
			PlayerManagerForPlayer.SpawnObject(Path.Combine("PhotonPrefabs", "Player Death", "Seeker"), transform.position, transform.rotation);
		}
		else if ((int)PV.Owner.CustomProperties["team"] == 1)
		{
			PlayerManagerForPlayer.SpawnObject(Path.Combine("PhotonPrefabs", "Player Death", "Hider"), transform.position, transform.rotation);
		}

		PlayerManagerForPlayer.Die();
	}
	#endregion

	#region Expload
	public void Expload(float force, Vector3 pos, float range)
	{
		PV.RPC(nameof(RPC_AddExploForce), RpcTarget.Others, force, pos, range);

		Die();
	}
	#endregion

	#region OnCollisionEnter
	void OnCollisionEnter(Collision other)
	{
		if ((int)PV.Owner.CustomProperties["team"] == 1) return;

		int team = -1;
		if (other.gameObject.GetComponent<PhotonView>() != null)
		{
			team = (int)other.gameObject.GetComponent<PhotonView>().Owner.CustomProperties["team"];
		}
		else
		{
			return;
		}

		if (team == 0) return;


		if (other.relativeVelocity.magnitude >= MinSpeedToDamage)
		{
			float percentDamage = other.relativeVelocity.magnitude / (PlayerSpeed - (PlayerSpeed * PercentileDamageRange));
			other.gameObject.GetComponent<IDamageable>()?.TakeDamage(CollisionHitDamage * percentDamage);
		}

	}
	#endregion

	#region RPC_AddExploForce
	[PunRPC]
	void RPC_AddExploForce(float force, Vector3 pos, float range)
	{
		rb.AddExplosionForce(force, pos, range);
	}
	#endregion

	#region AddForce
	public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
	{
		PV.RPC(nameof(RPC_AddForce), PV.Owner, force, forceMode);
	}
	#endregion

	#region RPC_AddForce
	[PunRPC]
	void RPC_AddForce(Vector3 force, ForceMode forceMode)
	{
		rb.AddForce(force, forceMode);
	}
	#endregion
}


