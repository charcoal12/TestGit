using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cSpiky : cCharacterObject {

	CircleCollider2D _collider;

	public float _time = 0;

	public int _idletime = 5;

	public override void _Init ()
	{
		base._Init ();
		_collider = this.GetComponent<CircleCollider2D> ();
	}

	public override void _VirtualAnimation ()
	{
		base._VirtualAnimation ();

		switch (_state) {

		case _eMushRoomState.Idle:

			_time += Time.deltaTime;

			if (_time >= _idletime) {

				_animator.SetTrigger (string.Format ("{0}{1}", "_isidle", Random.Range (0, 2).ToString ()));
				_time = 0;

			}
				
			break;
		case _eMushRoomState.Attack:

			_animator.SetTrigger ("_isattack");

			_sound.PlayOneShot (_sound.clip);

			_state = _eMushRoomState.Delayiing;
			break;
		case _eMushRoomState.Delayiing:

			_time += Time.deltaTime;

			if (_time > 3f) {
				_state = _eMushRoomState.Idle;
				_time = 0;
			}

			break;
		
		case _eMushRoomState.Seal:

			_time = 0;
			_state = _eMushRoomState.Sealing;

			break;

		case _eMushRoomState.Sealing:

			break;
		}

	}

	void Start()
	{
		_Init ();
	}

	void Update()
	{
		_VirtualAnimation();
	}

	// 공 격 중 스 킬 전 체 스 킬
	void OnTriggerStay2D(Collider2D other)
	{

		if (_eMushRoomState.Idle == _state) {

			if (other.gameObject.layer == LayerMask.NameToLayer ("Ground_MonsterLayer")) {


				Collider2D[] others = Physics2D.OverlapCircleAll (transform.position, 1.5f, 1 << 9);

				for (int i = 0; i < others.Length; i++) {


					cBarteriaBase monster = others [i].gameObject.GetComponent<cBarteriaBase> ();
					monster._HpDown (_skillvalue);
					monster._EffectHurt (monster.transform.position);

				}
				_state = _eMushRoomState.Attack;
			}
		}
	}
}
