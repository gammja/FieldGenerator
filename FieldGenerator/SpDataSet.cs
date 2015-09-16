using System.Data;

namespace FieldGenerator
{
    partial class SpDataSet
    {
        partial class SpDataTableRow
        {
            public int FAge
            {
                get { return this.Field<int>(tableSpDataTable.AgeColumn); }
                set { this.SetField<int>(tableSpDataTable.AgeColumn, value); }
            }
        }
    }
}

