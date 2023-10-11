using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculator.Service.Models
{
   
    public class DatatableModel
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }                 
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
    }
    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; }
    }

    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

    public class JqueryDatatableParam
    {
        public string sEcho { get; set; }
        public string sSearch { get; set; }
        public int iDisplayLength { get; set; }
        public int iDisplayStart { get; set; }
        public int iColumns { get; set; }
        public int iSortCol_0 { get; set; }
        public string sSortDir_0 { get; set; }
        public int iSortingCols { get; set; }
        public string sColumns { get; set; }
    }

    public class DatatableResponseModel<T>
    {
        public int draw { get; set; }
        public long recordsTotal { get; set; }
        public long recordsFiltered { get; set; }
        public List<T> data { get; set; }
    }

    public class DataTablePaginationModel
    {
        public int? DtPageNumber { get; set; }
        public int? DtPageSize { get; set; }
        public string DtSearch { get; set; }
        public string DtSortColumn { get; set; }
        public string DtSortOrder { get; set; }
    }
}