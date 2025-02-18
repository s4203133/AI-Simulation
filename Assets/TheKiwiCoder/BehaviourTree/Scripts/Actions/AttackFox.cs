using TheKiwiCoder;
using UnityEngine;

public class AttackFox : ActionNode
{
    [Space(15)]
    public float damage;
    public float animationDelay;
    private float startTime;

    public GameObject attackParticles;

    bool dodging;
    bool attacking;
    bool finished;

    float attackTimer;

    protected override void OnStart() {
        context.aiAgent.stats.currentAction = actionName;
        context.agent.isStopped = true;
        startTime = Time.time;
        attacking = false;
        dodging = true;
        finished = false;
        attackTimer = 0.65f;
        blackboard.nodeStack.PushNode(this);
    }

    protected override void OnStop() {
        context.aiAgent.combat.canAttack = false;
        context.agent.isStopped = false;
        blackboard.nodeStack.PopNode(this);
    }

    protected override State OnUpdate() {
        if(context.aiAgent.combat.currentAttacker == null) {
            return State.Failure;
        }
        if (dodging) {
            if (Time.time - startTime >= animationDelay) {
                // Play Dodge Animation
                context.animator.SetTrigger("Dodge");
                dodging = false;
                if (!attacking) {
                    return State.Failure;
                }
            }
            return State.Running;
        }

        if (attacking && !dodging) {
            if (Time.time - startTime >= (animationDelay + 0.33f)) {
                // Play Attack Animation
                context.animator.SetTrigger("Attack");
                SoundManager.PlaySound(SoundManager.instance.rabbitAttack, context.gameObject);
                context.transform.LookAt(context.aiAgent.combat.currentAttacker.transform.position);
                attacking = false;
                finished = true;
                SpawnAttackParticles();
            }
            return State.Running;
        }
        if (finished) {
            SpawnAttackParticles();
            if (Time.time - startTime >= (animationDelay + 1.33f)) {
                // Finnish the action now that both animations have finished
                context.aiAgent.combat.AttackAgent(context.aiAgent.combat.currentAttacker, damage);
                context.aiAgent.combat.kills++;
                context.aiAgent.combat.canAttack = false;
                return State.Success;
            }
            return State.Running;
        }

        if (context.aiAgent.combat.AttackChance()) {
            attacking = true;
            return State.Running;
        }
        context.aiAgent.combat.canAttack = false;
        return State.Failure;
    }

    private void SpawnAttackParticles() {
        attackTimer -= Time.deltaTime;
        if(attackTimer <= 0) {
            Instantiate(attackParticles, context.aiAgent.combat.currentAttacker.transform.position, Quaternion.identity);
            SoundManager.PlaySound(SoundManager.instance.foxKilled);
            attackTimer = 0.65f;
        }
    }
}
