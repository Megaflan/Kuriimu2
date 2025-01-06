using Kompression.Contract.DataClasses.Encoder.LempelZiv;

namespace Kompression.DataClasses.Encoder.LemeplZiv.MatchParser
{
    internal class MatchParserPositionData
    {
        private int _runValue;

        public MatchParserPositionData? Parent { get; set; }

        public LempelZivMatch? Match { get; set; }

        public int Price { get; set; }

        public int CurrentRunLength
        {
            get => IsMatchRun ? -_runValue : _runValue;
            set => _runValue = IsMatchRun ? -value : value;
        }

        public bool IsMatchRun
        {
            get => _runValue < 0;
            set
            {
                if (IsMatchRun != value)
                    _runValue = -_runValue;
            }
        }

        public MatchParserPositionData(int currentRunLength, bool isMatchRun)
        {
            _runValue = isMatchRun ? -currentRunLength : currentRunLength;
        }

        public MatchParserPositionData(int currentRunLength, bool isMatchRun, MatchParserPositionData? parent, int price) :
            this(currentRunLength, isMatchRun)
        {
            Parent = parent;
            Price = price;
        }
    }
}
