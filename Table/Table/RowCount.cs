using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table
{
    internal class RowCount 
    {
        internal RowCount()
        {
            _length = 0;
        }
        internal int Count
        {
            get { return _length; }
            set { _length = value; }
        }
        internal int adjust(int i = 1)
        {
            _length += i;
            return _length;
        }
        internal int LastRowId
        {
            get { return _length - 1; }
        }
        #region private
        private int _length;
        #endregion
     }
 
}
