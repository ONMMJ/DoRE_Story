using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player_Director : MonoBehaviour
{
	public GameObject scenePanel;

	[Header("Player Speed")]
	float speedDefault = 5;
	public float movePower;
	public float jumpPower;
	public float dashPower;

	[Header("Player Status")]
	public int maxHp;
	int hp;
	public int damage;
	public int defence;
	public int maxJumpCount = 2;
	public int maxDashCount = 2;
	public int coin;
	public int upgradeStone;

	[Header("Prefab")]
	public GameObject AttackPrefab;


	[Header("etc")]
	public GameObject interaction;
	Transform movingGround;
	Vector3 distance;
	Rigidbody2D rb;
	Animator animator;
	public HpBar hpBar;

	bool isDash = false;
	bool isPause = false;
	bool isDead = false;
	bool isGodMode = false;
	bool isMovingGround = false;
	int jumpCount = 0;
	int dashCount = 0;
	float dashCTime = 1f;
	float time = 0;


	Dictionary<KeyCode, Action> keyDictionary;

	void Start()
	{
		rb = gameObject.GetComponent<Rigidbody2D>();
		animator = transform.Find("Image").GetComponent<Animator>();
		hpBar = FindObjectOfType<HpBar>();

		keyDictionary = new Dictionary<KeyCode, Action>
		{
			{ KeyCode.Space, Jump },
			{ KeyCode.G, Interaction },
			{ KeyCode.LeftControl, Attack }
		};


		hp = maxHp;
	}

	void Update()
	{
		time += Time.deltaTime;

		if (isPause || isDead)		//피격 중 동작불가
			return;

		if (Input.GetKeyDown(KeyCode.LeftShift) && dashCount < maxDashCount)
			StartCoroutine(Dash());

		if (!isDash)
		{
			if (isMovingGround)
				MovingGroundMove();
			Move();
		}

		if (time >= dashCount)
			dashCount = 0;

		if (Input.anyKeyDown)
		{
			foreach (var dic in keyDictionary)
			{
				if (Input.GetKeyDown(dic.Key))
				{
					dic.Value();
				}
			}
		}
	}
	IEnumerator Dash()
	{
		isDash = true;
		isGodMode = true;
		animator.SetTrigger("DashTrigger");
		dashCount++;
		time = 0;
		Vector3 myScale = transform.localScale.normalized;
		rb.velocity = new Vector3(myScale.x * dashPower, 0, 0);
		yield return new WaitForSeconds(0.1f);
		if (isMovingGround)
			distance = movingGround.position - transform.position;
		rb.velocity = new Vector3(0, 0, 0);
		isGodMode = false;
		isDash = false;
	}

	void MovingGroundMove()
    {
		transform.position = movingGround.position - distance;
    }
	void Move()
	{
		Vector3 moveVelocity = Vector3.zero;
		if (Input.GetAxisRaw("Horizontal") < 0)
		{
			moveVelocity = Vector3.left;
			transform.localScale = new Vector3(-1, 1, 1);
			animator.SetBool("IsWalk", true);
		}

		else if (Input.GetAxisRaw("Horizontal") > 0)
		{
			moveVelocity = Vector3.right;
			transform.localScale = new Vector3(1, 1, 1);
			animator.SetBool("IsWalk", true);
		}
        else
			animator.SetBool("IsWalk", false);

		transform.position += moveVelocity * speedDefault * movePower * Time.deltaTime;
		
		if(isMovingGround)
			distance = movingGround.position - transform.position;
	}

	void Jump()
	{
		if (jumpCount >= maxJumpCount)
			return;
		animator.SetBool("IsJump", true);
		jumpCount++;
		rb.velocity = Vector2.zero;

		Vector2 jumpVelocity = new Vector2(0, jumpPower);
		rb.AddForce(jumpVelocity, ForceMode2D.Impulse);
	}

	void Interaction()
	{
		if (interaction == null)
			return;
		Interaction_Inf interaction_Inf = interaction.GetComponent<Interaction_Inf>();
		if (interaction_Inf.type == "trash can")
		{

		}
		if (interaction_Inf.type == "Portal")
		{
			SceneManager.LoadScene(interaction_Inf.mapName);
			transform.position = interaction_Inf.spawnPos;
		}

		if (interaction_Inf.isHideUI)
			UI.HideUI();
		else
			UI.ShowUI();
	}
	void Attack()
    {
		animator.SetTrigger("AttackTrigger");
		Transform attackPrefab = Instantiate(AttackPrefab).transform;
		attackPrefab.parent = gameObject.transform;
		attackPrefab.localPosition = Vector3.right * 2;
		attackPrefab.GetComponent<Attack_Inf>().Damage = damage;
		attackPrefab.localRotation = Quaternion.Euler(Vector3.zero);
		StartCoroutine(Delay(0.3f));
	}

	public void Attacked(int enemyDamage)
    {
		if (isGodMode)
			return;
		hp -= enemyDamage;
		SetHp();
		if (hp>0)
			StartCoroutine(Attacked(2f));
		else
			StartCoroutine(Dead());
	}

	public void SetHp()
    {
		hpBar.SetHp(hp, maxHp);
	}
    public void LandGround()
    {
		jumpCount = 0;
		animator.SetBool("IsJump", false);
	}
	public void MovingGround(bool isLand, Transform other)
	{
		isMovingGround = isLand;
		movingGround = other;
		distance = movingGround.position - transform.position;
	}
	public void MovingGround(bool isLand)
	{
		isMovingGround = isLand;
	}

	void SetPosition(Vector3 myPosition)
    {
		transform.position = myPosition;
    }
	void SetPosition(float x, float y)
	{
		transform.position = new Vector3(x, y, 0);
	}
	IEnumerator Delay(float time)
    {
		isPause = true;
		yield return new WaitForSeconds(time);
		isPause = false;
	}
	IEnumerator Dead()
    {
		yield return StartCoroutine(FadeOut(2f));
		SceneManager.LoadScene("TestScene");
		SetPosition(-16.29f, -6.87f);
		yield return StartCoroutine(FadeIn(2f));
	}
	IEnumerator Attacked(float GodTime)
	{
		isPause = true;
		isGodMode = true;
		//animator.SetBool("isAttacked", true);
		rb.velocity = new Vector2(transform.localScale.x * -25, 0);
		yield return new WaitForSeconds(0.2f);
		rb.velocity = new Vector2(0, 0);
		isPause = false;
		//animator.SetBool("isAttacked", false);
		yield return new WaitForSeconds(GodTime);   //무적시간
		isGodMode = false;
	}

	IEnumerator FadeOut(float fadeOutTime)
	{
		isDead = true;
		isGodMode = true;
		Image image = scenePanel.GetComponent<Image>();
		Color color = image.color;
		while (color.a < 1f)
		{
			color.a += Time.deltaTime / fadeOutTime;
			image.color = color;

			if (color.a >= 1f) color.a = 1f;

			yield return null;
		}
	}
	IEnumerator FadeIn(float fadeInTime)
	{
		Image image = scenePanel.GetComponent<Image>();
		Color color = image.color;
		while (color.a > 0f)
		{
			color.a -= Time.deltaTime / fadeInTime;
			image.color = color;

			if (color.a <= 0f) color.a = 0f;

			yield return null;
		}
		isDead= false;
		isGodMode = false;
	}
}