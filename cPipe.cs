using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cPipe : cCharacterObject {

	public float _time = 0;
	float _attacktime = 3;

	float _idletime = 5f;

	cBarteriaBase _ConenuMonster = null;

	PointEffector2D _Gravity;
	CircleCollider2D _collider;

	public AudioClip[] _clip = new AudioClip[3];
	//소화 시키는 량

	public override void _Init ()
	{
		base._Init ();
		_Gravity = this.GetComponent<PointEffector2D> ();
		_collider = this.GetComponent<CircleCollider2D> ();
	}

	void Start()
	{
		_Init ();
	}

	void Update()
	{
		_VirtualAnimation ();
	}
		
	public override void _Delete ()
	{
		base._Delete ();

		if (_ConenuMonster != null) {
			_ConenuMonster.gameObject.SetActive (true);
			_ConenuMonster = null;
		}
	}

	public override void _VirtualAnimation ()
	{
		base._VirtualAnimation ();

		switch (_state) {

		case _eMushRoomState.Idle:


			_time += Time.deltaTime;

			if (_time >= _idletime) {

				int random = Random.Range (0, 2);
				_animator.SetTrigger (string.Format ("{0}{1}", "_isidle", random.ToString ()));

				_time = 0;
			}

			break;
		case _eMushRoomState.Attack:

			_animator.SetTrigger ("_isattack");

			_Gravity.enabled = false;
			_collider.enabled = false;

			_ConenuMonster.gameObject.SetActive (false);

			//텁
			_sound.PlayOneShot(_clip[0]);

			_state = _eMushRoomState.Attacking;
			_time = 0;
			break;
		case _eMushRoomState.Attacking:

			_time += Time.deltaTime;

			if (_time > _attacktime) {


				//소화 시킴 hp에 따라
				if (_skillvalue >= _ConenuMonster.GetComponent<cBarteriaBase> ()._nHp) {
					_ConenuMonster.gameObject.SetActive (true);
					_ConenuMonster._HpDown (9999);
					//텁
					_sound.PlayOneShot(_clip[2]);

				} else {
					_sound.PlayOneShot(_clip[1]);

					if (_ConenuMonster.GetComponent<cSkillSlow> ()) {
						Destroy (_ConenuMonster.GetComponent<cSkillSlow> ());
					}

					_ConenuMonster.gameObject.SetActive (true);
					_ConenuMonster.GetComponent<cBarteriaBase> ()._EffectHurtStart ();
					_ConenuMonster.gameObject.AddComponent <cSkill2> ();
				}

				_time = 0;
				_state = _eMushRoomState.Delay;
			}

			break;
		case _eMushRoomState.Delay:

			_Gravity.enabled = false;
			_state = _eMushRoomState.Delayiing;

			break;
		case _eMushRoomState.Delayiing:

			_time += Time.deltaTime;

			if (_time > _delay) {

				_Gravity.enabled = true;
				_collider.enabled = true;

				_state = _eMushRoomState.Idle;
				_time = 0;
			}

			break;
		
		case _eMushRoomState.Seal:

			_Gravity.enabled = false;
			_collider.enabled = false;
			_time = 0;
			_state = _eMushRoomState.Sealing;

			break;

		case _eMushRoomState.Sealing:

			break;
		}

	}

	float _distance = 0f;

	void OnTriggerStay2D(Collider2D other)
	{

		if (_eMushRoomState.Attacking == _state)
			return;

		if (other.gameObject.layer == LayerMask.NameToLayer ("Ground_MonsterLayer")) {

			_distance = Vector3.Distance(transform.position + (Vector3)_collider.offset, other.transform.position);

			//쓰읍
			_sound.clip = _clip [0];
			_sound.Play ();


			if (_distance < 0.5f) {

				_ConenuMonster = other.GetComponent<cBarteriaBase> ();

				_ConenuMonster.transform.position = transform.position + (Vector3)_collider.offset;

				_state = _eMushRoomState.Attack;
			}

		}
	}
}
