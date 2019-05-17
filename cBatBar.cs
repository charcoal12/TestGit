using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cBatBar : cBarBase {

	public GameObject[] _effect = new GameObject[10];

	int _randomvalue = 1;

	public override void _Init ()
	{
		base._Init ();

		_randomvalue = cRoot._GetInst ()._bar._GetUpgread ()._GetDamager ();
	}

	public override void _Animation ()
	{
		base._Animation ();
	}
	// Use this for initialization
	void Start () {
		_Init ();
	}


	void OnCollisionEnter2D(Collision2D other)
	{
		if (_state == _eBarState.UP) {

			if (other.gameObject.layer == LayerMask.NameToLayer ("Ground_MonsterLayer")) {

				//길이가 2보다 클 때 범위 인정
				if (Vector2.Distance (other.transform.position, _pivot.position) >= 2f) {

					int random = Random.Range (0, 101);

					//확률 조건
					if (random <= _randomvalue) {
						//홈런 되어야 한다
						other.gameObject.AddComponent (typeof(cBatSkill));

						_audio.PlayOneShot (_clip [1]);

					}
				}
			}
		}
	}
}
