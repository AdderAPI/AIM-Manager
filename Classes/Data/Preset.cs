using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIMAPI.Classes.Data
{
    public class Preset : Base, IEquatable<Preset>
    {
        public int Pairs = 0;
        public int ProblemPairs = 0;

        public ActiveType Active = ActiveType.None;
        public int RXCount = 0;

        public ViewType ViewButton = ViewType.Disabled;
        public ViewType SharedButton = ViewType.Disabled;
        public ViewType ExclusiveButton = ViewType.Disabled;
        public ViewType ControlButton = ViewType.Disabled;        // V4

        public bool Equals(Preset other)
        {
            if (other == null)
                return false;

            return
                Id == other.Id &&
                Name == other.Name &&
                Description == other.Description &&
                Pairs == other.Pairs &&
                ProblemPairs == other.ProblemPairs &&
                Active == other.Active &&
                RXCount == other.RXCount &&
                ViewButton == other.ViewButton &&
                SharedButton == other.SharedButton &&
                ExclusiveButton == other.ExclusiveButton;
        }
    }
}
