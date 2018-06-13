using PROTR.Core;
using PROTR.Core.DataViews;

namespace Demo.Library.Business.Authors
{
    public class AuthorFilter : FilterBase
    {
        public AuthorFilter(BusinessBaseDecorator decorator) : base(decorator, 0)
        {

        }

        public override void SetDataView(DataView dataView)
        {
            base.SetDataView(dataView);

            dataView.FromClause = @"Author INNER JOIN
AuthorNationality ON Author.idAuthorNationality = AuthorNationality.idAuthorNationality";

            dataView.Columns.Add(new DataViewColumn(Decorator.TableNameEncapsulated,
                Decorator.Properties["surname"]));

            dataView.Columns.Add(new DataViewColumn("AuthorNationality.name", "Nationality"));
        }
    }
}
