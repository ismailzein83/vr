using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallGeneratorLibrary.Utilities;

namespace CallGeneratorLibrary.Models
{
    public class Language
    {
        #region Members
        private string _Code;
        private string _Name;
        private Enums.Align _Align;
        private Enums.Lang _Language;
        private Enums.Direction _Direction;
        private System.Globalization.CultureInfo _Culture;
        #endregion

        #region Properties
        public string Name { get { return _Name; } }
        public string Code { get { return _Code; } }
        public Enums.Align Align { get { return _Align; } }
        public Enums.Lang Lang { get { return _Language; } }
        public Enums.Direction Direction { get { return _Direction; } }
        public System.Globalization.CultureInfo Culture { get { return _Culture; } }
        #endregion

        #region Readonly
        public bool IsArabic { get { return _Language == Enums.Lang.Arabic; } }
        #endregion

        #region Constructor
        public Language(string name, string code, Enums.Align align, Enums.Lang language,
            Enums.Direction direction, System.Globalization.CultureInfo culture)
        {
            this._Name = Name;
            this._Code = code;
            this._Align = align;
            this._Language = language;
            this._Direction = direction;
            this._Culture = culture;
        }
        #endregion

    }
}
