using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : MonoBehaviour{

//-------------------------------------------------------------------------//
    // Valores SerializeField mantêm a variável privada e podem ser alterados durante o teste no Unity //
    [SerializeField] private float baseSpeed = 120f; // Velocidade base do Player //
    [SerializeField] private float rollDistance = 0.038f; // Distância do rolamento //
    [SerializeField] private float rollDuration = 0.2f; // Duração do rolamento //
    [SerializeField] private int rollCost = 30; // Custo de estamina ao rolar //
    [SerializeField] private float staminaMaxValue = 100f; // Estamina máxima //
    [SerializeField] private float staminaRegenRate = 16f; // Taxa de regen da estamina //
    [SerializeField] private int healthMaxValue = 100; // Vida máxima //
    [SerializeField] private int healthCurrentValue; // Vida atual //
    [SerializeField] private float healRate = 0.05f; // Taxa de regen da vida //
    [SerializeField] private float regenDelay = 2f; // Delay da taxa de regen da vida //
//-------------------------------------------------------------------------//
    private Vector2 moveInput; // Armazena o direcional //
    private Vector2 lastMoveInput; // Armazenda o último botão de Input //
    private float staminaCurrentValue; // Valor atual da estamina //
    public Rigidbody2D rigidBody;  // Referência ao Rigidbody2D do Player //
    private float regenTimer; // Intervalo de regen  da vida //
    private bool isRolling; // Variável caso o jogador esteja rolando //
//-------------------------------------------------------------------------//
    private void Start(){ // Função de início do jogo //
        rigidBody = GetComponent<Rigidbody2D>(); // Colisão do jogador //
        staminaCurrentValue = staminaMaxValue; // Estamina máxima ao iniciar //
        healthCurrentValue = healthMaxValue; // Vida máxima ao iniciar //
    }
//-------------------------------------------------------------------------//
    // Método iniciado por frame
    private void Update(){  // Função chamada a cada frame do jogo //
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized; // Input de movimento //
        moveInput = moveInput * baseSpeed * Time.deltaTime; // Input multiplicado pela velocidade base //
        rigidBody.velocity = moveInput;  // Move o jogador //

        if (Input.GetKeyDown(KeyCode.Space) && !isRolling && staminaCurrentValue >= 30){ // Condição para rolar //
            StartRoll(moveInput); // Inicia rolamento a partir do Input // 
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)){ // Teste para receber dano //
            DamageHP(30);
        }

        if (IsRolling){  // Caso o jogador esteja rolando... //
            Invoke("RegenerateSP", 5); // Aguarda 5 ticks e inicia a função RegenerateSP //
        }
            else{ 
                RegenerateSP();
            }
        
        if (healthCurrentValue < healthMaxValue){ // Caso a vida atual seja menor que a vida máxima... //
            regenTimer -= Time.deltaTime; // Diminui o valor  do intervalo de regeneração (por frame) //

            if (regenTimer <= 0){  // Quando o intervalo for 0 ou menos... //

                RegenerateHP(); // Inicia a função regeneração //
                regenTimer = regenDelay; // Zera o intervalo //
            }
        }

        Debug.Log(StaminaCurrentValue); // Teste para verificar valor da estamina //
    }
//-------------------------------------------------------------------------//
    private void FixedUpdate(){ // Função chamada imediatamente após o frame //  
        if (isRolling){
            rigidBody.MovePosition(rigidBody.position + lastMoveInput * rollDistance); // Move o jogador ao rolar com base na direção e distância
        }
    }
//-------------------------------------------------------------------------//
    private void ReduceStamina(int cost){ // Função reduzir estamina //
        staminaCurrentValue = Mathf.Clamp(staminaCurrentValue - cost, 0f, staminaMaxValue);
    }
//-------------------------------------------------------------------------//
    private void RegenerateSP(){ // Função regenerar estamina //
        if(rigidBody.velocity.magnitude == 0f){// Se jogador estiver parado... //
             staminaCurrentValue += staminaRegenRate * Time.deltaTime; // Regeneração normal //
        }
            else { // Caso o jogador não esteja parado... //
                staminaCurrentValue += (staminaRegenRate/2) * Time.deltaTime; // Metade da regeneração //
            }
        staminaCurrentValue = Mathf.Clamp(staminaCurrentValue, 0f, staminaMaxValue); // Limita a variável "currentStamina" de 0 ao valor "maxStamina" //
    }
//-------------------------------------------------------------------------//
    private void StartRoll(Vector2 rollInput){ // Função de iniciar rolamento //
        ReduceStamina(rollCost);

        if (rollInput.magnitude == 0){ // Se o jogador tiver parado, utilize o último botão de direção //
            rollInput = lastMoveInput;
        }
        
        if (rollInput == Vector2.zero){ // Se o jogador não tiver se movimentado na cena, role para direita
            rollInput = Vector2.right;
        }

        lastMoveInput = rollInput; // Direção do rolamento //
        isRolling = true;
        Invoke("StopRoll", rollDuration); // Para o rolamento após a duração do rolamento //
    }
//-------------------------------------------------------------------------//
    private void StopRoll(){ // Fução desligar o rolamento //
        isRolling = false;
    }
//-------------------------------------------------------------------------//
    private bool IsRolling{ // Função verificar rolamento //
        get{
            return isRolling;
        }
    }
//-------------------------------------------------------------------------//
    private void DamageHP(int damage){ // Função levar dano //
        healthCurrentValue = Mathf.Clamp(healthCurrentValue - damage, 0, healthMaxValue);
        regenTimer = regenDelay;
    }
//-------------------------------------------------------------------------//
    private void RegenerateHP(){ // Função regenerar vida //
        healthCurrentValue = Math.Clamp(healthCurrentValue + 1, 0, healthMaxValue);
    }
//-------------------------------------------------------------------------//
    public float StaminaMaxValue{ // Função get de valor máximo da estamina //
        get{
            return staminaMaxValue;
        }
    }
//-------------------------------------------------------------------------//
    public float StaminaCurrentValue{ // Função get de valor atual da estamina //
        get{
            return staminaCurrentValue;
        }
    }
//-------------------------------------------------------------------------//
    public float HealthMaxValue{ // Função get do valor máximo da vida //
        get{
            return healthMaxValue;
        }
    }
//-------------------------------------------------------------------------//
    public float HealthCurrentValue{ // Função get do valor atual da vida //
        get{
            return healthCurrentValue;
        }
    }
//-------------------------------------------------------------------------//
}