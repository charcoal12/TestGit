using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cDropletBar : cBarBase {


	int _count = 50;

	cDropletUI _ui;

	public override void _Init ()
	{
		base._Init ();

		_count = cRoot._GetInst ()._bar._GetUpgread ()._GetDamager();
	}

	public override void _Animation ()
	{
		base._Animation ();
	}
	// Use this for initialization
	void Start () {
		_Init ();


		string uipath = string.Format ("{0}", "Bar/DropUI");
		GameObject obj = Instantiate (Resources.Load (uipath), cGameUIManager._GetInst ().transform) as GameObject;

		_ui = obj.GetComponent<cDropletUI> ();

		if (_type == BARTYPE.LEFT) {
			obj.transform.localPosition = new Vector2 (-434, -600);
		}
		else if (_type == BARTYPE.RIGHT) {
			obj.transform.localPosition = new Vector2 (434, -600);

		}

		_ui._SetCount (_count);

	}


	void OnCollisionEnter2D(Collision2D other)
	{

		if (_count <= 0) {
			return;
		}
		if (_state == _eBarState.UP) {

			if (other.gameObject.layer == LayerMask.NameToLayer ("Ground_MonsterLayer")) {

				cBarteriaBase monster = other.gameObject.GetComponent<cBarteriaBase> ();


				if (monster.gameObject.transform.localScale.x >= 2.5f) {

					return;

				}
				if (monster.GetComponent<cSkill8> ()) {

					monster.gameObject.GetComponent<cSkill8> ()._SetMaximumSize (monster.transform.localScale.x + 0.1f);
				}

				else {

					monster.gameObject.AddComponent<cSkill8> ()._SetMaximumSize (monster.transform.localScale.x + 0.1f);
				}

				_count--;
				_ui._SetCount (_count);

				_audio.PlayOneShot (_clip [1]);
			}
		}
	}
}
