using SqlUniversity.DataAccess.Models;

namespace SqlUniversity.DataAccess.Repository
{

    public interface IRepositoryBase<TModel> where TModel : class
    {
        TModel Get(Predicate<TModel> selector);

        TModel Insert(TModel instance);
        IEnumerable<TModel> GetAll();
    }



    public abstract class RepositoryBase<TModel> where TModel : EntityDbBase
    {
        protected HashSet<TModel> Models;

        public RepositoryBase()
        {
            Models = new HashSet<TModel>();
        }


        public virtual TModel Get(Predicate<TModel> selector)
        {
            foreach (var model in Models)
            {
                if(selector(model))
                {
                    return model;
                }
            }

            return null;
        }
        /// <summary>
        /// Auto Id Generator
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public TModel Insert(TModel instance)
        {
            var newId = Models.Count + 1;
            instance.Id = newId;

            Models.Add(instance);
            return instance;
        }
        public IEnumerable<TModel> GetAll()
        {
            return Models.ToArray();
        }
    }
}
