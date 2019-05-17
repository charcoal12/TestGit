using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cBarteriaArbiter : cBarteriaBase {

	public Transform _eyeTransform;
	public Transform _eyeballTransform;
	public Transform _shadowTransform;

	const float _const =  0.015f;
	const float _constball = 0.002f;

	protected Vector3 _eyevector = Vector2.zero;
	protected Vector3 _eyeballvector = Vector2.zero;

	Vector3 _Oldeye = Vector3.zero;
	Vector3 _OldeyeBall = Vector3.zero;

	public override void _Init ()
	{
		base._Init ();

		transform.localScale = new Vector3 (2, 2);
	}

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
			_rigidbody.simulated = false;
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
		_nHp = _nMaxHP;

	}

	void FixedUpdate()
	{
		_PhysicMove ();
	}
	// Update is called once per frame
	void Update () {
		//_Animation ();
		_virtualAnimation ();
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer ("Wall_Layer")) {
			_ReflectWall (other.gameObject.GetComponent<cWall> ());
			_ReflectAddPorce (_rigidbody.velocity.magnitude);
		}
		else if(other.gameObject.layer == LayerMask.NameToLayer ("Ground_PlayerLayer")) {
			_EffectHurt (other.contacts[0].point);

		}
	}
}
