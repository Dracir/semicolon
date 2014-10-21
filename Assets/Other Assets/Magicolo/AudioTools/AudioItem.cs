using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;
using Magicolo.GeneralTools;
using Candlelight;

[System.Serializable]
public abstract class AudioItem : Magicolo.GeneralTools.INamable {

	public string Name { get; set; }
	
	protected int Id { get; set; }
	
	protected float Volume { get; set; }
	
	protected float Pitch { get; set; }
	
	protected AudioStates State { get; set; }

	protected AudioItemManager itemManager;
	protected Magicolo.AudioTools.Player player;
	
	protected AudioItem(string name, int id, float volume, float pitch, AudioStates state, AudioItemManager itemManager, Magicolo.AudioTools.Player player) {
		this.Name = name;
		this.Id = id;
		this.Volume = volume;
		this.Pitch = pitch;
		this.State = state;
		this.itemManager = itemManager;
		this.player = player;
	}
	
	protected AudioItem(string name, int id, AudioItemManager itemManager, Magicolo.AudioTools.Player player) {
		this.Name = name;
		this.Id = id;
		this.Volume = 1;
		this.Pitch = 1;
		this.State = AudioStates.Waiting;
		this.itemManager = itemManager;
		this.player = player;
	}
	
	public abstract void Update();
		
	protected abstract void UpdateVolume();
		
	protected abstract void UpdatePitch();
		
	/// <summary>
	/// Resumes the AudioItem if it has been paused after synced <paramref name="delay"/> in units corresponding to the <paramref name="syncMode"/>.
	/// </summary>
	public abstract void Play(float delay, SyncMode syncMode);
	
	/// <summary>
	/// Resumes the AudioItem if it has been paused after <paramref name="delay"/> in seconds.
	/// </summary>
	public abstract void Play(float delay);
	
	/// <summary>
	/// Resumes the AudioItem if it has been paused.
	/// </summary>
	public virtual void Play() {
		State = AudioStates.Playing;
	}
		
	/// <summary>
	/// Pauses the AudioItem after synced <paramref name="delay"/> in units corresponding to the <paramref name="syncMode"/>.
	/// </summary>
	public abstract void Pause(float delay, SyncMode syncMode);
		
	/// <summary>
	/// Pauses the AudioItem after <paramref name="delay"/> in seconds.
	/// </summary>
	public abstract void Pause(float delay);
	
	/// <summary>
	/// Pauses the AudioItem.
	/// </summary>
	public virtual void Pause() {
		State = AudioStates.Paused;
	}
		
	/// <summary>
	/// Stops the AudioItem with fade out after synced <paramref name="delay"/> in units corresponding to the <paramref name="syncMode"/>. After being stopped, an AudioItem is obsolete.
	/// </summary>
	public abstract void Stop(float delay, SyncMode syncMode);
	
	/// <summary>
	/// Stops the AudioItem with fade out after <paramref name="delay"/> in seconds. After being stopped, an AudioItem is obsolete.
	/// </summary>
	public abstract void Stop(float delay);

	/// <summary>
	/// Stops the AudioItem with fade out. After being stopped, an AudioItem is obsolete.
	/// </summary>
	public virtual void Stop() {
		State = AudioStates.Stopped;
	}

	/// <summary>
	/// Stops the AudioItem immediatly without fade out. After being stopped, an AudioItem is obsolete.
	/// </summary>
	public virtual void StopImmediate() {
		State = AudioStates.Stopped;
	}

	/// <summary>
	/// Gets the unique ID of the AudioItem.
	/// </summary>
	/// <returns>The ID.</returns>
	public virtual int GetID() {
		return Id;
	}
	
	/// <summary>
	/// Gets the current state of the AudioItem.
	/// </summary>
	/// <returns>The state.</returns>
	public virtual AudioStates GetState() {
		return State;
	}
	
	/// <summary>
	/// Gets the volume of the AudioItem.
	/// </summary>
	/// <returns>The volume.</returns>
	public virtual float GetVolume() {
		return Volume;
	}

	/// <summary>
	/// Ramps the volume of the AudioItem.
	/// </summary>
	/// <param name="targetVolume">The target to which the volume will be ramped.</param>
	/// <param name="time">The time it will take for the volume to reach the <paramref name="targetVolume"/>.</param>
	public abstract void SetVolume(float targetVolume, float time);
	
	/// <summary>
	/// Sets the volume of the AudioItem.
	/// </summary>
	/// <param name="targetVolume">The target to which the volume will be set.</param>
	public abstract void SetVolume(float targetVolume);

	/// <summary>
	/// Gets the pitch of the AudioItem.
	/// </summary>
	/// <returns>The pitch.</returns>
	public virtual float GetPitch() {
		return Pitch;
	}
	
	/// <summary>
	/// Ramps the pitch of the AudioItem proportionally.
	/// </summary>
	/// <param name="targetPitch">The target to which the pitch will be ramped.</param>
	/// <param name="time">The time it will take for the pitch to reach the <paramref name="targetPitch"/>.</param>
	/// <param name="quantizeStep">The size of each pitch grid step in semi-tones.</param>
	/// <remarks>Note that using negative pitches will not work as expected.</remarks>
	public abstract void SetPitch(float targetPitch, float time, float quantizeStep);
	
	/// <summary>
	/// Ramps the pitch of the AudioItem proportionally.
	/// </summary>
	/// <param name="targetPitch">The target to which the pitch will be ramped.</param>
	/// <param name="time">The time it will take for the pitch to reach the <paramref name="targetPitch"/>.</param>
	/// <remarks>Note that using negative pitches will not work as expected.</remarks>
	public abstract void SetPitch(float targetPitch, float time);
	
	/// <summary>
	/// Sets the pitch of the AudioItem.
	/// </summary>
	/// <param name="targetPitch">The target to which the pitch will be set.</param>
	public abstract void SetPitch(float targetPitch);

	#region IEnumerators
	protected virtual IEnumerator PlayAfterDelay(float delay) {
		float counter = 0;
		while (counter < delay) {
			counter += Time.deltaTime;
			yield return new WaitForSeconds(0);
		}
		Play();
	}
		
	protected virtual IEnumerator PauseAfterDelay(float delay) {
		float counter = 0;
		while (counter < delay) {
			counter += Time.deltaTime;
			yield return new WaitForSeconds(0);
		}
		Pause();
	}
	
	protected virtual IEnumerator StopAfterDelay(float delay) {
		float counter = 0;
		while (counter < delay) {
			counter += Time.deltaTime;
			yield return new WaitForSeconds(0);
		}
		Stop();
	}
		
	protected virtual IEnumerator RampVolume(float startVolume, float targetVolume, float time) {
		float counter = 0;
			
		while (counter < time) {
			Volume = ((counter / time) * (targetVolume - startVolume) + startVolume);
			UpdateVolume();
			counter += Time.deltaTime;
			yield return new WaitForSeconds(0);
		}
			
		Volume = targetVolume;
		UpdateVolume();
	}
		
	protected virtual IEnumerator RampPitch(float startPitch, float targetPitch, float time, float quantizeStep) {
		float counter = 0;
		float currentStep = 0;
		float currentRatio = 1;
		float direction = ((targetPitch - startPitch) / Mathf.Abs(targetPitch - startPitch)).Round();
		
		while (counter < time) {
			float rampPitch = startPitch * Mathf.Pow(targetPitch / startPitch, counter / time);
			
			if (quantizeStep <= 0) {
				Pitch = rampPitch;
			}
			else {
				float roundedPitchTarget = startPitch * currentRatio;
				if ((direction < 0 && rampPitch <= roundedPitchTarget) || (direction > 0 && rampPitch >= roundedPitchTarget)) {
					Pitch = roundedPitchTarget;
					currentStep += quantizeStep * direction;
					currentRatio = Mathf.Pow(2, currentStep / 12);
				}
			}
				
			UpdatePitch();
			counter += Time.deltaTime;
			yield return new WaitForSeconds(0);
		}
			
		Pitch = targetPitch;
		UpdatePitch();
	}
	#endregion
}
