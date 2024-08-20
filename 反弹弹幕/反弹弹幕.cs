using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;  // 添加此引用来使用Vector2

namespace 反弹弹幕
{
    // 主Mod类，管理模组的加载和卸载
    public class 反弹弹幕 : Mod
    {
        // 可以在这里初始化你的模组
    }

    // 全局玩家类，用于处理近战武器反弹弹幕的逻辑
    public class MyPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            // 检查玩家当前是否正在使用近战武器
            if (Player.HeldItem.damage > 0 && Player.HeldItem.DamageType == DamageClass.Melee)
            {
                // 检查玩家是否正在挥砍
                if (Player.itemAnimation > 0)
                {
                    // 玩家面向的方向
                    Vector2 playerFacingDirection = Player.direction == 1 ? Vector2.UnitX : Vector2.UnitX * -1;
                    
                    // 定义一个距离阈值
                    float distanceThreshold = 40f;  // 可以调整此值以适应你的需求

                    // 遍历所有弹幕
                    foreach (var projectile in Main.projectile)
                    {
                        // 检查弹幕是否是敌方的，并且是否在玩家面向的方向
                        if (projectile.active && !projectile.friendly)
                        {
                            // 计算弹幕的中心位置与玩家的相对位置
                            Vector2 playerToProjectile = projectile.Center - Player.Center;
                            float distanceToPlayer = playerToProjectile.Length();

                            // 判断弹幕是否在玩家面向的方向，并且在距离阈值内
                            if (Vector2.Dot(playerFacingDirection, playerToProjectile) > 0 && distanceToPlayer < distanceThreshold)
                            {
                                // 反弹弹幕并加速
                                ReflectProjectile(projectile);
                            }
                        }
                    }
                }
            }
        }

        private void ReflectProjectile(Projectile projectile)
        {
            // 改变弹幕的速度方向并加速至1.5倍
            projectile.velocity = -projectile.velocity * 1.5f;

            // 标记为玩家的弹幕
            projectile.owner = Player.whoAmI;
            projectile.friendly = true;
            projectile.hostile = false;

            // 防止弹幕再次反弹
            projectile.penetrate = 1;
        }
    }

    // 全局弹幕类，用于处理反弹弹幕的攻击行为
    public class MyGlobalProjectile : GlobalProjectile
    {
        public override bool? CanHitNPC(Projectile projectile, NPC target)
        {
            // 如果弹幕是反弹的，允许它击中敌人
            if (projectile.friendly && !projectile.hostile)
            {
                return true;
            }
            return base.CanHitNPC(projectile, target);
        }
    }
}
