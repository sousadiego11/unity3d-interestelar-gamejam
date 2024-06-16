using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Hackeable : MonoBehaviour {
    [Header("[ Hackeable ]")]
    [SerializeField] protected Slider staminaSlider;
    [SerializeField] protected Canvas staminaCanvas;
    [SerializeField] protected bool isRegeneratingStamina;
    [SerializeField] protected bool isDisabled;
    
    protected void CheckStatus() {
        staminaCanvas.enabled = staminaSlider.value < staminaSlider.maxValue;
        isDisabled = staminaSlider.value <= 0f || isRegeneratingStamina;

        if (isDisabled && !isRegeneratingStamina) {
            StartCoroutine(RegenerateStamina());
        }
    }

    public void OnHit() {
        staminaSlider.value -= Time.deltaTime * 2f;
    }
    
    protected IEnumerator RegenerateStamina() {
        isRegeneratingStamina = true;
        while (staminaSlider.value < staminaSlider.maxValue) {
            staminaSlider.value += Time.deltaTime * 0.4f;
            yield return null;
        }
        isRegeneratingStamina = false;
    }
}