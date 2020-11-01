using System;
using System.Collections.Generic;
using System.Text;

namespace GK2823.ModelLib.Shared
{
    public class Page<T>
    {
        public List<T> row { get; set; }
        public int totalNum { get; set; }
    }

    public class PageSet
    {
        public string table { get; set; }
        public string column { get; set; }
        public string sqlOrder { get; set; }
        public string sqlWhere { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }

        public PageSet()
        {
            this.column = "*";
            this.sqlOrder = " order by id ";
            this.pageSize = 20;
            this.pageIndex = 1;
        }
    }
}
