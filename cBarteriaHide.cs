using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cBarteriaHide : cBarteriaBase {

	public float _hidetime = 0;
	protected float _hideValue = 100;
	public float _time = 0;

	protected float _subtime = 0;
	protected float _PercentValue = 0;
	public SpriteRenderer[] _renderer;

	public Transform _eyeTransform;
	public Transform _eyeballTransform;

	public Transform _shadowTransform;

	const float _const =  0.015f;
	const float _constball = 0.002f;

	protected Vector3 _eyevector = Vector2.zero;
	protected Vector3 _eyeballvector = Vector2.zero;

	protected Vector3 _Oldeye = Vector3.zero;
	protected Vector3 _OldeyeBall = Vector3.zero;

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

		_renderer = new SpriteRenderer[3];

		_renderer [0] = _eyeTransform.GetComponent<SpriteRenderer> ();
		_renderer [1] = _eyeballTransform.GetComponent<SpriteRenderer> ();
		_renderer [2] = _bodyTransform.GetComponent<SpriteRenderer> ();

		_nMaxHP = _nHp;

		_PercentValue = 1 - (_hideValue / 100);
	}

	void FixedUpdate()
	{
		_PhysicMove ();


		switch (_skill) {
		case _eBarteriaSkill.Awake:
			
			break;
		case _eBarteriaSkill.Start:

			break;
		case _eBarteriaSkill.Update:

			_time += Time.deltaTime * 0.5f;

			for (int i = 0; i < _renderer.Length; i++) {
				_renderer [i].color = new Color (1,1,1, Mathf.Lerp (1, _PercentValue, _time));
			}

			if (_renderer [2].color.a <= _PercentValue) {
				_time = 0;
				_subtime = 0;
				_skill = _eBarteriaSkill.Return;
			}

			break;

		case _eBarteriaSkill.Return:

			_time += Time.deltaTime;

			//다시 돌아오게
			if (_time >= _hidetime) {

				_subtime += Time.deltaTime * 0.5f;

				for (int i = 0; i < _renderer.Length; i++) {
					_renderer [i].color = new Color (1,1,1, Mathf.Lerp (_PercentValue, 1, _subtime));
				}

				if (_renderer [2].color.a >= 1) {
					_time = 0;
					_subtime = 0;
					_skill = _eBarteriaSkill.Start;
				}
			}

			break;
		}
	}
	// Update is called once per frame
	void Update () {
		_virtualAnimation();
	}

	void OnCollisionEnter2D(Collision2D other)
	{

		if (other.gameObject.layer == LayerMask.NameToLayer ("Wall_Layer")) {

			_ReflectWall (other.gameObject.GetComponent<cWall> ());
			_ReflectAddPorce (_rigidbody.velocity.magnitude);

		}

		else if (other.gameObject.layer == LayerMask.NameToLayer ("Ground_PlayerLayer")){
			
			if (_skill == _eBarteriaSkill.Start)
				_skill = _eBarteriaSkill.Update;

			_EffectHurt(other.contacts[0].point);
		}
	}
}
