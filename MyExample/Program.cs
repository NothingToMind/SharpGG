namespace MyExample
{
    #region

    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Sharp.Core.Control;
    using Sharp.Core.Data.Control.Spell;
    using Sharp.Core.Data.Dependency;
    using Sharp.Core.Data.Event.Server.Game;
    using Sharp.Core.Draw;
    using Sharp.Core.Entity;
    using Sharp.Core.Entity.GameObject;
    using Sharp.Core.Enum.Spell;
    using Sharp.Core.Event;
    using Sharp.Core.Gui;
    using Sharp.Core.Vector;

    #endregion

    public class Program : IAssembly
    {
        private readonly IEvent Event;
        private readonly IControl Control;
        private readonly IEntity Entity;
        private readonly IGui Gui;
        private readonly IDraw Draw;

        private IAiHeroClient Me;

        private Sharp.Core.Data.Spell.ISpell Q;
        private Sharp.Core.Data.Spell.ISpell W;
        private Sharp.Core.Data.Spell.ISpell E;
        private Sharp.Core.Data.Spell.ISpell R;

        public Program(IEvent _event, IControl _control, IEntity _entity, IGui _gui, IDraw _draw)
        {
            Event = _event;
            Control = _control;
            Entity = _entity;
            Gui = _gui;
            Draw = _draw;
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
            Event.Server.Game.OnUpdateSync += OnUpdateSync;
        }

        public async Task OnUpdateSync(UpdateEventArgs args)
        {
            if (!Me.Health.IsAlive || IsRecall(Me) || IsGuiOpen())
            {
                return;
            }

            //TODO
            try
            {
                //swam logic
            }
            catch (InvalidOperationException ex)
            {
                // Nothing
            }

            await Task.CompletedTask;
        }

        // Cast On Self
        public void Cast(Sharp.Core.Data.Spell.ISpell spell)
        {
            Control.Spell.Cast(spell.Data.SpellSlot, new SelfCastSpellBase()); //TODO fix this
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
    }
}
