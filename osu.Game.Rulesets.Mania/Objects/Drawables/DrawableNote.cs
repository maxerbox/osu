// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using OpenTK.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Mania.Judgements;
using osu.Game.Rulesets.Mania.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Mania.Communication;
using Newtonsoft.Json;
using System.Linq;

namespace osu.Game.Rulesets.Mania.Objects.Drawables
{
    /// <summary>
    /// Visualises a <see cref="Note"/> hit object.
    /// </summary>
    public class DrawableNote : DrawableManiaHitObject<Note>, IKeyBindingHandler<ManiaAction>
    {
        protected readonly GlowPiece GlowPiece;

        private readonly LaneGlowPiece laneGlowPiece;
        private readonly NotePiece headPiece;
        private SocketCommunication socket;
        private double lastSendTime = 0;

        public DrawableNote(Note hitObject, ManiaAction action, SocketCommunication socket)
            : base(hitObject, action)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            this.socket = socket;
            Children = new Drawable[]
            {
                laneGlowPiece = new LaneGlowPiece
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                GlowPiece = new GlowPiece(),
                headPiece = new NotePiece
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre
                }
            };
        }

        public override Color4 AccentColour
        {
            get { return base.AccentColour; }
            set
            {
                if (base.AccentColour == value)
                    return;
                base.AccentColour = value;

                laneGlowPiece.AccentColour = value;
                GlowPiece.AccentColour = value;
                headPiece.AccentColour = value;
            }
        }

        protected override void CheckForJudgements(bool userTriggered, double timeOffset)
        {
            var maniaHit = (ManiaHitObject)HitObject;
            var column = socket.Columns[maniaHit.Column];
            if (maniaHit == column.HitObjects.First())
                // if (maniaHit == socket.Columns[maniaHit.Column]timeOffset > -150 && timeOffset < 100)
            {
                if (socket != null && (Time.Current - lastSendTime) > 5)
                {
                    var Event = EventBuilder.createNoteUpdateEvent(maniaHit.Column, 10, timeOffset);
                    socket.send(Event.ToString());
                    lastSendTime = Time.Current;
                }
            }
            if (!userTriggered)
            {
                if (timeOffset > HitObject.HitWindows.Bad / 2)
                {
                    AddJudgement(new ManiaJudgement { Result = HitResult.Miss });
                    if (maniaHit == column.HitObjects.First()) removeFromColumn(column, maniaHit);
                }
                return;
            }

            double offset = Math.Abs(timeOffset);

            if (offset > HitObject.HitWindows.Miss / 2)
            {
                if (maniaHit == column.HitObjects.First()) removeFromColumn(column, maniaHit);
                return;
            }

            AddJudgement(new ManiaJudgement { Result = HitObject.HitWindows.ResultFor(offset) ?? HitResult.Miss });
            if (maniaHit == column.HitObjects.First()) removeFromColumn(column, maniaHit);
        }

        protected override void UpdateState(ArmedState state)
        {
        }
        protected void removeFromColumn(SocketCommunication.ColumnHitObject column, ManiaHitObject HitObject)
        {
            column.HitObjects.Remove(HitObject);
        }

        public virtual bool OnPressed(ManiaAction action)
        {
            if (action != Action)
                return false;

            return UpdateJudgement(true);
        }

        public virtual bool OnReleased(ManiaAction action) => false;
    }
}
