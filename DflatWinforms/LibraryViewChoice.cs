using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DflatWinforms
{
    class LibraryViewChoice
    {
        private readonly string _displayName;
        private readonly string _choice;

        public LibraryViewChoice(string displayName, string choice)
        {
            _displayName = displayName;
            _choice = choice;
        }

        public string DisplayName
        {
            get { return _displayName; }
        }

        public string Choice
        {
            get { return _choice; }
        }
    }
}
