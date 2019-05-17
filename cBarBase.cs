using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BARTYPE
{
	LEFT	,
	RIGHT 	,
};

public enum _eBarState
{
	IDLE	,
	UP		,
	DOWN	,
};

public class cBarBase : MonoBehaviour {

	public enum _eBarSkillState
	{
		NULL	,
		Idle	,
		Attack	,
		Delay	,
		Delaying,
	};

	public BARTYPE _type;
	public _eBarNUMBER _barnumber = _eBarNUMBER.Normal;

	public _eBarState _state;
	public _eBarSkillState _skillstate;

	public Transform _pivot;

	protected AudioSource _audio;
	protected Rigidbody2D _rigidbody;

	public bool _isTouch = false;

	//바 움직임
	public float _time = 0;
	public float _Speed = 5;
	public float _angleStart = -35;
	public float _angleEnd = 27;

	public AudioClip[] _clip = new AudioClip[2];

	public virtual void _Init()
	{
		_audio = this.GetComponent<AudioSource> ();
		_rigidbody = this.GetComponent<Rigidbody2D> ();
		//_rigidbody.MoveRotation (_angleStart);
	}

	public virtual void _Animation() {}


	void FixedUpdate()
	{

		if (_state == _eBarState.IDLE)
		{
			return;
		}

		_time += Time.deltaTime;

		if (_type == BARTYPE.LEFT) {

			if (_state == _eBarState.UP) {

				if (_rigidbody.rotation >= _angleEnd - 1) {
					_time = 0;
					_state = _eBarState.DOWN;
				} else {

					_rigidbody.MoveRotation (linear (_rigidbody.rotation, _angleEnd, _time * _Speed));

				}

			} else {

				if (_rigidbody.rotation <= _angleStart + 1) {

					_time = 0;
					_state = _eBarState.IDLE;
				} else {

					_rigidbody.MoveRotation (linear (_rigidbody.rotation, _angleStart, _time * _Speed));

				}
			}

		} else {

			if (_state == _eBarState.UP) {


				if (_rigidbody.rotation <= _angleEnd+1) {
					_time = 0;
					_state = _eBarState.DOWN;
				} else {

					_rigidbody.MoveRotation (linear (_rigidbody.rotation, _angleEnd, _time * _Speed));

				}

			} else {

				if (_rigidbody.rotation >= _angleStart-1) {
					_time = 0;
					_state = _eBarState.IDLE;
				} else {

					_rigidbody.MoveRotation (linear (_rigidbody.rotation, _angleStart, _time * _Speed));

				}
			}
		}

	}

	private float linear(float start, float end, float value){
		return Mathf.Lerp(start, end, value);
	}

	public AudioSource _GetAudio()
	{
		return _audio;
	}

	public void _TouchBar()
	{
		_audio.PlayOneShot (_clip [0]);
	}
}
