using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROTR.Core
{
    public partial class BusinessCollectionBase : IList<BusinessBase>
    {
        protected int dbNumber = 0;
        private string parentRelationFieldName;
        protected string childRelationFieldName;
        protected string childObjectName;
        private BusinessBase active;
        public BusinessBase Parent { get; set; }
        private string sql = "";
        protected bool readed = false;
        protected string lastErrorKey = "";
        protected string lastErrorProperty = "";
        protected bool alwaysMustSave = false;
        private List<BusinessBase> list = new List<BusinessBase>();
        private string collectionName = "";

        public BusinessCollectionBase(BusinessBase parent,
            string childObjectName, string sql = "", string childRelationFieldName = "", 
            string parentRelationFieldName = "", int dbNumber = 0)
        {
            Parent = parent;
            this.childObjectName = childObjectName;
            if (parentRelationFieldName == "")
            {
                this.parentRelationFieldName = parent.Decorator.ListProperties[0].FieldName;
            }
            else
            {
                this.parentRelationFieldName = parentRelationFieldName;
            }
            if (childRelationFieldName != "")
            {
                this.childRelationFieldName = childRelationFieldName;
            }
            else
            {
                this.childRelationFieldName = this.parentRelationFieldName;
            }


            if (sql == "")
            {
                sql = "Select * From " + childObjectName
                    + " Where " + this.childRelationFieldName + " = @id";

                PropertyDefinition firstStringField =
                    parent.BusinessProvider.GetDecorator(parent.ContextProvider, childObjectName, dbNumber).FirstStringProperty;

                if (firstStringField != null)
                {
                    sql += " Order By " + firstStringField.FieldName;
                }
            }
            this.sql = sql;

            this.dbNumber = dbNumber;

            if (!parent.BusinessProvider.IsDecoratorCreated(parent.ContextProvider, childObjectName, dbNumber))
            {
                parent.BusinessProvider.GetDecorator(parent.ContextProvider, childObjectName, dbNumber);
            }
        }

        public virtual string CollectionName
        {
            set
            {
                collectionName = value;
            }
            get
            {
                if (collectionName == "")
                {
                    string nom;

                    nom = this.ToString();
                    collectionName = nom.Substring(nom.LastIndexOf(".") + 1);
                }

                return collectionName;
            }
        }

        public async Task EnsureList()
        {
            if (!readed)
            {
                await ReadFromDBInternal();
            }
        }

        public void LeaveOnlyNew()
        {
            for (int i = Count - 1; i >= 0; --i)
            {
                if (!this[i].IsNew)
                {
                    this.Remove(this[i]);
                }
            }

            ClientRefreshPending = true;
            Parent.ClientRefreshPending = true;
            // Changed();  Not here.
        }

        public void Reset()
        {
            this.Clear();
            readed = false;
            ClientRefreshPending = true;
            Parent.ClientRefreshPending = true;
            // Changed();  Not here.
        }

        public virtual void Changed()
        {
            ClientRefreshPending = true;
            Parent.ClientRefreshPending = true;
        }

        public string SQL
        {
            get
            {
                return sql;
            }
            set
            {
                if (sql != value)
                {
                    Reset();
                    sql = value;
                }
            }
        }

        public virtual BusinessBase CreateNewChild()
        {
            return Parent.BusinessProvider.CreateObject(Parent.ContextProvider, childObjectName, dbNumber);
        }

        public BusinessBase CreateNew()
        {
            BusinessBase b;

            b = CreateNewChild();
            b.Parent = this;
            b.SetPropertiesFrom(Parent);

            ActiveObject = b;

            return b;
        }

        public virtual string SQLQuery
        {
            get
            {
                return sql;
            }
        }

        public object SQLParameters
        {
            get
            {
                if (parentRelationFieldName == "")
                {
                    return null;
                }

                return new { id = Parent[parentRelationFieldName] };
            }
        }

        public async Task<int> CountInCollection()
        {
            await EnsureList();

            return this.Count;
        }

        public async Task<int> CountNoRead()
        {
            if (readed)
            {
                return this.Count;
            }
            else
            {
                if (SQLQuery.ToUpper().Contains("ORDER BY"))
                {
                    return await Parent.ContextProvider.DbContext
                        .QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM ("
                            + SQLQuery.Substring(0, SQLQuery.ToUpper().LastIndexOf("ORDER BY")) + ") As D");
                }
                else
                {
                    return await Parent.ContextProvider.DbContext
                        .QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM (" + SQLQuery + ") As D");
                }
            }
        }

        private async Task ReadFromDBInternal()
        {
            bool afterReadFromDBPending = false;

            if (sql != "" && !readed)
            {
                readed = true;
                await Parent.ContextProvider.DbContext.ReadBusinessCollection(this);

                afterReadFromDBPending = true;
            }

            if (afterReadFromDBPending)
            {
                AfterReadFromDB();
            }
        }

        public virtual void AfterReadFromDB()
        {
        }

        public virtual bool Validate()
        {
            bool valid = true;

            lastErrorKey = "";
            lastErrorProperty = "";

            if (readed)
            {
                foreach (BusinessBase b in this)
                {
                    if (!b.Validate())
                    {
                        valid = false;
                        lastErrorKey = b.Key;
                        lastErrorProperty = b.LastErrorProperty;
                        break;
                    }
                }
            }

            return valid;
        }

		public int CountWhenSaving
        {
            get
            {
                int c = 0;

                foreach (BusinessBase b in this)
                {
                    if (!b.IsDeleting)
                    {
                        c++;
                    }
                }

                return c;
            }
        }

        public virtual string Description
        {
            get
            {
                StringBuilder desc = new StringBuilder();

                foreach (BusinessBase b in this)
                {
                    if (b is Specialized.N2M)
                    {
                        if ((bool)b["Active"])
                        {
                            desc.Append(" " + b.Description);
                        }
                    }
                    else
                    {
                        desc.Append(" " + b.Description);
                    }
                }

                return desc.ToString();
            }
        }

        public virtual string LastErrorMessage
        {
            get
            {
                string errorMessage = "";

                if (this.readed)
                {
                    foreach (BusinessBase b in this)
                    {
                        if (b.LastErrorMessage != "")
                        {
                            if (errorMessage != "")
                            {
                                errorMessage += ". ";
                            }
                            errorMessage += b.LastErrorMessage;
                        }
                    }
                }

                return errorMessage;
            }
            set
            {
                if (value == "")
                {
                    if (this.readed)
                    {
                        foreach (BusinessBase b in this)
                        {
                            b.LastErrorMessage = "";
                        }
                    }
                }
                else
                {
                    Parent.LastErrorMessage = value;
                }
            }
        }

        public string LastErrorKey
        {
            get
            {
                return lastErrorKey;
            }
            set
            {
                lastErrorKey = value;
            }
        }
        public string LastErrorProperty
        {
            get
            {
                return lastErrorProperty;
            }
            set
            {
                lastErrorProperty = value;
            }
        }

        public bool MustSave
        {
            get
            {
                if (alwaysMustSave)
                {
                    return true;
                }
                else
                {
                    return readed;
                }
            }
        }

        public virtual async Task StoreToDB()
        {
            if (!readed && !Parent.IsDeleting)
            {
                return;
            }
            await EnsureList();
            foreach (BusinessBase b in this)
            {
                if (parentRelationFieldName != "")
                {
                    if (b[childRelationFieldName] == null)
                    {
                        b[childRelationFieldName] = Parent[parentRelationFieldName];
                    }
                    else
                    {
                        if (Parent.Decorator.PrimaryKeyIsOneInt)
                        {
                            if ((int)b[childRelationFieldName] != (int)Parent[parentRelationFieldName])
                            {
                                b[childRelationFieldName] = Parent[parentRelationFieldName];
                            }
                        }
                        else if (Parent.Decorator.PrimaryKeyIsOneLong)
                        {
                            if ((long)b[childRelationFieldName] != (long)Parent[parentRelationFieldName])
                            {
                                b[childRelationFieldName] = Parent[parentRelationFieldName];
                            }
                        }
                        else
                        {
                            if (((IComparable)b[childRelationFieldName]).CompareTo(Parent[parentRelationFieldName]) != 0)
                            {
                                b[childRelationFieldName] = Parent[parentRelationFieldName];
                            }
                        }
                    }
                }
                await b.StoreToDB();
            }
            readed = false;
            // For health (N2M and others).
            if (!Parent.IsDeleting)
            {
                CreateNew();
            }
        }

        /// <summary>
        /// Elimina los registros nuevos de la colección y marca para eliminar los que existen.
        /// </summary>
		public virtual async Task SetForDeletion()
        {
            await EnsureList();
            DeleteNewObjects();
            foreach (BusinessBase b in this)
            {
                if (b.CanDelete)
                {
                    b.IsDeleting = true;
                }
            }
        }

        private void DeleteNewObjects()
        {
            // No se puede usar foreach.
            // Son registros nuevos no guardados.
            for (int c = 0; c < this.Count; ++c)
            {
                BusinessBase b = this[c];
                if (b.IsNew)
                {
                    b = null;
                    RemoveAt(c);
                    --c;
                }
            }

            ClientRefreshPending = true;
            Parent.ClientRefreshPending = true;
        }

        public virtual async Task SetNew(bool preserve = false, bool withoutCollections = false)
        {
            await EnsureList();
            foreach (BusinessBase b in this)
            {
                if (b.IsDeleting && !b.IsNew)
                {
                    b.IsDeleting = false;
                }
                await b.SetNew(preserve, withoutCollections);
            }
        }

        public virtual async Task CopyTo(BusinessCollectionBase colTarget)
        {
            await EnsureList();
            foreach (BusinessBase objOrigen in this)
            {
                bool isNew = false;
                BusinessBase objTarget = colTarget.Search(objOrigen);

                if (objTarget == null)
                {
                    isNew = true;
                    objTarget = colTarget.CreateNew();
                }

                objOrigen.CopyTo(objTarget, null);

                if (isNew)
                {
                    colTarget.AddActiveObject();
                }
            }

            colTarget.CreateNew();
        }

        public virtual BusinessBase CreateActiveObjectCopy()
        {
            BusinessBase target = CreateNewChild();

            target.Parent = this;

            ActiveObject.CopyTo(target, null);

            ActiveObject = target;

            return ActiveObject;
        }

        public virtual void AddActiveObject()
        {
            EnsureList().RunSynchronously();
            if (!this.Contains(this.ActiveObject))
            {
                if (!CanAddActiveObject)
                {
                    throw new System.Exception("Object cannot be added.");
                }
                Add(ActiveObject);

                Changed();
            }
        }

        public virtual void InsertActiveObject(int index)
        {
            EnsureList().RunSynchronously();
            if (!this.Contains(this.ActiveObject))
            {
                if (!CanAddActiveObject)
                {
                    throw new System.Exception("Object cannot be inserted.");
                }
                this.Insert(index, this.ActiveObject);

                Changed();
            }
        }

        public bool CanAddActiveObject
        {
            get
            {
                if (readed)
                {
                    EnsureList().RunSynchronously();
                    if (ActiveObject.Decorator.PrimaryKeys.Count != 1)
                    {
                        foreach (BusinessBase b in this)
                        {
                            if (ActiveObject == b)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
        }

        public async Task SetActiveObject(int Position)
        {
            await EnsureList();
            ActiveObject = this[Position];
        }

        public async Task SetActiveObject(string Key)
        {
            await SetActiveObject(Key, "");
        }

        public async Task<int> ActiveObjectPosition(string filterName)
        {
            int indice = -1;

            await EnsureList();
            if (filterName == "")
            {
                indice = this.IndexOf(ActiveObject);
            }
            else
            {
                int i = 0;

                foreach (BusinessBase b in Filter(filterName))
                {
                    if ((object)ActiveObject == (object)b)
                    {
                        indice = i;
                        break;
                    }
                    ++i;
                }
            }

            return indice;
        }

        public async Task SetActiveObject(string Key, string filterName)
        {
            await EnsureList();
            foreach (BusinessBase b in Filter(filterName))
            {
                if (b.Key == Key)
                {
                    ActiveObject = b;
                    break;
                }
            }
        }

        public virtual BusinessBase Search(BusinessBase searchObject)
        {
            if (searchObject is Specialized.N2M)
            {
                foreach (BusinessBase b in this)
                {
                    string ownFieldNameM = ((Specialized.N2M)searchObject).ExternalFieldNameM;

                    if ((int)b[ownFieldNameM] == (int)searchObject[ownFieldNameM])
                    {
                        return b;
                    }
                }
            }
            else
            {
                return this[searchObject.Key];
            }

            return null;
        }

        public virtual BusinessBase ActiveObject
        {
            get
            {
                if (active == null)
                {
                    EnsureList().RunSynchronously();
                }

                if (active == null)  // Multitasking issue...
                {
                    ReadFromDBInternal().RunSynchronously();
                }

                return active;
            }
            set
            {
                active = value;
                if (active != null)
                {
                    active.Parent = this;
                    if (parentRelationFieldName != "")
                    {
                        if (active[childRelationFieldName] == null)
                        {
                            if (Parent.Decorator.PrimaryKeyIsOneInt)
                            {
                                if (Parent[parentRelationFieldName].NoNullInt() > 0)
                                {
                                    active[childRelationFieldName] = Parent[parentRelationFieldName];
                                }
                            }
                            else if (Parent.Decorator.PrimaryKeyIsOneLong)
                            {
                                if (Parent[parentRelationFieldName].NoNullLong() > 0)
                                {
                                    active[childRelationFieldName] = Parent[parentRelationFieldName];
                                }
                            }
                            else
                            {
                                if (Parent[parentRelationFieldName] != null)
                                {
                                    active[childRelationFieldName] = Parent[parentRelationFieldName];
                                }
                            }
                        }
                        else
                        {
                            if (Parent.Decorator.PrimaryKeyIsOneInt)
                            {
                                if (active[childRelationFieldName].NoNullInt() != Parent[parentRelationFieldName].NoNullInt())
                                {
                                    active[childRelationFieldName] = Parent[parentRelationFieldName];
                                }
                            }
                            else if (Parent.Decorator.PrimaryKeyIsOneLong)
                            {
                                if (active[childRelationFieldName].NoNullLong() != Parent[parentRelationFieldName].NoNullLong())
                                {
                                    active[childRelationFieldName] = Parent[parentRelationFieldName];
                                }
                            }
                            else
                            {
                                if (((IComparable)active[childRelationFieldName]).CompareTo(Parent[parentRelationFieldName]) != 0)
                                {
                                    active[childRelationFieldName] = Parent[parentRelationFieldName];
                                }
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable Filter(string filterName)
        {
            if (filterName == "")
            {
                return this;
            }
            else
            {
                return new BusinessCollectionFiltered (this, filterName);
            }
        }

        public BusinessBase this[string key]
        {
            get
            {
                EnsureList().RunSynchronously();

                if (key == "0") // Carefully...
                {
                    if (!ActiveObject.IsNew)
                    {
                        this.CreateNew();
                    }

                    return ActiveObject;
                }
                else
                {
                    if (ActiveObject.Key == key)
                    {
                        return ActiveObject;
                    }

                    for (int c = 0; c < list.Count; ++c)
                    {
                        if (this[c].Key == key)
                        {
                            return this[c];
                        }
                    }
                }

                return null;
            }
        }

        public virtual bool IsModified
        {
            get
            {
                bool mod = false;

                if (readed)
                {
                    foreach (BusinessBase obj in this)
                    {
                        if (obj.IsModified || obj.IsNew || obj.IsDeleting)
                        {
                            mod = true;
                            break;
                        }
                    }
                }

                return mod;
            }
        }

        public virtual bool IsSomeOneNewOrChanged
        {
            get
            {
                bool mod = false;

                if (readed)
                {
                    foreach (BusinessBase b in this)
                    {
                        if (b.IsNewOrChanged)
                        {
                            mod = true;
                            break;
                        }
                    }
                }

                return mod;
            }
        }

        public int IndexOf(BusinessBase item)
        {
            if (item == null)
            {
                return -1;
            }
            EnsureList().RunSynchronously();
            return (list.IndexOf(item));
        }

        public void Insert(int index, BusinessBase item)
        {
            EnsureList().RunSynchronously();
            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            EnsureList().RunSynchronously();
            list.RemoveAt(index);
            Changed();
        }

        public BusinessBase this[int index]
        {
            get
            {
                EnsureList().RunSynchronously();
                return ((BusinessBase)list[index]);
            }
            set
            {
                EnsureList().RunSynchronously();
                list[index] = value;
            }
        }

        public void Add(BusinessBase item)
        {
            EnsureList().RunSynchronously();
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(BusinessBase item)
        {
            EnsureList().RunSynchronously();
            return list.Contains(item);
        }

        public bool ContainsWithOutEnsureList(BusinessBase item)
        {
            return list.Contains(item);
        }

        public void CopyTo(BusinessBase[] array, int arrayIndex)
        {
            EnsureList().RunSynchronously();
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                EnsureList().RunSynchronously();
                return list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(BusinessBase item)
        {
            bool removed = false;
            EnsureList().RunSynchronously();

            removed = list.Remove(item);

            Changed();

            return removed;
        }

        public IEnumerator<BusinessBase> GetEnumerator()
        {
            EnsureList().RunSynchronously();
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            EnsureList().RunSynchronously();
            return list.GetEnumerator();
        }

        public void MoveObject(bool up, string key)
        {
            if (this.Count > 0)
            {
                int pos = this.IndexOf(this[key]);

                if (pos >= 0)
                {
                    int posOther = pos;

                    if (up)
                    {
                        if (pos > 0)
                        {
                            posOther = pos - 1;
                        }
                    }
                    else
                    {
                        if (pos < this.Count - 1)
                        {
                            posOther = pos + 1;
                        }
                    }

                    Swap(pos, posOther);
                }
            }
        }

        public void Swap(int pos1, int pos2)
        {
            if (pos1 != pos2)
            {
                lock (this)
                {
                    BusinessBase tmp = this[pos1];

                    this[pos1] = this[pos2];
                    this[pos2] = tmp;
                }
                //this[pos1].SwapData(this[pos2]); ???

                this[pos1].ClientRefreshPending = true;
                this[pos2].ClientRefreshPending = true;

                ClientRefreshPending = true;
            }
        }

        public string SQLListUsingField(string fieldName)
        {
            string list = "";
            bool isNumeric = ActiveObject.Decorator.Properties[fieldName].BasicType
                == BasicType.Number;

            foreach (BusinessBase obj in this)
            {
                if (list != "")
                {
                    list += ",";
                }
                if (isNumeric)
                {
                    list += obj[fieldName].ToString();
                }
                else
                {
                    list += "'" + obj[fieldName].ToString().Replace("'", "''") + "'";
                }
            }

            if (list == "")
            {
                list = (isNumeric ? "0" : "''");
            }

            return list;
        }
    }

    public class BusinessCollectionFiltered : IEnumerable, ICollection
    {
        BusinessCollectionFilteredEnumerator filtered;

        public BusinessCollectionFiltered(BusinessCollectionBase col, string filterName)
        {
            filtered = new BusinessCollectionFilteredEnumerator(col, filterName);

            while (filtered.MoveNext())
            {
                Count++;
            }
            filtered.Reset();
        }

        #region IEnumerable members
        public IEnumerator GetEnumerator()
        {
            return filtered;
        }
        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
        }

        public int Count { get; } = 0;

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion
    }

    public class BusinessCollectionFilteredEnumerator : IEnumerator
    {
        BusinessBase actual;
        BusinessCollectionBase _col;
        readonly string _filterName;

        int pos;

        public BusinessCollectionFilteredEnumerator(BusinessCollectionBase col, string filterName)
        {
            _col = col;
            _filterName = filterName;
            Reset();
        }

        #region IEnumerator members
        public void Reset()
        {
            actual = null;
            pos = -1;
        }

        public object Current
        {
            get
            {
                return actual;
            }
        }

        public bool MoveNext()
        {
            bool found = false;

            pos++;
            while (pos < _col.Count)
            {
                actual = _col[pos];
                if (actual.MatchFilter(_filterName))
                {
                    found = true;
                    break;
                }
                pos++;
            }

            return found;
        }
        #endregion
    }
}
