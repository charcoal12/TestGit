using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering;

public enum eBarteriaSTATE
{
	NULL	,
	Start	,
	Live	,
	Die		,
};

public enum _eEffectSTATE
{
	NULL		,
	//가루 이펙트
	ASH			,
	//독에			
	Poison		,
	//불에
	Fire		,
};

public enum _eBarteriaSkill
{
	Awake		,
	Start		,
	Update		,
	Return		,
};

public abstract class cBarteriaBase : MonoBehaviour {

	public bool _isActive = true;
	//[HideInInspector]
	public float _nHp;
	public float _nMaxHP;
	public float _Damager;
	public Transform _bodyTransform;
	public _eBarteriaSkill _skill;
	//
	protected AudioSource _sound;

	public float _gold = 0;
	//
	//hp bar
	//public UIGrid _hpgrid;

	public Renderer _hpgauge;
	//gameObject.GetComponent<Renderer>().material.SetFloat("_Progress", i);
	//public float _nhpconst;

	public eBarteriaSTATE _state;
	public _eEffectSTATE _Effectstate;
	public eBarteriaNUMBER _number;

	protected Vector3 _scale = Vector3.one;
	protected Vector2 _settingScale = new Vector3(1,1.1f,1);

	protected Vector3 reflectVector; 
	protected Vector3 _velocity = Vector2.zero;

	protected Rigidbody2D _rigidbody;

	public virtual void _PhysicMove()
	{
		if (_rigidbody.velocity.magnitude >= 40) {
			_rigidbody.velocity /= 2;
		}
	}

	public float _GetGold()
	{
		return _gold;
	}

	protected float _notmove;

	public virtual void _virtualAnimation()
	{
		
		switch (_Effectstate) {

		case _eEffectSTATE.ASH:
			_sound.volume = 1;

			if (_nHp <= 0) {

				GameObject obj = cAshPoolManager._GetInst ()._GetAshEffect ();

				if (obj != null) {

					obj.transform.localScale = this.transform.localScale;
					obj.transform.position = this.transform.position;

					obj.SetActive (true);

				}

				_state = eBarteriaSTATE.Die;
			}
				
			break;

		case _eEffectSTATE.Poison:

			_sound.volume = 1;

			_sound.PlayOneShot (cMainGame._GetInst ()._ManagerSound._Bac_Poision);
			_Effectstate = _eEffectSTATE.NULL;

			break;
		case _eEffectSTATE.Fire:

			_sound.volume = 1;

			if (_nHp <= 0) {

				_Effectstate = _eEffectSTATE.ASH;
				break;

			}
			_sound.PlayOneShot (cMainGame._GetInst ()._ManagerSound._Bac_Fire);
			_Effectstate = _eEffectSTATE.NULL;

			break;
		}


		switch (_state) {

		case eBarteriaSTATE.NULL:

			//if (_hpgrid.gameObject.activeSelf == true) {
			//	_hpgrid.gameObject.SetActive (false);
			//}

			break;

		case eBarteriaSTATE.Start:

			_sound.volume = 1;

			_sound.PlayOneShot (cMainGame._GetInst ()._ManagerSound._BacInPipeOut ());
			//_hpgrid.gameObject.SetActive (true);

			break;

		case eBarteriaSTATE.Live:

			if (_nHp <= 0) {
				_SetState (eBarteriaSTATE.Die);
			}

			break;
		case eBarteriaSTATE.Die:
			
			//_sound.PlayOneShot (cMainGame._GetInst ()._ManagerSound._Bac_DeathRandom ());

		//	_sound.clip = cMainGame._GetInst ()._ManagerSound._Bac_DeathRandom ();
			//_sound.Play ();

			cMainGame._GetInst ()._waveManager._monsterLive--;
			cMainGame._GetInst ()._waveManager._killMonster++;

			cMainGame._GetInst ()._BarteriaKillGoldAdd (_gold);

			GameObject obj = null;
			for (int i = 0; i < 3; i++) {
				switch (Random.Range (0, 3)) {
				case 0:
					obj = cObjectPool.SharedInstance.GetPooledObject ("MonsterDamager1");


					break;
				case 1:
					obj = cObjectPool.SharedInstance.GetPooledObject ("MonsterDamager2");


					break;
				case 2:
					obj = cObjectPool.SharedInstance.GetPooledObject ("MonsterDamager3");


					break;
				}

				if (obj != null) {
					obj.transform.position = this.transform.position + new Vector3 (Random.Range (-0.1f, 0.1f), Random.Range (-0.1f, 0.1f), 0);
					obj.SetActive (true);
					//obj.GetComponent<AudioSource> ().PlayOneShot (cMainGame._GetInst ()._ManagerSound._Bac_DeathRandom ());
				}
			}
			if (obj != null) {
				obj.GetComponent<AudioSource> ().PlayOneShot (cMainGame._GetInst ()._ManagerSound._Bac_DeathRandom ());
			}
			cWasteBasketManager._GetInst ()._ObjectInMonster (this);

			break;
		}
	}



	public void _HpUp(float healing)
	{
		//Debug.Log (_sound + "   ");
		
		_sound.PlayOneShot (cMainGame._GetInst ()._ManagerSound._Bac_Recovery);

		if (_nHp >= _nMaxHP) {
			return;
		}

		float healingValue = _nHp + healing;

		if (healingValue > _nMaxHP) {

			_nHp += _nMaxHP - _nHp;

		} else {

			_nHp += healing;

		}

		if(_hpgauge != null)
			_hpgauge.material.SetFloat("_Progress", (float)(_nHp / _nMaxHP));

	}
	public void _HpDown(float damager, _eEffectSTATE state = _eEffectSTATE.NULL)
	{
		_nHp -= damager;

		_Effectstate = state;

		//if (_hpgrid == null)
		//	return;
		if(_hpgauge != null)
			_hpgauge.material.SetFloat("_Progress", (float)(_nHp / _nMaxHP));

	}

	public virtual void _Init()
	{
		_nHp = _nMaxHP;
		_state = eBarteriaSTATE.Start;
		_Effectstate = _eEffectSTATE.NULL;
		this.GetComponent<CircleCollider2D> ().enabled = true;
		_bodyTransform.GetComponent<SpriteRenderer> ().color = new Color (255, 255, 255);

		if(_hpgauge != null)
			_hpgauge.material.SetFloat("_Progress", 1);

		//transform.localScale = new Vector2 (1.5f, 1.5f);

		//for (int i = 0; i < _hpgrid.GetChildList ().Count; i++) {
		////	_hpgrid.GetChild (i).gameObject.SetActive (true);
		//}

		//_nhpconst = _nMaxHP / 5;

		//쇼크 스킬 있을 때
		if (this.GetComponent<cSkillShock> ()) {
			Destroy (this.GetComponent<cSkillShock> ());
		}
		//독 있을시
		else if (this.GetComponent<cSkillPoison> ()) {
			Destroy (this.GetComponent<cSkillPoison> ());
		} else if (this.GetComponent<cSkill8> ()) {
			Destroy (this.GetComponent<cSkill8> ());
		}

        if(_rigidbody != null)
        this._rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

		transform.localScale = new Vector2 (1.5f, 1.5f);
	}

	IEnumerator _IEffectHurt()
	{
		_bodyTransform.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, _bodyTransform.GetComponent<SpriteRenderer> ().color.a);
		yield return new WaitForSeconds (0.07f);
		_bodyTransform.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, _bodyTransform.GetComponent<SpriteRenderer> ().color.a);
	}

	public void _EffectHurt(Vector2 vector)
	{
		StartCoroutine (_IEffectHurt ());
		_EffectHurtObject (vector);
	}
	public void _EffectHurtStart()
	{
		StartCoroutine (_IEffectHurt ());
	}

	void _EffectHurtObject(Vector2 vector)
	{
		GameObject obj = null;
		switch (Random.Range (0, 3)) {
		case 0:
			obj = cObjectPool.SharedInstance.GetPooledObject ("MonsterDamager1");
			break;
		case 1:
			obj = cObjectPool.SharedInstance.GetPooledObject ("MonsterDamager2");
			break;
		case 2:
			obj = cObjectPool.SharedInstance.GetPooledObject ("MonsterDamager3");
			break;
		}

		if (obj != null) {
			obj.transform.position = vector;
			obj.SetActive(true);
		}
	}

	public void _SetState(eBarteriaSTATE state)
	{
		_state = state;
	}
	public void _ReflectWall(cWall wall)
	{
		switch (wall._ewall) {

		case eWall.L:
			reflectVector = Vector3.Reflect (_velocity, Vector2.right);

			break;

		case eWall.R:

			reflectVector = Vector3.Reflect (_velocity, Vector2.left);

			break;

		case eWall.D:

			reflectVector = Vector3.Reflect (_velocity, Vector2.down);

			break;

		default:
			reflectVector = _rigidbody.velocity;
			break;
		}



	}
	public void _ReflectAddPorce (float power)
	{

		_rigidbody.velocity = (Vector2)reflectVector.normalized * (power);

		/*
		if (_rigidbody.velocity.sqrMagnitude > 90 && _rigidbody.velocity.sqrMagnitude < 120) {


		} else if (_rigidbody.velocity.sqrMagnitude > 70 && _rigidbody.velocity.sqrMagnitude < 90) {

		} else if (_rigidbody.velocity.sqrMagnitude > 50 && _rigidbody.velocity.sqrMagnitude < 70) {

		} else if (_rigidbody.velocity.sqrMagnitude > 30 && _rigidbody.velocity.sqrMagnitude < 50) {

		}
		*/
		//Debug.Log (_rigidbody.velocity.sqrMagnitude +  "   "  + _sound.volume);

		//_sound.PlayOneShot (cMainGame._GetInst ()._ManagerSound._Bac_Coll);
	}
	public void _AddPorce(float power)
	{
		_rigidbody.velocity = _rigidbody.velocity.normalized * power;
	}
	public Rigidbody2D _GetRigidbody()
	{
		return _rigidbody;
	}
	public void _SetRigidbody(Rigidbody2D rigidbody)
	{
		_rigidbody = rigidbody;
	}

	protected void _NotMoveAwake()
	{
		if (_velocity == Vector3.zero) {

			_notmove += Time.deltaTime;

			if (_notmove >= 3f) {
				
				Vector2 vector = new Vector2 (Random.Range (-1f, 1f), Random.Range (1, 5f));
				_rigidbody.AddForce (vector * 2000f);

				_notmove = 0;
			}
		} else {
			_notmove = 0;
		}
	}

	public void _SkillActuation()
	{
		_skill = _eBarteriaSkill.Update;
	}
}

public enum eBarteriaNUMBER
{
	NULL,
	// 건들지ㄴ
	normal_1,
	normal_2,
	normal_3,
	normal_4,
	normal_5,

	hide_1	,
	hide_2	,
	hide_3	,
	hide_4	,

	zombie_1,
	zombie_2,
	zombie_3,
	zombie_4,

	speed_1	,
	speed_2	,
	speed_3	,

	medic_1	,
	medic_2	,
	medic_3	,

	rich	,
	life	,
	gift	,
	playeronkill, //23
	arbiter	,
	allkill	,	//25
	boom	,
	splits	,	//27
	dawdler	,
	throwbody,	//29
	//
	air_1	,	//30
	air_2	,
	air_3	,	//32

	split	,	//33
	Coin	,

	END		,
}