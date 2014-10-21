using UnityEngine;
using System.Collections;

public class TimeShower : Observer {

	private TextCollider2D textCollider2d;
	
	private LevelTime time;
	private bool wentNegative = false;
	private Effect flashEffect;
	
	void Start () {
		time = LevelTime.instance;
		time.observers.Add(this);
		textCollider2d = this.GetComponent<TextCollider2D>();
	}
	
	
	public override void notify(){
		textCollider2d.Text = Mathf.Ceil(time.TimeLeft) + "";
		
		if(wentNegative){
			if(time.TimeLeft >= 10){
				wentNegative = false;
				if(!flashEffect.isDone){
					flashEffect.isDone = true;
				}
				textCollider2d.Color = GameConstantes.instance.currentTheme.instructionColor;
				textCollider2d.Color = Color.magenta;
			}
		}else{
			if(time.TimeLeft < 10){
				wentNegative = true;
				flashEffect = GameConstantes.instance.currentTheme.createTimeGoingUnder10Gradient(textCollider2d);
				EffectManager.AddGameEffect( flashEffect);
			}
		}
	}
}
