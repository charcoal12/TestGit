using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cBarteriaMedic : cBarteriaBase {

	public _eEffectSTATE _subState;

	public float _Skilltime = 3;
	public float _PercentValue = 30;
	public float _Skillvalue;
	float _time = 0;

	public Transform _eyeTransform;
	public Transform _eyeballTransform;
	public Transform _shadowTransform;

	const float _const = 0.01f;
	const float _constY = 0.075f;
	const float _constball = 0.008f;

	protected Vector3 _eyevector = Vector2.zero;
	protected Vector3 _eyeballvector = Vector2.zero;

	Vector3 _Oldeye = Vector3.zero;
	Vector3 _OldeyeBall = Vector3.zero;

	public override void _PhysicMove ()
	{
		base._PhysicMove ();
		_velocity = _rigidbody.velocity;

		//
		_NotMoveAwake ();

		_eyevector = (Vector3)_rigidbody.velocity * _const;
		_eyeballvector = (Vector3)_rigidbody.velocity * _constball;

		if (_rigidbody.velocity.magnitude >= 0 && _rigidbody.velocity.magnitude <= 10) {

			_bodyTransform.localScale = new Vector3(1, (_scale.y + _rigidbody.velocity.y * 0.01f), 1);
			_shadowTransform.localScale = new Vector3(1, (_scale.y + _rigidbody.velocity.y * 0.01f), 1);

		} else {

			_bodyTransform.localScale = _settingScale;
			_shadowTransform.localScale = _settingScale;
		}


		_eyeTransform.localPosition = _eyevector + _Oldeye;
		_eyeballTransform.localPosition = _eyeballvector + _OldeyeBall;
	}

	public override void _virtualAnimation ()
	{
		base._virtualAnimation ();

		switch (_state) {

		case eBarteriaSTATE.NULL:

			if (_shadowTransform.gameObject.activeInHierarchy) {
				_shadowTransform.gameObject.SetActive (false);
			}

			break;

		case eBarteriaSTATE.Start:

			_shadowTransform.gameObject.SetActive (true);

			_rigidbody.simulated = true;
			_SetState (eBarteriaSTATE.Live);
			_skill = _eBarteriaSkill.Start;
			break;

		case eBarteriaSTATE.Live:
			break;
		case eBarteriaSTATE.Die:
			break;
		}

	}
	// Use this for initialization
	void Start () {
		_rigidbody = this.GetComponent<Rigidbody2D> ();
		_sound = this.GetComponent<AudioSource> ();

		_Oldeye = _eyeTransform.localPosition;
		_OldeyeBall = _eyeballTransform.localPosition;

		_nMaxHP = _nHp;

	}

	void FixedUpdate()
	{
		_PhysicMove ();

		switch (_skill) {
		case _eBarteriaSkill.Awake:
			
			//_skill = _eBarteriaSkill.Update;
			break;
		case _eBarteriaSkill.Start:

			_time = 0;
			_skill = _eBarteriaSkill.Update;

			break;
		case _eBarteriaSkill.Update:

			_time += Time.deltaTime;

			if (_time > _Skilltime) {

				_time = 0;
				_skill = _eBarteriaSkill.Return;
			}

			break;

		case _eBarteriaSkill.Return:

			break;
		}

	}
	// Update is called once per frame
	void Update () {
		//_Animation ();
		_virtualAnimation();
	}
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer ("Wall_Layer")) {
			_ReflectWall (other.gameObject.GetComponent<cWall> ());
			_ReflectAddPorce (_rigidbody.velocity.magnitude);
		} else if (other.gameObject.layer == LayerMask.NameToLayer ("Ground_PlayerLayer")) {
			_EffectHurt (other.contacts[0].point);

		} else if (other.gameObject.layer == LayerMask.NameToLayer ("Ground_MonsterLayer")) {


			if (_skill == _eBarteriaSkill.Return) {

				cBarteriaBase barteria = other.collider.GetComponent<cBarteriaBase> ();

				//barteria._HpUp (barteria._nMaxHP * _PercentValue / 100);
//				Debug.Log(barteria.name);
				barteria._HpUp (_Skillvalue);

				GameObject obj = cSkillHealingManager._GetInst ()._GetSkill11Effect ();
				if (obj != null) {
					
					obj.transform.parent = barteria.transform;
					obj.transform.localPosition = Vector2.zero;
					obj.transform.localScale = Vector2.one;
					obj.SetActive (true);
				}

				_skill = _eBarteriaSkill.Start;

			}
		}
	}
}
