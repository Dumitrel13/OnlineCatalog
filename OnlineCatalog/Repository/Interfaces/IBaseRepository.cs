namespace OnlineCatalog.Repository.Interfaces
{
	public interface IBaseRepository<T> where T : class
	{
		Task<ICollection<T>> GetAllAsync();
		Task AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(T entity);
	}
}
