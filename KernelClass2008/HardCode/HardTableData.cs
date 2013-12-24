using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace KernelClass
{
    public class HardTableData
    {
        public HardTableData() { }

        public static DataTable CreateBigClass()
        {
            DataTable oDT = new DataTable("BigClass");

            oDT.Columns.Add("BigClassID", typeof(int));
            oDT.Columns.Add("BigClassName", typeof(string));
            oDT.Columns.Add("Status", typeof(int));

            oDT.Rows.Add(1, "NET", 1);
            oDT.Rows.Add(2, "ASP", 1);
            oDT.Rows.Add(3, "XML&XSL", 1);
            oDT.Rows.Add(4, "CSS", 1);
            oDT.Rows.Add(5, "JQuery", 1);
            oDT.Rows.Add(6, "SQL", 1);
            oDT.Rows.Add(7, "HTML", 1);

            return oDT;
        }

        public static DataTable CreateSmallClass()
        {
            DataTable oDT = new DataTable("SmallClass");

            oDT.Columns.Add("BigClassID", typeof(int));
            oDT.Columns.Add("SmallClassID", typeof(int));
            oDT.Columns.Add("SmallClassName", typeof(string));
            oDT.Columns.Add("Status", typeof(int));

            #region insert data
            oDT.Rows.Add(1, 1, "Low Level~!@#$%^&*()_+{}|:\"<>?/.,';\\[]\"_'", 1);
            oDT.Rows.Add(1, 2, "Medium Level", 1);
            oDT.Rows.Add(1, 3, "High Level", 1);

            oDT.Rows.Add(2, 1, "NET - 1", 1);
            oDT.Rows.Add(2, 2, "NET - 2", 1);
            oDT.Rows.Add(2, 3, "NET - 3", 1);
            oDT.Rows.Add(2, 4, "NET - 4", 1);

            oDT.Rows.Add(3, 1, "XML&XSL - 1", 1);
            oDT.Rows.Add(3, 2, "XML&XSL - 2", 1);

            oDT.Rows.Add(4, 1, "CSS - 1", 1);
            oDT.Rows.Add(4, 2, "CSS - 2", 1);

            oDT.Rows.Add(5, 1, "JQuery - 1", 1);

            oDT.Rows.Add(6, 2, "SQL - 2", 1);
            #endregion

            return oDT;
        }

        #region sample product data

        public static DataTable CreateSampleProductData()
        {
            DataTable tbl = new DataTable("Products");

            tbl.Columns.Add("ProductID", typeof(int));
            tbl.Columns.Add("CreateDate", typeof(string));
            tbl.Columns.Add("ProductName", typeof(string));
            tbl.Columns.Add("ProductDesc", typeof(string));
            tbl.Columns.Add("UnitPrice", typeof(decimal));
            tbl.Columns.Add("CategoryID", typeof(int));
            tbl.Columns.Add("Status", typeof(string));

            tbl.Rows.Add(1, "15 Dec 08", "Chai", "This is the product descrtion1", 18, 1, "Enable");
            tbl.Rows.Add(2, "15 Dec 08", "Chai", "This is the product descrtion1", 18, 1, "Enable");
            tbl.Rows.Add(3, "3 Dec 08", "Chang", "This is the product descrtion2", 19, 1, "Disable");
            tbl.Rows.Add(4, "3 Dec 08", "Chang", "This is the product descrtion2", 19, 1, "Disable");
            tbl.Rows.Add(5, "15 Nov 08", "Aniseed Syrup", "This is the product descrtion", 10, 2, "Enable");
            tbl.Rows.Add(6, "10 Oct 08", "Chef Anton's Cajun Seasoning", "This is the product descrtion", 22, 2, "Disable");
            tbl.Rows.Add(7, "21 Aug 08", "Chef Anton's Gumbo Mix", "This is the product descrtion", 21.35, 2, "Enable");
            tbl.Rows.Add(8, "6 Jul 08", "Zaanse koeken", "This is the product descrtion", 9.5, 3, "Disable");
            tbl.Rows.Add(9, "15 Apr 08", "Chocolade", "This is the product descrtion", 12.75, 3, "Enable");
            tbl.Rows.Add(10, "15 Apr 08", "Maxilaku", "This is the product descrtion", 20, 3, "Disable");
            tbl.Rows.Add(11, "15 Nov 08", "Aniseed Syrup", "This is the product descrtion", 10, 2, "Enable");
            tbl.Rows.Add(12, "10 Oct 08", "Chef Anton's Cajun Seasoning", "This is the product descrtion", 22, 2, "Disable");
            tbl.Rows.Add(13, "21 Aug 08", "Chef Anton's Gumbo Mix", "This is the product descrtion", 21.35, 2, "Enable");
            tbl.Rows.Add(14, "6 Jul 08", "Zaanse koeken", "This is the product descrtion", 9.5, 3, "Disable");
            tbl.Rows.Add(15, "15 Apr 08", "Chocolade", "This is the product descrtion", 12.75, 3, "Enable");
            tbl.Rows.Add(16, "15 Apr 08", "Maxilaku", "This is the product descrtion", 20, 3, "Disable");

            tbl.Rows.Add(1, "15 Dec 08", "Chai", "This is the product descrtion", 18, 1, "Enable");
            tbl.Rows.Add(2, "3 Dec 08", "Chang", "This is the product descrtion", 19, 1, "Disable");
            tbl.Rows.Add(3, "15 Nov 08", "Aniseed Syrup", "This is the product descrtion", 10, 2, "Enable");
            tbl.Rows.Add(4, "10 Oct 08", "Chef Anton's Cajun Seasoning", "This is the product descrtion", 22, 2, "Disable");
            tbl.Rows.Add(5, "21 Aug 08", "Chef Anton's Gumbo Mix", "This is the product descrtion", 21.35, 2, "Enable");
            tbl.Rows.Add(47, "6 Jul 08", "Zaanse koeken", "This is the product descrtion", 9.5, 3, "Disable");
            tbl.Rows.Add(48, "15 Apr 08", "Chocolade", "This is the product descrtion", 12.75, 3, "Enable");
            tbl.Rows.Add(49, "15 Apr 08", "Maxilaku", "This is the product descrtion", 20, 3, "Disable");
            tbl.Rows.Add(1, "15 Dec 08", "Chai", "This is the product descrtion", 18, 1, "Enable");
            tbl.Rows.Add(2, "3 Dec 08", "Chang", "This is the product descrtion", 19, 1, "Disable");
            tbl.Rows.Add(3, "15 Nov 08", "Aniseed Syrup", "This is the product descrtion", 10, 2, "Enable");
            tbl.Rows.Add(4, "10 Oct 08", "Chef Anton's Cajun Seasoning", "This is the product descrtion", 22, 2, "Disable");
            tbl.Rows.Add(5, "21 Aug 08", "Chef Anton's Gumbo Mix", "This is the product descrtion", 21.35, 2, "Enable");
            tbl.Rows.Add(47, "6 Jul 08", "Zaanse koeken", "This is the product descrtion", 9.5, 3, "Disable");
            tbl.Rows.Add(48, "15 Apr 08", "Chocolade", "This is the product descrtion", 12.75, 3, "Enable");
            tbl.Rows.Add(49, "15 Apr 08", "Maxilaku", "This is the product descrtion", 20, 3, "Disable");
            return tbl;
        }

        #endregion
    }
}