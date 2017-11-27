namespace MyExample
{
    #region

    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Sharp.Core.Control;
    using Sharp.Core.Data.Dependency;
    using Sharp.Core.Data.Event.Server.Game;
    using Sharp.Core.Draw;
    using Sharp.Core.Entity;
    using Sharp.Core.Data.Entity.Components.Relations;
    using Sharp.Core.Entity.GameObject;
    using Sharp.Core.Event;
    using Sharp.Core.Data.Event.Draw;
    using Sharp.Core.Gui;
    using Sharp.Core.Server;
    using Sharp.Core.Vector;

    #endregion

    public class Program : IAssembly
    {
        private readonly IEvent Event;
        private readonly IControl Control;
        private readonly IEntity Entity;
        private readonly IGui Gui;
        private readonly IDraw Draw;
        private readonly INavigationMesh NavMesh;

        private IAiHeroClient Me;

        private Sharp.Core.Data.Spell.ISpell Q;
        private Sharp.Core.Data.Spell.ISpell W;
        private Sharp.Core.Data.Spell.ISpell E;
        private Sharp.Core.Data.Spell.ISpell R;

        public Program(IEvent _event, IControl _control, IEntity _entity, IGui _gui, IDraw _draw, INavigationMesh _navmesh)
        {
            Event = _event;
            Control = _control;
            Entity = _entity;
            Gui = _gui;
            Draw = _draw;
            NavMesh = _navmesh;
        }

        public Task OnInit()
        {
            Me = Entity.List.Player;

            Q = Me.SpellBook.Q;
            W = Me.SpellBook.W;
            E = Me.SpellBook.E;
            R = Me.SpellBook.R;
            
            InitMenu();
            InitEvent();

            return Task.CompletedTask;
        }

        private void InitMenu()
        {
            //Nothing
        }

        private void InitEvent()
        {
            // 0.01
            // Event.Server.Game.OnUpdate += OnUpdate;
            // Event.Draw.OnDraw += OnDraw;

            // 0.02
            Event.Server.Game.OnUpdateSync += OnUpdateSync;
            Event.Draw.OnDrawSync += OnDrawSync;
        }

        public async Task OnUpdateSync(UpdateEventArgs Args)
        {
            if (!Me.Health.IsAlive || IsRecall(Me) || IsGuiOpen())
            {
                return;
            }

            // 0.02

            try
            {
                // For KillSteal Logic
                KillSteal();

                // TODO 

                // If Combo Key Active
                var comboKeyActive = true;
                if (comboKeyActive)
                {
                    Combo();
                }

                var harassKeyActive = true;
                if (harassKeyActive)
                {
                    Harass();
                }

                var laneClearKeyActive = true;
                if (laneClearKeyActive)
                {
                    LaneClear();
                    JungleClear();
                }

                var lastHitKeyActive = true;
                if (lastHitKeyActive)
                {
                    LastHit();
                }
            }
            catch (InvalidOperationException ex)
            {
                // Nothing
            }

            await Task.CompletedTask;
        }

        private void Combo()
        {
            // https://github.com/SharpGG/Core.Examples/blob/master/Core.Examples/Insec.cs#L50
            var target = Gui.Information.Target;

            if (target != null) // and need some check
            {
                // post your logic in here
                // ex: cast Q

                var qRange = Q.Data.CastRange;

                if (IsReady(Q) && DistanceToPlayer(target) <= qRange)
                {
                    // Cast Q To target
                    CastOnUnit(Q, target);

                    // Cast Q To Position
                    Cast(Q, target.Transform.Position);

                    // Cast Q Self
                    Cast(Q);
                }
            }
        }

        private void Harass()
        {
            // Mana Check
            var manaLimit = 50;
            if (Me.Resource.Percent <= manaLimit)
            {
                return;
            }

            // https://github.com/SharpGG/Core.Examples/blob/master/Core.Examples/Insec.cs#L50
            var target = Gui.Information.Target;

            if (target != null) // and need some check
            {
                // post your logic in here
                // ex: cast Q

                var qRange = Q.Data.CastRange;

                if (IsReady(Q) && DistanceToPlayer(target) <= qRange)
                {
                    // Cast Q To target
                    CastOnUnit(Q, target);

                    // Cast Q To Position
                    Cast(Q, target.Transform.Position);

                    // Cast Q Self
                    Cast(Q);
                }
            }
        }

        private void LaneClear()
        {
            // Mana Check
            var manaLimit = 50;
            if (Me.Resource.Percent <= manaLimit)
            {
                return;
            }

            // ex Get Q Range Minion List
            var qRange = Q.Data.CastRange;
            var minions = Entity.List.Minions.Enemies.Where(x => DistanceToPlayer(x) <= qRange);

            if (minions.Any())
            {
                // do your logic
            }
        }

        private void JungleClear()
        {
            // Mana Check
            var manaLimit = 50;
            if (Me.Resource.Percent <= manaLimit)
            {
                return;
            }

            // ex Get Q Range Mob List
            var qRange = Q.Data.CastRange;
            var mobs = Entity.List.Minions.Neutral.Where(x => DistanceToPlayer(x) <= qRange);

            if (mobs.Any())
            {
                // do your logic
            }
        }

        private void LastHit()
        {
            // Mana Check
            var manaLimit = 50;
            if (Me.Resource.Percent <= manaLimit)
            {
                return;
            }

            // ex Get Q Range Minion List
            var qRange = Q.Data.CastRange;
            var minions = Entity.List.Minions.Enemies.Where(x => DistanceToPlayer(x) <= qRange);

            if (minions.Any())
            {
                // do your logic
            }
        }

        private void KillSteal()
        {
            // For Spell KillAble Target
            // ex

            var qDMG = 100; // just example
            var qRange = Q.Data.CastRange; // Get Q Range

            foreach (var target in Entity.List.Heroes.Enemies.Where(x => DistanceToPlayer(x) <= qRange && x.Health.Secondary.Current <= qDMG))
            {
                // Check Q Can be cast
                // TODO check target can killable
                if (IsReady(Q))
                {
                    // Cast Q To target
                    CastOnUnit(Q, target);

                    // Cast Q To Position
                    Cast(Q, target.Transform.Position);

                    // Cast Q Self
                    Cast(Q);
                }
            }
        }

        public async Task OnDrawSync(DrawEventArgs Args)
        {
            if (!Me.Health.IsAlive || IsGuiOpen())
            {
                return;
            }

            try
            {
                // draw skillshot range or others
                // ex: Q Range

                var qRange = Q.Data.CastRange;

                if (Q.IsReady)
                {
                    // https://github.com/SharpGG/Core.Examples/blob/master/Core.Examples/AutoAttackRangeDrawer.cs#L41
                    // Draw.Renderer.Render(new Circle(Me.Transform.Position, qRange));
                }
            }
            catch (InvalidOperationException ex)
            {
                // Nothing
            }

            await Task.CompletedTask;
        }

        public bool IsReady(Sharp.Core.Data.Spell.ISpell spell)
        {
            return spell.IsReady && (Me.Resource.Max == 0 || spell.ResourceCost == 0 ||
                                     Me.Resource.Current >= spell.ResourceCost);
        }

        // Cast On Self
        public void Cast(Sharp.Core.Data.Spell.ISpell spell)
        {
            Control.Spell.Cast(spell.Data.SpellSlot);
        }

        // Cast On Position
        public void Cast(Sharp.Core.Data.Spell.ISpell spell, Vector3 Position)
        {
            Control.Spell.Cast(spell.Data.SpellSlot, Position); // TODO Core fix
        }

        // Cast On Position
        public void Cast(Sharp.Core.Data.Spell.ISpell spell, Vector2 Position)
        {
            Control.Spell.Cast(spell.Data.SpellSlot, To3D(Position)); // TODO Core fix
        }

        // Cast On Target
        public void CastOnUnit(Sharp.Core.Data.Spell.ISpell spell, IIsSpellTargetable target)
        {
            Control.Spell.Cast(spell.Data.SpellSlot, target); // TODO Core fix
        }

        public bool IsRecall(IAiHeroClient player)
        {
            return player.Buffs.All.Values.Any(x =>
                x.Active && string.Equals(x.InternalName, "recall", StringComparison.OrdinalIgnoreCase));
        }

        public bool IsGuiOpen()
        {
            return Gui.Information.ChatOpen || Gui.Information.ScoreboardOpen || Gui.Information.ShopOpen ||
                   !Gui.Information.WindowOnTop;
        }

        public double DistanceToPlayer(IAttackableUnit target)
        {
            if (target == null || !target.Health.IsAlive)
            {
                return 20000;
            }

            return target.Transform.Position.Distance(Me.Transform.Position);
        }

        public Vector2 To2D(Vector3 Position)
        {
            return new Vector2(Position.X, Position.Y);
        }

        // TODO should be test it
        public Vector3 To3D(Vector2 Position)
        {
            return new Vector3(Position.X, Position.Y, NavMesh.GetHeightForPosition(Position));
        }
    }
}
