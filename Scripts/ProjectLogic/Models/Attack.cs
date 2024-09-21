using Godot;

namespace Boulder.Scripts.Models
{
    public partial class Attack
    {
        public double Damage { get; set; }
        public double KnockBackForce { get; set; }
        public Vector2 AttackPosition { get; set; }


        public Attack(
            double damage, 
            double knockBackForce, 
            Vector2 attackPosition
            )
        {
            Damage = damage;
            KnockBackForce = knockBackForce;
            AttackPosition = attackPosition;
        }
    }
}
