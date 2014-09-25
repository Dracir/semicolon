using UnityEngine;
using System.Collections;

public class PooledObject : Hydrogen.Core.ObjectPoolItemBase {

	protected bool initialized;
	
	public override bool IsInactive() {
		return ParentPool.HasRigidbody && gameObject.rigidbody.IsSleeping();
	}

	public virtual void Despawn(float delay = 0) {
		if (delay > 0)
			StartCoroutine("DespawnAfterDelay", delay);
		else
			hObjectPool.Instance.Despawn(gameObject, PoolID);
	}
	
	public override void DespawnSafely() {
		StartCoroutine(WaitForParticles());
	}

	public virtual IEnumerator Initialize() {
		initialized = true;
		yield return null;
	}
	
	public virtual IEnumerator DespawnAfterDelay(float delay) {
		yield return new WaitForSeconds(delay);
		hObjectPool.Instance.Despawn(gameObject, PoolID);
	}

	public virtual IEnumerator WaitForParticles() {
		if (particleEmitter != null) {
			yield return null;
			yield return new WaitForEndOfFrame();

			while (particleEmitter.particleCount > 0) {
				yield return null;
			}
			particleEmitter.emit = false;
		}
		else if (particleSystem != null) {
			yield return new WaitForSeconds(particleSystem.startDelay + 0.25f);
			while (particleSystem.IsAlive(true)) {
				if (!particleSystem.gameObject.activeSelf) {
					particleSystem.Clear(true);
					yield break;
				}
				yield return null;
			}
		}
		gameObject.SetActive(false);

		hObjectPool.Instance.ObjectPools[PoolID].DespawnImmediate(gameObject);
	}
	
	public override void OnSpawned() {
		gameObject.SetActive(true);

		initialized = false;
		StartCoroutine("Initialize");
	}

	public override void OnDespawned() {
		if (ParentPool.HasRigidbody) {
			gameObject.rigidbody.velocity = Vector3.zero;
		}
		else if (ParentPool.HasRigidbody2D){
			gameObject.rigidbody2D.velocity = Vector2.zero;
		}
		
		gameObject.SetActive(false);
		StopCoroutine("Initialize");
		StopCoroutine("DespawnAfterDelay");
	}

}
