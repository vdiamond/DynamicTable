using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table;
namespace Table
{
    public class DynamicTable : IDynamicTable
    {
        public DynamicTable()
        {
            _rowCount = new RowCount();
            _columns = new Columns(_rowCount);
         }
        public IColumns columns
        {
            get { return _columns; }
        }
        public IRows rows
        {
            get { return new Rows(_rowCount, _columns); }
        }
        #region private
        private Columns _columns;
        private RowCount _rowCount;
        #endregion
    }

}
