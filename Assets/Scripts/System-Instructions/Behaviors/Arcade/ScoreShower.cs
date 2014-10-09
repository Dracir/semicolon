using System.Reflection;
using UnityEngine;
using System.Collections;

public class ScoreShower : Observer {

	
	private TextCollider2D textCollider2d;
	
	private LevelScore score;
	
	void Start () {
		score = LevelScore.instance;
		score.observers.Add(this);
		textCollider2d = this.GetComponent<TextCollider2D>();
	}
	
	
	public override void notify(){
		textCollider2d.Text = score.Score + "";
	}
}
